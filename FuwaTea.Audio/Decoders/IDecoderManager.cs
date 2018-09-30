using System.ComponentModel.Composition;
using CSCore;
using FuwaTea.Audio.Files;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Decoders
{
    [InheritedExport]
    public interface IDecoderManager : IImplementationPriorityManager<ITrackDecoder, IFileHandle, ISampleSource>
    {
    }
}
