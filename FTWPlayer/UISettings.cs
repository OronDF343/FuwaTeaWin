using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FuwaTea.Extensibility.Config;
using FuwaTea.Wpf.Helpers;
using FuwaTea.Wpf.Keyboard;
using JetBrains.Annotations;
using Newtonsoft.Json;
using KeyBinding = FuwaTea.Wpf.Keyboard.KeyBinding;

namespace FTWPlayer
{
    // TODO IMPORTANT: Reset setting to default!
    [ConfigPage(nameof(UISettings))]
    public class UISettings : IConfigPage
    {
        [JsonIgnore]
        public UISettings Default => new UISettings();

        public ProcessPriorityClass ProcessPriority { get; set; } = ProcessPriorityClass.AboveNormal;
        public TimeSpan InstanceCreationTimeout { get; set; } = TimeSpan.FromSeconds(20);
        public bool RememberVolume { get; set; } = true;
        public decimal LastVolume { get; set; } = 1m;
        public bool VolumeFadeOutOnExit { get; set; } = true;
        public double MaxEqualizerGain { get; set; } = 10.0;
        public bool ShowToolTips { get; set; } = true;
        public bool OverrideDefaultLanguage { get; set; } = false;
        public string SelectedLanguage { get; set; }
        public bool ShowInTaskbar { get; set; } = false;
        public bool Topmost { get; set; } = true;
        public string LastVersion { get; set; }
        public bool EnableKeyboardHook { get; set; } = true;
        public string Skin { get; set; } = "pack://application:,,,/Skins/Glacier";
        public double DesiredMaxHeight { get; set; } = 540.0;
        public double DesiredWidth { get; set; } = 540.0;
        public WindowPositions AutoWindowPosition { get; set; } = WindowPositions.BottomRight;
        public string ScrollingTextFormat { get; set; } = "{Tag.JoinedPerformers>Tag.JoinedAlbumArtists>\"Unknown Performer\"} - {Tag.Title>FileName>\"Unknown Title\"}";
        public uint TrayIconPreference { get; set; } = 3;

        // TODO: To list
        public ObservableCollection<KeyBinding> KeyBindings { get; set; } = new ObservableCollection<KeyBinding>
        {
            new KeyBinding
            {
                Name = "Hold shift to seek",
                Enabled = true,
                CommandKey = "CompactSeekCommand",
                Kind = KeyBindingKind.Hold,
                KeyGesture = { Key.LeftShift }
            },
            new KeyBinding
            {
                Name = "Previous",
                Enabled = true,
                CommandKey = "PreviousCommand",
                Kind = KeyBindingKind.Normal,
                KeyGesture = { Key.MediaPreviousTrack }
            },
            new KeyBinding
            {
                Name = "Play / Pause / Resume",
                Enabled = true,
                CommandKey = "PlayPauseResumeCommand",
                Kind = KeyBindingKind.Normal,
                KeyGesture = { Key.MediaPlayPause }
            },
            new KeyBinding
            {
                Name = "Next",
                Enabled = true,
                CommandKey = "NextCommand",
                Kind = KeyBindingKind.Normal,
                KeyGesture = { Key.MediaNextTrack }
            },
            new KeyBinding
            {
                Name = "Stop",
                Enabled = true,
                CommandKey = "StopCommand",
                Kind = KeyBindingKind.Normal,
                KeyGesture = { Key.MediaStop }
            }
        };

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
