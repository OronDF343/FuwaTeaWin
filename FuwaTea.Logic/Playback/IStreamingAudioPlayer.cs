using System;
using FuwaTea.Common.Models;

namespace FuwaTea.Logic.Playback
{
    public interface IStreamingAudioPlayer : IAudioPlayer
    {
        event EventHandler StreamMetadataChanged;
        Tag StreamMetadata { get; }
    }
}
