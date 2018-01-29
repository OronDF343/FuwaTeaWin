using System;
using System.ComponentModel.Composition;

namespace FuwaTea.Extensibility
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface)]
    [MetadataAttribute]
    public class PlatformFilterAttribute : Attribute, IPlatformFilter
    {
        public PlatformFilterAttribute(FilterAction action, OSKind osKind, ProcessArch procArch = ProcessArch.Any,
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
        public ProcessArch ProcessArchitecture { get; }
        public string Version { get; }
        public string OtherVersion { get; set; }
    }
}
