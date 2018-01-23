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
                case OSKind.OSX: return OSPlatform.OSX;
                default: throw new ArgumentException("Invalid OS kind", nameof(osKind));
            }
        }

        public static bool AppliesToCurrentPlatform([NotNull] this IPlatformFilter filter)
        {
            return RuntimeInformation.IsOSPlatform(filter.OSKind.ToOSPlatform())
                   && filter.ProcessArchitecture.AppliesTo(RuntimeInformation.ProcessArchitecture);
        }

        public static bool AppliesTo(this OSArch arch, System.Runtime.InteropServices.Architecture arch2)
        {
            switch (arch2)
            {
                case System.Runtime.InteropServices.Architecture.X86:
                    return arch.HasFlag(OSArch.X86);
                case System.Runtime.InteropServices.Architecture.X64:
                    return arch.HasFlag(OSArch.X64);
                case System.Runtime.InteropServices.Architecture.Arm:
                    return arch.HasFlag(OSArch.Arm);
                case System.Runtime.InteropServices.Architecture.Arm64:
                    return arch.HasFlag(OSArch.Arm64);
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
