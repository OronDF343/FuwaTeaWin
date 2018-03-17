using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using DryIocAttributes;
using FuwaTea.Config;
using JetBrains.Annotations;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Newtonsoft.Json;

namespace FuwaTea.Playback.NAudio
{
    [ConfigPage(nameof(NAudioPlaybackSettings)), Export]
    [Reuse(ReuseType.Singleton)]
    public class NAudioPlaybackSettings : IConfigPage
    {
        [JsonIgnore]
        public NAudioPlaybackSettings Default => new NAudioPlaybackSettings();

        public OutputApis OutputApi { get; set; } = OutputApis.Wasapi;

        public Guid DirectSoundDevice { get; set; } = Guid.Empty;

        [JsonIgnore]
        public Dictionary<Guid, string> DirectSoundDevices => DirectSoundOut.Devices.ToDictionary(d => d.Guid, d => d.Description);

        public string WasapiDevice { get; set; } = "";

        [JsonIgnore]
        public Dictionary<string, string> WasapiDevices => new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                                                                                  .ToDictionary(mmd => mmd.ID, mmd => mmd.FriendlyName);

        public string AsioDevice { get; set; } = "";

        [JsonIgnore]
        public string[] AsioDevices => AsioOut.GetDriverNames();

        public bool WasapiExclusive { get; set; }
        
        public int DesiredLatency { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
