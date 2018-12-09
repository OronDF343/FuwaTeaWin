using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using DryIocAttributes;
using FTWPlayer.Views.SettingsViews;
using FuwaTea.Audio.Playback.CSCore;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    //[UIPart("Playback settings tab")]
    [Reuse(ReuseType.Singleton)]
    public class PlaybackSettingsViewModel : ISettingsTab
    {
        public PlaybackSettingsViewModel(/*[Import] CSCoreApiConfigBase settings*/)
        {
            //Settings = settings;
        }

        public TabItem GetTabItem()
        {
            // TODO: Reimplement this
            /*DirectSoundDevices = Settings.DirectSoundDevices;
            WasapiDevices = Settings.WasapiDevices;
            WasapiDevices?.Add("", "Default Playback Device");
            AsioDevices = Settings.AsioDevices;*/
            return new PlaybackSettingsView(this);
        }
        public decimal Index => 0;
        public CSCoreApiConfigBase Settings { get; }
        public Dictionary<Guid, string> DirectSoundDevices { get; private set; }
        public Dictionary<string, string> WasapiDevices { get; private set; }
        public string[] AsioDevices { get; private set; }
    }
}
