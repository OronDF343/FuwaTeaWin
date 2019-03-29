using System.ComponentModel.Composition;
using CSCore;
using FuwaTea.Lib.DataModel;
using Sage.Audio.Files;

namespace Sage.Audio.Decoders
{
    [InheritedExport]
    public interface ITrackDecoder : IUpdateMetadata, ICanHandle<IFileHandle, ISampleSource>
    {
    }
}
