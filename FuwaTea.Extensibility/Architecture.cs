using System;

namespace FuwaTea.Extensibility
{
    [Flags]
    public enum Architecture
    {
        Other = 0,
        X86 = 1,
        X64 = 2,
        Arm = 4,
        Arm64 = 8,
        Is32Bit = X86 | Arm,
        Is64Bit = X64 | Arm64,
        Any = Is32Bit | Is64Bit
    }
}
