using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using CSCore.SoundOut;
using DryIocAttributes;
using FuwaTea.Config;
using Newtonsoft.Json;

namespace FuwaTea.Audio.Playback.CSCore
{
    [ConfigPage(nameof(WaveOut)), Export, Reuse(ReuseType.Singleton)]
    public class WaveOutConfig : CSCoreApiConfigBase
    {
        [JsonIgnore]
        public WaveOutConfig Default => new WaveOutConfig();
        
        [JsonIgnore]
        public IReadOnlyDictionary<int, string> DirectSoundDevices =>
            new ReadOnlyDictionary<int, string>(WaveOutDevice.EnumerateDevices().ToDictionary(d => d.DeviceId, d => d.Name));

        public int Device { get; set; } = WaveOutDevice.DefaultDevice.DeviceId;

        public int DesiredLatency { get; set; } = 100;
        public bool UseChannelMixingMatrices { get; set; } = false;
    }
}