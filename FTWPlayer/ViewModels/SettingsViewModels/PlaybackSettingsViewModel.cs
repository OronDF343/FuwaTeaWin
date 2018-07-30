using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using DryIocAttributes;
using FTWPlayer.Views.SettingsViews;
using FuwaTea.Playback.NAudio;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    //[UIPart("Playback settings tab")]
    [Reuse(ReuseType.Singleton)]
    public class PlaybackSettingsViewModel : ISettingsTab
    {
        public PlaybackSettingsViewModel([Import] NAudioPlaybackSettings settings)
        {
            Settings = settings;
        }

        public TabItem GetTabItem()
        {
            // TODO: Clean up this mess
            DirectSoundDevices = Settings.DirectSoundDevices;
            WasapiDevices = Settings.WasapiDevices;
            WasapiDevices?.Add("", "Default Playback Device");
            AsioDevices = Settings.AsioDevices;
            return new PlaybackSettingsView(this);
        }
        public decimal Index => 0;
        public NAudioPlaybackSettings Settings { get; }
        public Dictionary<Guid, string> DirectSoundDevices { get; private set; }
        public Dictionary<string, string> WasapiDevices { get; private set; }
        public string[] AsioDevices { get; private set; }
    }
}
