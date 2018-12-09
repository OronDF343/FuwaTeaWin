using System.Collections.Generic;
using System.ComponentModel.Composition;
using FuwaTea.Audio.Files;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Decoders
{
    [InheritedExport]
    public interface ISubTrackEnumerationManager : IImplementationPriorityManager<ISubTrackEnumerator, IFileHandle, IEnumerable<ISubTrackInfo>>
    {
    }
}
