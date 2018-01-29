﻿using System;

namespace FuwaTea.Extensibility
{
    [Flags]
    public enum FilterRule : byte
    {
        Any = 0,
        LessThan = 1,
        GreaterThan = 2,
        Equals = 4,
        LessThanOrEqualTo = LessThan | Equals,
        GreaterThanOrEqualTo = GreaterThan | Equals,
        Between = 0xFF
    }
}