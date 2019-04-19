using System.ComponentModel.Composition;
using Sage.Lib.Models;

namespace Sage.Audio.Files
{
    [InheritedExport]
    public interface IProtocolHandler : ICanHandle<IFileLocationInfo, IFileHandle>
    {
    }
}
