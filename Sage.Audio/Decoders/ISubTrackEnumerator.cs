using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sage.Audio.Files;
using Sage.Lib.Models;

namespace Sage.Audio.Decoders
{
    [InheritedExport]
    public interface ISubTrackEnumerator : IUpdateMetadata, ICanHandle<ISubTrackInfo, ISubTrackHandle>, ICanHandle<IFileHandle, IEnumerable<ISubTrackInfo>>
    {
    }
}
