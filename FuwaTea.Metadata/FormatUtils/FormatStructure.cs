using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace FuwaTea.Metadata.FormatUtils
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
                LogManager.GetLogger(GetType()).Warn("Failed to format object:", e);
                return "[Format error, please check settings]"; // TODO: Localize
            }
        }
    }
}
