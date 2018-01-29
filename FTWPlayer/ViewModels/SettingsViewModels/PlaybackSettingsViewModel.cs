using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;
using DryIocAttributes;
using FTWPlayer.Views.SettingsViews;
using FuwaTea.Extensibility.ConfigurationTemp;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    //[UIPart("Playback settings tab")]
    [Reuse(ReuseType.Singleton)]
    public class PlaybackSettingsViewModel : ISettingsTab
    {
        public TabItem GetTabItem(ApplicationSettingsBase settings, List<IConfigurablePropertyInfo> dynSettings)
        {
            Settings = settings;
            var et = settings["OutputApi"].GetType();
            OutputApis = Enum.GetValues(et).Cast<Enum>().ToDictionary(v => v, v => Enum.GetName(et, v));
            DirectSoundDevices = (Dictionary<Guid, string>)dynSettings.FirstOrDefault(d => d.Name == "DirectSoundDevice")?.ValidValues;
            WasapiDevices = (Dictionary<string, string>)dynSettings.FirstOrDefault(d => d.Name == "WasapiDevice")?.ValidValues;
            WasapiDevices?.Add("", "Default Playback Device");
            AsioDevices = (string[])dynSettings.FirstOrDefault(d => d.Name == "AsioDevice")?.ValidValues;
            return new PlaybackSettingsView(this);
        }
        public decimal Index => 0;
        public ApplicationSettingsBase Settings { get; private set; }
        public Dictionary<Enum, string> OutputApis { get; private set; }
        public Dictionary<Guid, string> DirectSoundDevices { get; private set; }
        public Dictionary<string, string> WasapiDevices { get; private set; }
        public string[] AsioDevices { get; private set; }
    }
}
