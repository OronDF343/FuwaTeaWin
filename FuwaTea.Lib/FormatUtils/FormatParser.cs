using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;

namespace FuwaTea.Lib.FormatUtils
{
    // TODO: Consider saving to cache (serialization)
    public class FormatParser
    {
        private readonly Dictionary<string, FormatStructure> _cache = new Dictionary<string, FormatStructure>();

        public string FormatObject(string format, object source)
        {
            return GetFormat(format).GetValue(source);
        }

        private FormatStructure GetFormat(string format)
        {
            if (_cache.ContainsKey(format)) return _cache[format];

            //	stage 1: find&split
            //	(?<=(?<!\\)(\\\\)*)\{			find an unescaped {
            //	(?:						followed by this expression:
            //		(?(Segment)				if a 'Segment was found before:
            //			(?<=(?<!\\)(\\\\)*)\>			an unescaped >
            //		)						[end]
            //		(?'Segment'.*?)			a 'Segment' (anything, lazy)
            //	)+?						[end] one or more times (lazy)
            //	(?<=(?<!\\)(\\\\)*)\}			followed by an unescaped }
            var i = 0;
            var ccl = new List<IEnumerable<string>>();
            var f = Regex.Replace(format, @"(?<=(?<!\\)(\\\\)*)\{(?:(?(Segment)(?<=(?<!\\)(\\\\)*)\>)(?'Segment'.*?))+?(?<=(?<!\\)(\\\\)*)\}",
                                  match =>
                                  {
                                      ccl.Add(from Capture c in match.Groups["Segment"].Captures
                                              select c.Value);
                                      return $"{{{i++}}}";
                                  });

            var fl = new List<FormatSegment>();
            foreach (var sc in ccl)
            {
                FormatSegment head = null, actualHead = null;
                foreach (var seg in sc)
                {
                    //	stage 2 branch 1: get properties
                    //	\A							at beginning of string
                    //	(?'LastFormatRef'\:)?		zero or one : means 'LastFormatRef'
                    //	(?'PropertyName'			a 'PropertyName':
                    //		(?:\p{Lu}[\p{Ll}]+\.?)+		any number of capitalized words not spaced, dots allowed
                    //	)							[end]
                    //	(?(LastFormatRef)|			if 'LastFormatRef' NOT found
                    //		\:		                    :
                    //		(?'Format'.*?)				the 'Format' (lazy)
                    //	)?							[end] zero or one times
                    //	(?:							expression:
                    //		(?<=(?<!\\)(\\\\)*)\:		unescaped :
                    //		(?'PrivateFormat'.*)		the 'PrivateFormat'
                    //	)?							[end] zero or one time
                    //	\z							end of string reached
                    var m = Regex.Match(seg, @"\A(?'LastFormatRef'\:)?(?'PropertyName'(?:\p{Lu}[\p{Ll}]+\.?)+)(?(LastFormatRef)|\:(?'Format'.*?))?(?:(?<=(?<!\\)(\\\\)*)\:(?'PrivateFormat'.*))?\z");
                    FormatSegment curr;
                    if (m.Success)
                        curr = new FormatSegment
                        {
                            InheritFormat = m.Groups["LastFormatRef"].Success,
                            IsLiteral = false,
                            Content = Unescape(m.Groups["PropertyName"].Value),
                            Format = Unescape(m.Groups["Format"].Value),
                            PrivateFormat = Unescape(m.Groups["PrivateFormat"].Value)
                        };
                    else
                    {
                        //	stage 2 branch 2: get literal
                        //	\A					at beginning of string
                        //	(?'LastFormatRef'\:)?		zero or one : means 'LastFormatRef'
                        //	\""					"
                        //	(?'Literal'.*)		the 'Literal'
                        //	\""					"
                        //	\z					end of string reached
                        m = Regex.Match(seg, @"\A(?'LastFormatRef'\:)?\""(?'Literal'.*)\""\z");
                        if (!m.Success) LogManager.GetLogger(GetType()).Warn("Syntax error in format string!");
                        curr = new FormatSegment
                        {
                            InheritFormat = m.Groups["LastFormatRef"].Success,
                            IsLiteral = true,
                            Content = Unescape(m.Groups["Literal"].Value)
                        };
                    }
                    if (head != null) head.Next = curr;
                    else actualHead = curr;
                    head = curr;
                }
                fl.Add(actualHead);
            }
            var s = new FormatStructure {FormatString = Unescape(f), Segments = fl};
            _cache.Add(format, s);
            return s;
        }

        private static string Unescape(string str)
        {
            // unescape the remaining characters
            // find:
            // \\       find \
            // (.)      followed by one character (any) -> to capture group 1 [that is what the () are for]
            // replace:
            // $1       replace with capture group 1
            return Regex.Replace(str, @"\\(.)", @"$1");
        }
    }
}
