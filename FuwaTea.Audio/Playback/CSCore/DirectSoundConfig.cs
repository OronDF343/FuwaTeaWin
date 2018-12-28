using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CSCore.DirectSound;
using DryIocAttributes;
using FuwaTea.Config;
using Newtonsoft.Json;

namespace FuwaTea.Audio.Playback.CSCore
{
    [ConfigPage(nameof(DirectSound))]
    public class DirectSoundConfig : CSCoreApiConfigBase
    {
        [JsonIgnore]
        public DirectSoundConfig Default => new DirectSoundConfig();

        [JsonIgnore]
        public IReadOnlyDictionary<Guid, string> DirectSoundDevices =>
            new ReadOnlyDictionary<Guid, string>(DirectSoundDeviceEnumerator.EnumerateDevices().ToDictionary(d => d.Guid, d => d.Description));

        public Guid Device { get; set; } = DirectSoundDevice.DefaultPlaybackGuid;

        public int DesiredLatency { get; set; } = 100;
        public ThreadPriority PlaybackThreadPriority { get; set; } = ThreadPriority.AboveNormal;
    }
}