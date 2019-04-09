using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using CSCore.DirectSound;
using Newtonsoft.Json;
using Sage.Extensibility.Config;

namespace Sage.Audio.Playback.CSCore
{
    [ConfigPage(nameof(DirectSound)), Export]
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