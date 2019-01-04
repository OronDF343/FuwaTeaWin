using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace FuwaTea.Extensibility
{
    /// <summary>
    /// Various utility methods and extension methods.
    /// </summary>
    public static class Utils
    {
        public static AssemblyLoadResult TryLoadAssembly(this AssemblyName an, out Assembly a)
        {
            a = null;
            try
            {
                a = Assembly.Load(an);
                return a == null ? AssemblyLoadResult.NullResult : AssemblyLoadResult.OK;
            }
            catch (BadImageFormatException) { return AssemblyLoadResult.BadImageFormat; }
            catch (FileNotFoundException) { return AssemblyLoadResult.FileNotFound; }
            catch (FileLoadException) { return AssemblyLoadResult.FileLoadFailed; }
        }

        public static string MakeScopedKey(string module, string area, string name)
        {
            return module + "." + area + "." + name;
        }

        public static bool CheckApiVersion(int ver, bool overrideWhitelist = false)
        {
            return ExtensibilityConstants.ApiVersionWhitelist.Contains(ver)
                   || overrideWhitelist && ver <= ExtensibilityConstants.CurrentApiVersion && !ExtensibilityConstants.ApiVersionBlacklist.Contains(ver);
        }
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
            var cVer = Environment.OSVersion.Version;
            var fVer = new Version(filter.Version);
            switch (filter.Rule)
            {
                case FilterRule.Any: return true;
                case FilterRule.Equals: return cVer == fVer;
                case FilterRule.LessThan: return cVer < fVer;
                case FilterRule.GreaterThan: return cVer > fVer;
                case FilterRule.LessThanOrEqualTo: return cVer <= fVer;
                case FilterRule.GreaterThanOrEqualTo: return cVer >= fVer;
                case FilterRule.Between:
                    var fVer2 = new Version(filter.OtherVersion);
                    return cVer > fVer && cVer < fVer2; // TODO: Consider <= >= combinations
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