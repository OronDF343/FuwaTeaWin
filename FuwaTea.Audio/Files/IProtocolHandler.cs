using System.ComponentModel.Composition;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Files
{
    [InheritedExport]
    public interface IProtocolHandler : ICanHandle<IFileLocationInfo, IFileHandle>
    {
    }
}
