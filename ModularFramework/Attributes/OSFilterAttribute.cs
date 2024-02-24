using System;

namespace ModularFramework.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public class OSFilterAttribute : Attribute
    {
        public OSFilterAttribute(FilterActions action, PlatformID osType, FilterRules rule, string osVersion)
        {
            Action = action;
            OSType = osType;
            Rule = rule;
            Version = osVersion;
        }

        public FilterActions Action { get; set; }
        public PlatformID OSType { get; set; }
        public FilterRules Rule { get; set; }
        public string Version { get; set; }

        public bool AppliesTo(OperatingSystem os)
        {
            if (OSType != os.Platform) return false;
            switch (Rule)
            {
                case FilterRules.Any:
                    return true;
                case FilterRules.Equals:
                    return os.Version == new Version(Version);
                case FilterRules.LessThan:
                    return os.Version < new Version(Version);
                case FilterRules.GreaterThan:
                    return os.Version > new Version(Version);
            }
            return false;
        }
    }

    public enum FilterActions : byte
    {
        Blacklist = 1, Whitelist = 2
    }

    public enum FilterRules : byte
    {
        Any = 0, LessThan = 1, Equals = 2, GreaterThan = 3
    }
}
