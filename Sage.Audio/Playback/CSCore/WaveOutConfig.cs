using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CSCore.SoundOut;
using Newtonsoft.Json;
using Sage.Extensibility.Config;

namespace Sage.Audio.Playback.CSCore
{
    [ConfigPage(nameof(WaveOut))]
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