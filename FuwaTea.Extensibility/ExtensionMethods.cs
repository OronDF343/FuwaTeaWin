using System;
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

        public static bool AppliesTo(this Architecture arch, System.Runtime.InteropServices.Architecture arch2)
        {
            switch (arch2)
            {
                case System.Runtime.InteropServices.Architecture.X86:
                    return arch.HasFlag(Architecture.X86);
                case System.Runtime.InteropServices.Architecture.X64:
                    return arch.HasFlag(Architecture.X64);
                case System.Runtime.InteropServices.Architecture.Arm:
                    return arch.HasFlag(Architecture.Arm);
                case System.Runtime.InteropServices.Architecture.Arm64:
                    return arch.HasFlag(Architecture.Arm64);
                default:
                    throw new ArgumentOutOfRangeException(nameof(arch2), arch2, null);
            }
        }
    }
}
