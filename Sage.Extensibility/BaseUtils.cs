using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Sage.Extensibility.Attributes;

namespace Sage.Extensibility
{
    /// <summary>
    /// Various utility methods and extension methods.
    /// </summary>
    public static class BaseUtils
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

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));
            //if (toPath.StartsWith("file:")) return toPath.Remove(0, 5); // changed

            var toUri = new Uri(toPath);

            // Fix directory paths that do not end with slash
            // Check for missing slash (forward on all platforms, also backslash on Windows)
            if (!fromPath.EndsWith(Path.DirectorySeparatorChar.ToString()) && !fromPath.EndsWith("/"))
            {
                // If it would be a file scheme, add platform slash
                if (new Uri(fromPath).Scheme.ToLowerInvariant() == "file")
                    fromPath += Path.DirectorySeparatorChar;
                // If not, add forward slash
                else fromPath += "/";
            }

            var fromUri = new Uri(fromPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // Mismatched scheme: Path can't be made relative.

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
            return toUri.Scheme.ToLowerInvariant() == "file"
                       ? relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                       : relativePath;
        }

        /// <summary>
        /// Creates an absolute path from a relative path given the 
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string MakeAbsolutePath(string srcPath, string targetPath)
        {
            if (string.IsNullOrEmpty(srcPath)) throw new ArgumentNullException(nameof(srcPath));
            if (string.IsNullOrEmpty(targetPath)) throw new ArgumentNullException(nameof(targetPath));
            return Path.IsPathRooted(targetPath) ? targetPath : Path.Combine(srcPath, targetPath);
        }
    }
}