using System.ComponentModel.Composition;
using Sage.Lib.Models;

namespace Sage.Audio.Files
{
    [InheritedExport]
    public interface IProtocolManager : IImplementationPriorityManager<IProtocolHandler, IFileLocationInfo, IFileHandle>
    {
    }
}
