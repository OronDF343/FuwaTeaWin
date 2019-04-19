using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sage.Audio.Files;
using Sage.Lib.Models;

namespace Sage.Audio.Decoders
{
    [InheritedExport]
    public interface ISubTrackEnumerationManager : IImplementationPriorityManager<ISubTrackEnumerator, IFileHandle, IEnumerable<ISubTrackInfo>>
    {
    }
}
