#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using FTWPlayer.Properties;
using FTWPlayer.Tabs;
using FuwaTea.Annotations;
using FuwaTea.Common.Models;
using FuwaTea.Logic.Playlist;
using FuwaTea.Presentation.Playback;
using GalaSoft.MvvmLight.CommandWpf;
using LayerFramework;
using WPFLocalizeExtension.Engine;
using FuwaTea.Wpf.Keyboard;

namespace FTWPlayer.ViewModel
{
    public sealed class MainViewModel : DependencyObject, INotifyPropertyChanged, IDisposable // This is a DependencyObject so it supports animations
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            LocalizeDictionary.Instance.Culture = string.IsNullOrWhiteSpace(Settings.Default.SelectedLanguage) ? CultureInfo.CurrentUICulture : CultureInfo.CreateSpecificCulture(Settings.Default.SelectedLanguage);
            Settings.Default.SelectedLanguage = LocalizeDictionary.Instance.Culture.IetfLanguageTag;

            PlaybackManager = LayerFactory.GetElement<IPlaybackManager>();

            _tmr = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.ApplicationIdle, Tick,
                Application.Current.Dispatcher);
            _tmr.Stop();
            PlaybackManager.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "EnableShuffle")
                    RaisePropertyChanged("PositionTextFormatString");
                if (args.PropertyName == "CurrentIndex" || args.PropertyName == "ElementCount")
                    RaisePropertyChanged("OneBasedCurrentIndex");
                if (args.PropertyName == "CurrentIndexAbsolute" || args.PropertyName == "ElementCount")
                    RaisePropertyChanged("OneBasedCurrentIndexAbsolute");
                if (args.PropertyName == "Current")
                {
                    RaisePropertyChanged("CurrentAlbumArt"); // Testing: Album Art Display
                    RaisePropertyChanged("ScrollingTextFormatString"); // Testing: Scrolling Text
                }
                if (args.PropertyName != "CurrentState") return;
                switch (PlaybackManager.CurrentState)
                {
                    case PlaybackState.Stopped:
                        _tmr.Stop();
                        PlaybackManager.SendPositionUpdate();
                        break;
                    case PlaybackState.Paused:
                        _tmr.Stop();
                        break;
                    case PlaybackState.Playing:
                        _tmr.Start();
                        break;
                }
            };

            // TODO: C#6 cleanup
            PreviousCommand = new RelayCommand<RoutedEventArgs>(PreviousButtonClick);
            PlayPauseResumeCommand = new RelayCommand<RoutedEventArgs>(PlayPauseResumeButtonClick);
            NextCommand = new RelayCommand<RoutedEventArgs>(NextButtonClick);
            StopCommand = new RelayCommand<RoutedEventArgs>(StopButtonClick);
            MuteClickCommand = new RelayCommand<RoutedEventArgs>(MuteClick);
            VolumeClickCommand = new RelayCommand(VolumeClick);
            VolumeMouseLeaveCommand = new RelayCommand<Control>(VolumeMouseLeave);
            ShuffleCommand = new RelayCommand<RoutedEventArgs>(ShuffleButtonClick);
            RepeatCommand = new RelayCommand<RoutedEventArgs>(RepeatButtonClick);
            ExpandCommand = new RelayCommand<RoutedEventArgs>(ExpandClick);
            KeyDownCommand = new RelayCommand<KeyEventArgs>(OnKeyDown);
            KeyUpCommand = new RelayCommand<KeyEventArgs>(OnKeyUp);
            ToolTipUpdateCommand = new RelayCommand<MouseEventArgs>(ToolTipUpdate);
            SeekCommand = new RelayCommand<Grid>(Seek);
            MouseEnterSeekAreaCommand = new RelayCommand<MouseEventArgs>(MouseEnterSeekArea);
            MouseLeaveSeekAreaCommand = new RelayCommand<MouseEventArgs>(MouseLeaveSeekArea);
            DragEnterCommand = new RelayCommand<DragEventArgs>(DragEnter);
            DropCommand = new RelayCommand<DragEventArgs>(Drop);

            // Load tabs
            Tabs = new ObservableCollection<TabItem>(LayerFactory.GetElements<ITab>().OrderBy(t => t.Index).Select(t => t.TabObject));

            // TODO: this is testing
            var plm = LayerFactory.GetElement<IPlaylistManager>(); // TODO: remove logic reference
            if (plm.SelectedPlaylist == null)
            {
                if (!plm.LoadedPlaylists.ContainsKey("temp")) plm.CreatePlaylist("temp");
                plm.SelectedPlaylistId = "temp";
            }
            Volume = Settings.Default.RememberVolume ? Settings.Default.LastVolume : PlaybackManager.Volume;
            // TODO: this is more testing
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 31, Gain = 0 });
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 62, Gain = 0 });
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 125, Gain = 0 });
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 250, Gain = 0 });
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 500, Gain = 0 });
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 1000, Gain = 0 });
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 2000, Gain = 0 });
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 4000, Gain = 0 });
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 8000, Gain = 0 });
            PlaybackManager.EqualizerBands.Add(new EqualizerBand { Bandwidth = 1f, Frequency = 16000, Gain = 0 });

            //Begin post-UI-loading
            MiscUtils.ParseClArgs(Environment.GetCommandLineArgs().ToList());
            _kbdListen = new KeyboardListener();
            _kbdListen.KeyDown += _kbdListen_KeyDown;
            Settings.Default.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName.Equals("EnableKeyboardHook", StringComparison.OrdinalIgnoreCase))
                    _kbdListen.IsEnabled = Settings.Default.EnableKeyboardHook;
            };
        }

        private void _kbdListen_KeyDown(object sender, RawKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.MediaPlayPause:
                    PlayPauseResumeButtonClick(new RoutedEventArgs());
                    break;
                case Key.MediaPreviousTrack:
                    PreviousButtonClick(new RoutedEventArgs());
                    break;
                case Key.MediaNextTrack:
                    NextButtonClick(new RoutedEventArgs());
                    break;
                case Key.MediaStop:
                    StopButtonClick(new RoutedEventArgs());
                    break;
            }
        }

        private readonly KeyboardListener _kbdListen;

        private readonly DispatcherTimer _tmr;
        private void Tick(object sender, EventArgs e)
        {
            PlaybackManager.SendPositionUpdate();
        }

        public IPlaybackManager PlaybackManager { get; private set; }

        #region Commands

        public ICommand PreviousCommand { get; private set; }

        private void PreviousButtonClick(RoutedEventArgs e)
        {
            PlaybackManager.Previous();
        }

        public ICommand PlayPauseResumeCommand { get; private set; }

        private void PlayPauseResumeButtonClick(RoutedEventArgs e)
        {
            PlaybackManager.PlayPauseResume();
        }

        public ICommand NextCommand { get; private set; }

        private void NextButtonClick(RoutedEventArgs e)
        {
            PlaybackManager.Next();
        }

        public ICommand StopCommand { get; private set; }

        private void StopButtonClick(RoutedEventArgs e)
        {
            PlaybackManager.Stop();
        }

        public ICommand MuteClickCommand { get; private set; }

        private decimal _tempVolume;
        private void MuteClick(RoutedEventArgs e)
        {
            if (PlaybackManager.Volume == 0)
                PlaybackManager.Volume = _tempVolume > 0 ? _tempVolume : 1;
            else
            {
                _tempVolume = PlaybackManager.Volume;
                PlaybackManager.Volume = 0;
            }
        }

        public ICommand VolumeClickCommand { get; private set; }

        private void VolumeClick() { ShowVolumeSlider = true; }

        public ICommand VolumeMouseLeaveCommand { get; private set; }

        private void VolumeMouseLeave(Control e) { if (!e.IsMouseOver) ShowVolumeSlider = false; }

        public ICommand ShuffleCommand { get; private set; }

        private void ShuffleButtonClick(RoutedEventArgs e)
        {
            PlaybackManager.EnableShuffle = !PlaybackManager.EnableShuffle;
        }

        public ICommand RepeatCommand { get; private set; }

        private void RepeatButtonClick(RoutedEventArgs e)
        {
            switch (PlaybackManager.LoopType)
            {
                case LoopTypes.None:
                    PlaybackManager.LoopType = LoopTypes.Single;
                    break;
                case LoopTypes.Single:
                    PlaybackManager.LoopType = LoopTypes.All;
                    break;
                default:
                    PlaybackManager.LoopType = LoopTypes.None;
                    break;
            }
        }

        public ICommand ExpandCommand { get; private set; }

        private void ExpandClick(RoutedEventArgs e)
        {
            Expanded = !Expanded;
            ShiftMode = Expanded;
        }

        public ICommand KeyDownCommand { get; private set; }

        private void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.LeftShift || !PlaybackManager.IsSomethingLoaded || Expanded) return;
            ShiftMode = true;
            AllowDrag = false;
        }

        public ICommand KeyUpCommand { get; private set; }

        private void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key != Key.LeftShift || Expanded) return;
            ShiftMode = false;
            AllowDrag = true;
        }

        public ICommand ToolTipUpdateCommand { get; private set; }

        private void ToolTipUpdate(MouseEventArgs e)
        {
            if (!ShiftMode) return;
            RaisePropertyChanged("MouseX");
            RaisePropertyChanged("MouseY");
        }

        public ICommand SeekCommand { get; set; }

        private void Seek(Grid bar)
        {
            var p = NativeMethods.CorrectGetPosition(bar);
            PlaybackManager.Position = TimeSpan.FromMilliseconds(PlaybackManager.Duration.TotalMilliseconds / bar.ActualWidth * p.X);
        }

        public ICommand MouseEnterSeekAreaCommand { get; private set; }

        private void MouseEnterSeekArea(MouseEventArgs e)
        {
            if (Expanded) AllowDrag = false;
        }

        public ICommand MouseLeaveSeekAreaCommand { get; private set; }

        private void MouseLeaveSeekArea(MouseEventArgs e)
        {
            if (Expanded) AllowDrag = true;
        }

        public ICommand DragEnterCommand { get; private set; }

        private void DragEnter(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effects = DragDropEffects.None;
        }

        public ICommand DropCommand { get; private set; }

        private void Drop(DragEventArgs e)
        {
            foreach (var s in (string[])e.Data.GetData(DataFormats.FileDrop, false))
            {
                MiscUtils.LoadObject(s, true);
            }
        }

        #endregion

        public ObservableCollection<TabItem> Tabs { get; private set; }

        private bool _showVolumeSlider;
        public bool ShowVolumeSlider { get { return _showVolumeSlider; } set { if (_showVolumeSlider == value) return; _showVolumeSlider = value; RaisePropertyChanged(); } }

        private bool _expanded;
        public bool Expanded { get { return _expanded; } set { _expanded = value; RaisePropertyChanged(); } }

        private bool _shiftMode;
        public bool ShiftMode { get { return _shiftMode; } set { _shiftMode = value; RaisePropertyChanged(); } }

        private bool _allowDrag = true;
        public bool AllowDrag { get { return _allowDrag; } set { _allowDrag = value; RaisePropertyChanged(); } }

        public Point CurrentMousePosition { get { return NativeMethods.CorrectGetPosition(); } }
        public double MouseX { get { return CurrentMousePosition.X; } }
        public double MouseY { get { return CurrentMousePosition.Y; } }

        public string ScrollingTextFormatString { get { return PlaybackManager.Current != null ? "{1} - {2}" : "{0}"; } }

        public string PositionTextFormatString { get { return PlaybackManager.EnableShuffle ? "{0} ({1}) / {2} > {3} / {4}" : "{0} / {2} > {3} / {4}"; } }
        public int OneBasedCurrentIndex { get { return PlaybackManager.ElementCount > 0 ? PlaybackManager.CurrentIndex + 1 : 0; } set { PlaybackManager.JumpTo(value - 1); } }
        public int OneBasedCurrentIndexAbsolute { get { return PlaybackManager.ElementCount > 0 ? PlaybackManager.CurrentIndexAbsolute + 1 : 0; } set { PlaybackManager.JumpToAbsolute(value - 1); } }

        #region Volume animation hax ++

        // This is a DependencyProperty so it can be animated

        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register("Volume", typeof(decimal), typeof(MainViewModel), new PropertyMetadata(1.0m, VolumePropertyChanged));

        private static void VolumePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((MainViewModel)obj).PlaybackManager.Volume = (decimal)args.NewValue;
            if (((MainViewModel)obj).UpdateRememberVolume) Settings.Default.LastVolume = (decimal)args.NewValue;
        }

        public decimal Volume
        {
            get { return ((decimal)(GetValue(VolumeProperty))); }
            set { SetValue(VolumeProperty, value); }
        }

        public static readonly DependencyProperty UpdateRememberVolumeProperty = DependencyProperty.Register("UpdateRememberVolume", typeof(bool), typeof(MainViewModel), new PropertyMetadata(true));

        public bool UpdateRememberVolume
        {
            get { return ((bool)(GetValue(UpdateRememberVolumeProperty))); }
            set { SetValue(UpdateRememberVolumeProperty, value); }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            //PlaybackManager.Dispose();
        }
    }
}