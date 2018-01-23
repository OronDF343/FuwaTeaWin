using System;
using System.ComponentModel.Composition;

namespace FuwaTea.Extensibility
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    [MetadataAttribute]
    public class PlatformFilterAttribute : Attribute, IPlatformFilter
    {
        public PlatformFilterAttribute(FilterAction action, OSKind osKind, OSArch procArch = OSArch.Any,
                                       FilterRule rule = FilterRule.Any, string version = null)
        {
            Action = action;
            OSKind = osKind;
            ProcessArchitecture = procArch;
            Rule = rule;
            Version = version;
        }

        public FilterAction Action { get; }
        public OSKind OSKind { get; }
        public FilterRule Rule { get; }
        public OSArch ProcessArchitecture { get; }
        public string Version { get; }
    }
}
