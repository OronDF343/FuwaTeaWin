using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace FuwaTea.Metadata
{
    public static class FormatUtils
    {
        private static readonly Dictionary<string, Func<IMusicInfoModel, string>> FormatElements = new Dictionary<string, Func<IMusicInfoModel, string>>
        {
            {"$FN", info => info.FileName},
            {"$FD", info => info.FilePath.Substring(0, info.FilePath.LastIndexOf('\\'))},
            {"$FP", info => info.FilePath},
            {"$TI", info => info.Tag.Title},
            {"$P1", info => info.Tag.FirstPerformer},
            {"$PJ", info => info.Tag.JoinedPerformers},
            {"$A1", info => info.Tag.FirstAlbumArtist},
            {"$AJ", info => info.Tag.JoinedAlbumArtists},
            {"$AL", info => info.Tag.Album},
            {"$TN", info => info.Tag.Track.ToString(CultureInfo.InvariantCulture)},
            {"$G1", info => info.Tag.FirstGenre},
            {"$GJ", info => info.Tag.JoinedGenres},
            {"$YR", info => info.Tag.Year.ToString(CultureInfo.InvariantCulture)},
            {"$DU", info => info.Duration.ToString()},
            {"$CO", info => Path.GetExtension(info.FilePath)?.ToLowerInvariant()},
            {"$BT", info => info.Bitrate.ToString(CultureInfo.InvariantCulture)}
        };
        
        // In short:
        // (?<=(?<![\$])(\$\$)*)    matches only if even number of $ preceeds this
        // \$\w{2}                  what we're looking for
        // (\(.*?(?<!\$)\))?        optional: ( followed by: anyting (lazy) followed by: ) not preceeded by $
        //
        // In very long:
        //  (?<=                before the requested string, make sure that:
        //          (?<![\$])           there must not be $ before this [remember that we are moving backwards, we need to make sure we count all the consecutive $ as far back as possible]
        //      (\$\$)*             there are zero or more pairs of $ 
        //  )                   [after this there can only be 0 or 1 $ left. 0 means there is nothing unescaped here]
        //
        //  \$\w{2}             find $ followed by 2 alphanumeric characters [this is what we are looking for]
        //
        //  (                   [optional, must be right after what we found]
        //      \(                  find ( 
        //      .*?                 followed by zero or more of any character 
        //          (?<!\$)             the last of these characters must not be $ [so the ) is not escaped. looking backwards because we want to allow empty ()]
        //      \)                  followed by )
        //  )?                  this is optional, only include this if it exists
        // this will give us $xx or $xx(...)
        private const string RegexString = @"(?<=(?<![\$])(\$\$)*)\$\w{2}(\(.*?(?<!\$)\))?";


        private static string ParseWithFallback(IMusicInfoModel m, string h)
        {
            // we have $xx or $xx(...)
            // get the value of $xx:
            string s;
            try { s = FormatElements[h.Substring(0, 3)](m); }
            catch { s = ""; }

            // if there is no need to fallback, return the value
            if (!string.IsNullOrWhiteSpace(s)) return s;

            // we need to fallback
            // if we don't have a fallback, return a default string
            // if we do, lets use our regex to parse it, recursively calling this method where needed
            return h.Length < 5 ? "[Unknown]" : Regex.Replace(h.Substring(4, h.Length - 5), RegexString, match => ParseWithFallback(m, match.Value));
        }

        public static string FormatHeader(IMusicInfoModel m, string h)
        {
            try
            {
                // use our regex to parse this, call ParseWithFallback to evaluate variables and fallback
                var r = Regex.Replace(h, RegexString, match => ParseWithFallback(m, match.Value));
                // unescape the remaining characters
                // find:
                // \$       find $
                // (.)      followed by one character (any) -> to capture group 1 [that is what the () are for]
                // replace:
                // $1       replace with capture group 1
                return Regex.Replace(r, @"\$(.)", @"$1");
            }
            catch
            {
                return "[Format Error]";
            }
        }
    }
}
