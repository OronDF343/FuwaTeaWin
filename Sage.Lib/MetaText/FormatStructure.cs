using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace Sage.Lib.MetaText
{
    public class FormatStructure
    {
        public List<FormatSegment> Segments { get; set; } = new List<FormatSegment>();
        public string FormatString { get; set; }

        public string GetValue(object source)
        {
            try {
                return string.Format(FormatString, Segments.Select(s => s.GetValue(source)).Cast<object>().ToArray());
            }
            catch (Exception e)
            {
                Log.ForContext(GetType()).Warning("Failed to format object:", e);
                return null;
            }
        }
    }
}
