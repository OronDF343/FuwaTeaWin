using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CSCore.CoreAudioAPI;
using Newtonsoft.Json;
using Sage.Extensibility.Config;

namespace Sage.Audio.Playback.CSCore
{
    [ConfigPage(nameof(Wasapi))]
    public class WasapiConfig : CSCoreApiConfigBase
    {
        [JsonIgnore]
        public WasapiConfig Default => new WasapiConfig();

        [JsonIgnore]
        public IReadOnlyDictionary<string, string> WasapiDevices =>
            new ReadOnlyDictionary<string, string>(MMDeviceEnumerator
                                                   .EnumerateDevices(DataFlow.Render, DeviceState.Active)
                                                   .ToDictionary(mmd => mmd.DevicePath,
                                                                 mmd => mmd.FriendlyName));

        public string Device { get; set; } = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).DevicePath;

        public bool UseEventSync { get; set; } = false;
        public bool UseExclusiveMode { get; set; } = false;
        public int DesiredLatency { get; set; } = 100;
        public ThreadPriority PlaybackThreadPriority { get; set; } = ThreadPriority.AboveNormal;
        public bool UseChannelMixingMatrices { get; set; } = false;
    }
}