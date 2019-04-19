using System.ComponentModel.Composition;
using CSCore;
using Sage.Audio.Files;
using Sage.Lib.Models;

namespace Sage.Audio.Decoders
{
    [InheritedExport]
    public interface IDecoderManager : IImplementationPriorityManager<ITrackDecoder, IFileHandle, ISampleSource>
    {
    }
}
