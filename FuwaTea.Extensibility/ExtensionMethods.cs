using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace FuwaTea.Extensibility
{
    public static class ExtensionMethods
    {
        public static OSPlatform ToOSPlatform(this OSKind osKind)
        {
            switch (osKind)
            {
                case OSKind.Windows: return OSPlatform.Windows;
                case OSKind.Linux: return OSPlatform.Linux;
                case OSKind.MacOS: return OSPlatform.OSX;
                default: throw new ArgumentException("Invalid OS kind", nameof(osKind));
            }
        }

        public static bool OSVersionMatches([NotNull] this IPlatformFilter filter)
        {
            var cver = Environment.OSVersion.Version;
            var fver = new Version(filter.Version);
            switch (filter.Rule)
            {
                case FilterRule.Any: return true;
                case FilterRule.Equals: return cver == fver;
                case FilterRule.LessThan: return cver < fver;
                case FilterRule.GreaterThan: return cver > fver;
                case FilterRule.LessThanOrEqualTo: return cver < fver;
                case FilterRule.GreaterThanOrEqualTo: return cver > fver;
                case FilterRule.Between:
                    var fver2 = new Version(filter.OtherVersion);
                    return cver > fver && cver < fver2; // TODO: Consider <= >= combinations
            }

            return false;
        }

        public static bool AppliesTo(this ProcessArch arch, Architecture arch2)
        {
            switch (arch2)
            {
                case Architecture.X86:
                    return (arch & ProcessArch.X86) > 0;
                case Architecture.X64:
                    return (arch & ProcessArch.X64) > 0;
                case Architecture.Arm:
                    return (arch & ProcessArch.Arm) > 0;
                case Architecture.Arm64:
                    return (arch & ProcessArch.Arm64) > 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arch2), arch2, null);
            }
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomAttributeProvider p, bool inherit = true)
            where TAttribute : Attribute
        {
            return p.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
        }
        
        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider p, bool inherit = true)
            where TAttribute : Attribute
        {
            return p.GetCustomAttributes(typeof(TAttribute), inherit).FirstOrDefault() as TAttribute;
        }
    }
}
