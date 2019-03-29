using System.Collections.Generic;
using System.ComponentModel.Composition;
using FuwaTea.Lib.DataModel;
using Sage.Audio.Files;

namespace Sage.Audio.Decoders
{
    [InheritedExport]
    public interface ISubTrackEnumerationManager : IImplementationPriorityManager<ISubTrackEnumerator, IFileHandle, IEnumerable<ISubTrackInfo>>
    {
    }
}
