using System;
using System.ComponentModel.Composition;

namespace Sage.Core.Platform.Windows
{
    [Export(typeof(IPlatformSupport))]
    public class WindowsPlatformSupport : IPlatformSupport
    {
    }
}
