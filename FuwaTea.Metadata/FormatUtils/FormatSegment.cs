using System;
using log4net;
using ModularFramework;

namespace FuwaTea.Metadata.FormatUtils
{
    /// <summary>
    /// Bi-directional linked-list :P LOL
    /// </summary>
    public class FormatSegment
    {
        /// <summary>
        /// Should be true only for the last segment.
        /// </summary>
        public bool IsLiteral { get; set; }
        /// <summary>
        /// If IsLiteral=false this is a Property name, otherwise just a string.
        /// </summary>
        public string Content { get; set; }
        private string _format;
        /// <summary>
        /// Setter only effective if InheritFormat=false, however the underlying field is still changed.
        /// </summary>
        public string Format { get { return InheritFormat ? Parent?.Format : _format; } set { _format = value; } }
        /// <summary>
        /// Should never be set for the first segment.
        /// </summary>
        public bool InheritFormat { get; set; }
        /// <summary>
        /// Format without inheritance (applied first)
        /// </summary>
        public string PrivateFormat { get; set; }

        private FormatSegment _parent;
        /// <summary>
        /// If this property is null then this is the first segment.
        /// </summary>
        public FormatSegment Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null) _parent._next = null;
                _parent = value;
                if (_parent != null) _parent._next = this;
            }
        }

        private FormatSegment _next;

        /// <summary>
        /// If this property is null then this is the last segment.
        /// </summary>
        public FormatSegment Next
        {
            get { return _next; }
            set
            {
                if (_next != null) _next._parent = null;
                _next = value;
                if (_next != null) _next._parent = this;
            }
        }

        [CanBeNull]
        public string GetValue(object source)
        {
            try
            {
                if (Content == null)
                {
                    LogManager.GetLogger(GetType()).Warn("Format string syntax error detected!");
                    return "[Format syntax error, please check settings]"; // TODO: Localize
                }
                if (IsLiteral) return Content;
                var c = Content;
                var prop = source;
                while (!string.IsNullOrWhiteSpace(c))
                {
                    var dot = c.IndexOf('.');
                    dot = dot < 0 ? c.Length : dot;
                    var pdef = prop?.GetType().GetProperty(c.Substring(0, dot));
                    c = dot < c.Length ? c.Substring(dot + 1) : "";
                    if (pdef == null)
                    {
                        LogManager.GetLogger(GetType()).Warn($"Property not found for type {prop?.GetType().FullName}: {Content}");
                        prop = null;
                        break;
                    }
                    prop = pdef.GetValue(prop);
                }
                if (!string.IsNullOrEmpty(PrivateFormat))
                    prop = string.Format($"{{0:{PrivateFormat}}}", prop);
                if (!string.IsNullOrEmpty(Format))
                    prop = string.Format(Format, prop);
                var r = prop?.ToString();
                return !string.IsNullOrEmpty(r) ? r : Next?.GetValue(source);
            }
            catch (Exception e)
            {
                LogManager.GetLogger(GetType()).Warn("Object formatting error:", e);
                return Next?.GetValue(source);
            }
        }
    }
}
