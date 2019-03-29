using System.ComponentModel.Composition;
using FuwaTea.Lib.DataModel;

namespace Sage.Audio.Files
{
    [InheritedExport]
    public interface IProtocolHandler : ICanHandle<IFileLocationInfo, IFileHandle>
    {
    }
}
