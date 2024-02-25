using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using CSCore.CoreAudioAPI;
using System.Text.Json.Serialization;
using Sage.Extensibility.Config;

namespace Sage.Audio.Playback.CSCore
{
    [ConfigPage(nameof(Wasapi)), Export]
    public class WasapiConfig : CSCoreApiConfigBase
    {
        [JsonIgnore]
        public WasapiConfig Default => new WasapiConfig();

        [JsonIgnore]
        public IReadOnlyDictionary<string, string> WasapiDevices =>
            new ReadOnlyDictionary<string, string>(MMDeviceEnumerator
                                                   .EnumerateDevices(DataFlow.Render, DeviceState.Active)
                                                   .ToDictionary(mmd => mmd.DeviceID,
                                                                 mmd => mmd.FriendlyName));

        public string Device { get; set; } = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).DeviceID;

        public bool UseEventSync { get; set; } = false;
        public bool UseExclusiveMode { get; set; } = false;
        public int DesiredLatency { get; set; } = 100;
        public ThreadPriority PlaybackThreadPriority { get; set; } = ThreadPriority.AboveNormal;
        public bool UseChannelMixingMatrices { get; set; } = false;
    }
}