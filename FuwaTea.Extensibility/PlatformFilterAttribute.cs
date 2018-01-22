using System;
using System.ComponentModel.Composition;

namespace FuwaTea.Extensibility
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    [MetadataAttribute]
    public class PlatformFilterAttribute : Attribute, IPlatformFilter
    {
        public PlatformFilterAttribute(FilterAction action, OSKind osKind, Architecture procArch = Architecture.Any)
        {
            Action = action;
            OSKind = osKind;
            //Rule = rule;
            ProcessArchitecture = procArch;
            //Version = version;
        }

        public FilterAction Action { get; }
        public OSKind OSKind { get; }
        //public FilterRule Rule { get; }
        public Architecture ProcessArchitecture { get; }
        //public string Version { get; }
    }
}
