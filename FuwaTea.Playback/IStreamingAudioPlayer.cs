using System;
using System.ComponentModel.Composition;
using FuwaTea.Metadata.Tags;

namespace FuwaTea.Playback
{
    [InheritedExport]
    public interface IStreamingAudioPlayer : IAudioPlayer
    {
        event EventHandler StreamMetadataChanged;
        Tag StreamMetadata { get; }
    }
}
