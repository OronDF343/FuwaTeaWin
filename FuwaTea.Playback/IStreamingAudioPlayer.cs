using System;
using FuwaTea.Metadata;
using FuwaTea.Metadata.Tags;

namespace FuwaTea.Playback
{
    public interface IStreamingAudioPlayer : IAudioPlayer
    {
        event EventHandler StreamMetadataChanged;
        Tag StreamMetadata { get; }
    }
}
