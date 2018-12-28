using System;
using System.Collections.Generic;
using System.Text;

namespace FuwaTea.Core
{
    public interface IPlatformSupport
    {
        bool IsInstalled { get; }

    }
}
