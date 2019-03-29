using System.ComponentModel.Composition;
using FuwaTea.Lib.DataModel;

namespace Sage.Audio.Files
{
    [InheritedExport]
    public interface IProtocolManager : IImplementationPriorityManager<IProtocolHandler, IFileLocationInfo, IFileHandle>
    {
    }
}
