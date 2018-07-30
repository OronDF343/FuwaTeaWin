using System.Collections.Generic;
using System.ComponentModel.Composition;
using FuwaTea.Audio.Files;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Decoders
{
    [InheritedExport]
    public interface ISubTrackEnumerator : IUpdateMetadata, ICanHandle<ISubTrackInfo, ISubTrackHandle>, ICanHandle<IFileHandle, IEnumerable<ISubTrackInfo>>
    {
    }
}
