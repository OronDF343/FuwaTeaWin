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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using DryIoc;
using FTWPlayer.Localization;
using FuwaTea.Audio.Decoders;
using FuwaTea.Audio.Playback;
using FuwaTea.Extensibility;
using FuwaTea.Lib.NotifyIcon;
using FuwaTea.Wpf.Keyboard;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;
using log4net;

namespace FTWPlayer.ViewModels
{
    public sealed class MainViewModel : DependencyObject, INotifyPropertyChanged, IDisposable // This is a DependencyObject so it supports animations
    {
        public IResolverContext MainViewModelScope { get; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            MainViewModelScope = ((App)Application.Current).MainContainer.OpenScope(nameof(MainViewModel));
            Settings = MainViewModelScope.Resolve<UISettings>();
            PlaybackManager = MainViewModelScope.Resolve<IPlaybackManager>();
            PlaylistManager = MainViewModelScope.Resolve<ISubTrackEnumerationManager>();
            NotifyIconManager = MainViewModelScope.Resolve<NotifyIconManager>();
            VolumeProperty = DependencyProperty.Register("Volume", typeof(decimal), typeof(MainViewModel),
                                                         new PropertyMetadata(1.0m, VolumePropertyChanged));

            _tmr = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.ApplicationIdle, Tick,
                Application.Current.Dispatcher);
            _tmr.Stop();
            PlaybackManager.PropertyChanged += (sender, args) =>
            {
                // TODO: shuffle
                /*if (args.PropertyName == nameof(IPlaybackManager.EnableShuffle))
                    RaisePropertyChanged(nameof(PositionTextFormatString));*/
                // TODO: CollectionChanged???
                if (args.PropertyName == nameof(IPlaybackManager.Index)/* || args.PropertyName == nameof(PlaybackManager.ElementCount)*/)
                    RaisePropertyChanged(nameof(OneBasedCurrentIndex));
                // TODO: shuffle
                /*if (args.PropertyName == nameof(IPlaybackManager.CurrentIndexAbsolute) || args.PropertyName == nameof(PlaybackManager.ElementCount))
                    RaisePropertyChanged(nameof(OneBasedCurrentIndexAbsolute));*/
                if (args.PropertyName == nameof(IPlaybackManager.Index))
                    RaisePropertyChanged(nameof(ScrollingTextFormatString)); // Testing: Scrolling Text
                switch (PlaybackManager.Player.State)
                {
                    case AudioPlayerState.Stopped:
                        _tmr.Stop();
                        // TODO
                        //PlaybackManager.SendPositionUpdate();
                        break;
                    case AudioPlayerState.Paused:
                        _tmr.Stop();
                        break;
                    case AudioPlayerState.Playing:
                        _tmr.Start();
                        break;
                }
            };

            // Create commands:
            PreviousCommand = new RelayCommand<RoutedEventArgs>(PreviousButtonClick);
            PlayPauseResumeCommand = new RelayCommand<RoutedEventArgs>(PlayPauseResumeButtonClick);
            NextCommand = new RelayCommand<RoutedEventArgs>(NextButtonClick);
            StopCommand = new RelayCommand<RoutedEventArgs>(StopButtonClick);
            MuteCommand = new RelayCommand<RoutedEventArgs>(MuteClick);
            VolumeClickCommand = new RelayCommand(VolumeClick);
            VolumeMouseLeaveCommand = new RelayCommand<Control>(VolumeMouseLeave);
            VolumeAddCommand = new RelayCommand<string>(VolumeAdd, VolumeChangeCanExecute);
            VolumeSubtractCommand = new RelayCommand<string>(VolumeSubtract, VolumeChangeCanExecute);
            ShuffleCommand = new RelayCommand<RoutedEventArgs>(ShuffleButtonClick);
            RepeatCommand = new RelayCommand<RoutedEventArgs>(RepeatButtonClick);
            ExpandCommand = new RelayCommand<RoutedEventArgs>(ExpandClick);
            KeyDownCommand = new RelayCommand<KeyEventArgs>(OnKeyDown);
            KeyUpCommand = new RelayCommand<KeyEventArgs>(OnKeyUp);
            ToolTipUpdateCommand = new RelayCommand<MouseEventArgs>(ToolTipUpdate);
            SeekCommand = new RelayCommand<Grid>(Seek);
            SeekAddCommand = new RelayCommand<string>(SeekAdd, SeekChangeCanExecute);
            SeekSubtractCommand = new RelayCommand<string>(SeekSubtract, SeekChangeCanExecute);
            MouseEnterSeekAreaCommand = new RelayCommand<MouseEventArgs>(MouseEnterSeekArea);
            MouseLeaveSeekAreaCommand = new RelayCommand<MouseEventArgs>(MouseLeaveSeekArea);
            DragEnterCommand = new RelayCommand<DragEventArgs>(DragEnter);
            DropCommand = new RelayCommand<DragEventArgs>(Drop);

            // Load tabs
            Tabs = new ObservableCollection<TabItem>(MainViewModelScope.ResolveMany<ITab>().OrderBy(t => t.Index).Select(t => t.TabObject));
            
            _settings = MainViewModelScope.Resolve<UISettings>();
            // TODO
            //Volume = _settings.RememberVolume ? _settings.LastVolume : PlaybackManager.Player.Volume;
            // TODO: EQ
            /*
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
            */

            // Create key commands
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = "CompactSeekCommand",
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Hold, KeyBindingKind.Toggle },
                Commands = new List<ICommand> { new RelayCommand(LeftShiftKeyDown), new RelayCommand(LeftShiftKeyUp) }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(PreviousCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal, KeyBindingKind.Repeat },
                Commands = new List<ICommand> { PreviousCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(PlayPauseResumeCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal },
                Commands = new List<ICommand> { PlayPauseResumeCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(NextCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal, KeyBindingKind.Repeat },
                Commands = new List<ICommand> { NextCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(StopCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal },
                Commands = new List<ICommand> { StopCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(MuteCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal, KeyBindingKind.Hold },
                Commands = new List<ICommand> { MuteCommand, MuteCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(VolumeAddCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal, KeyBindingKind.Repeat },
                Commands = new List<ICommand> { VolumeAddCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(VolumeSubtractCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal, KeyBindingKind.Repeat },
                Commands = new List<ICommand> { VolumeSubtractCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(ShuffleCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal },
                Commands = new List<ICommand> { ShuffleCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(RepeatCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal },
                Commands = new List<ICommand> { RepeatCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(ExpandCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal },
                Commands = new List<ICommand> { ExpandCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(SeekAddCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal, KeyBindingKind.Repeat },
                Commands = new List<ICommand> { SeekAddCommand }
            });
            KeyBindingsManager.Instance.KeyCommands.Add(new KeyCommand
            {
                Key = nameof(SeekSubtractCommand),
                SupportedKinds = new HashSet<KeyBindingKind> { KeyBindingKind.Normal, KeyBindingKind.Repeat },
                Commands = new List<ICommand> { SeekSubtractCommand }
            });

            // Begin post-UI-loading
            if (MiscUtils.ParseClArgs(Environment.GetCommandLineArgs().ToList()) == false)
                MessageBox.Show(Application.Current.MainWindow, "Unsupported file!", "LoadObject",
                                MessageBoxButton.OK, MessageBoxImage.Error);

            _settings.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(_settings.ScrollingTextFormat))
                    RaisePropertyChanged(nameof(ScrollingTextFormatString));
                if (e.PropertyName == nameof(_settings.TrayIconPreference))
                    if (!NotifyIconManager.SetPreference(Assembly.GetEntryAssembly().Location, (NOTIFYITEM_PREFERENCE)_settings.TrayIconPreference)) LogManager.GetLogger(GetType()).Warn("Failed to set NotifyIcon preference!");
            };
            // Run once: Set NotifyIcon to always visible
            // This setting purposely doesn't update to reflect the system setting
            if (_settings.TrayIconPreference > 2)
                _settings.TrayIconPreference = (uint)NOTIFYITEM_PREFERENCE.Always;
        }

        public UISettings Settings { get; }

        private readonly DispatcherTimer _tmr;
        private void Tick(object sender, EventArgs e)
        {
            // TODO
            //PlaybackManager.SendPositionUpdate();
        }

        public IPlaybackManager PlaybackManager { get; }
        public ISubTrackEnumerationManager PlaylistManager { get; }
        public NotifyIconManager NotifyIconManager { get; }

        public string AppName => Assembly.GetEntryAssembly().GetAttribute<AssemblyTitleAttribute>().Title;

        #region Commands

        public ICommand PreviousCommand { get; private set; }

        private void PreviousButtonClick(RoutedEventArgs e)
        {
            PlaybackManager.Index--;
        }

        public ICommand PlayPauseResumeCommand { get; private set; }

        private void PlayPauseResumeButtonClick(RoutedEventArgs e)
        {
            switch (PlaybackManager.Player.State)
            {
                case AudioPlayerState.Playing:
                    PlaybackManager.Player.Pause();
                    break;
                default:
                    PlaybackManager.Player.Play();
                    break;
            }
        }

        public ICommand NextCommand { get; private set; }

        private void NextButtonClick(RoutedEventArgs e)
        {
            PlaybackManager.Index++;
        }

        public ICommand StopCommand { get; private set; }

        private void StopButtonClick(RoutedEventArgs e)
        {
            PlaybackManager.Player.Stop();
        }

        public ICommand MuteCommand { get; private set; }

        private decimal _tempVolume;
        private void MuteClick(RoutedEventArgs e)
        {
            if (Volume == 0)
                Volume = _tempVolume > 0 ? _tempVolume : 1;
            else
            {
                _tempVolume = Volume;
                Volume = 0;
            }
        }

        public ICommand VolumeClickCommand { get; private set; }

        private void VolumeClick() { ShowVolumeSlider = true; }

        public ICommand VolumeMouseLeaveCommand { get; private set; }

        private void VolumeMouseLeave(Control e) { if (!e.IsMouseOver) ShowVolumeSlider = false; }

        public ICommand VolumeAddCommand { get; private set; }

        private void VolumeAdd(string s)
        {
            Volume += decimal.Parse(s);
        }

        public ICommand VolumeSubtractCommand { get; private set; }

        private void VolumeSubtract(string s)
        {
            Volume -= decimal.Parse(s);
        }

        private bool VolumeChangeCanExecute(string s)
        {
            return decimal.TryParse(s, out var d) && d > 0 && d < 1;
        }

        public ICommand ShuffleCommand { get; private set; }

        private void ShuffleButtonClick(RoutedEventArgs e)
        {
            // TODO: shuffle
            //PlaybackManager.EnableShuffle = !PlaybackManager.EnableShuffle;
        }

        public ICommand RepeatCommand { get; private set; }

        private void RepeatButtonClick(RoutedEventArgs e)
        {
            switch (PlaybackManager.Behavior)
            {
                case PlaybackBehavior.Normal:
                    PlaybackManager.Behavior = PlaybackBehavior.RepeatTrack;
                    break;
                case PlaybackBehavior.RepeatTrack:
                    PlaybackManager.Behavior = PlaybackBehavior.RepeatList;
                    break;
                default:
                    PlaybackManager.Behavior = PlaybackBehavior.Normal;
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
            if (e.Key != Key.LeftShift || _settings.EnableKeyboardHook) return;
            LeftShiftKeyDown();
        }

        private void LeftShiftKeyDown()
        {
            if (PlaybackManager.NowPlaying == null || Expanded || !Application.Current.MainWindow.IsVisible) return;
            ShiftMode = true;
            AllowDrag = false;
        }

        public ICommand KeyUpCommand { get; private set; }

        private void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key != Key.LeftShift || _settings.EnableKeyboardHook) return;
            LeftShiftKeyUp();
        }

        private void LeftShiftKeyUp()
        {
            if (Expanded) return;
            ShiftMode = false;
            AllowDrag = true;
        }

        public ICommand ToolTipUpdateCommand { get; private set; }

        private void ToolTipUpdate(MouseEventArgs e)
        {
            if (!ShiftMode) return;
            RaisePropertyChanged(nameof(MouseX));
            RaisePropertyChanged(nameof(MouseY));
        }

        public ICommand SeekCommand { get; private set; }

        private void Seek(Grid bar)
        {
            if (!ShiftMode) return;
            var p = NativeMethods.CorrectGetPosition(bar);
            PlaybackManager.Player.Position = TimeSpan.FromMilliseconds(PlaybackManager.Player.Duration.TotalMilliseconds / bar.ActualWidth * p.X);
        }

        private const string SeekTimeSpanFormat = @"hh\:mm\:ss\.FFF";
        private const string PercentOfTimeRegex = @"^[\d-[0]]\d?(?:\.\d+)?%$";

        public ICommand SeekAddCommand { get; private set; }

        private void SeekAdd(string time)
        {
            PlaybackManager.Player.Position += Regex.IsMatch(time, PercentOfTimeRegex)
                                            ? TimeSpan.FromMilliseconds(PlaybackManager.Player.Duration.TotalMilliseconds
                                                                        * (double.Parse(time.TrimEnd('%')) / 100))
                                            : TimeSpan.ParseExact(time, SeekTimeSpanFormat, null);
        }

        public ICommand SeekSubtractCommand { get; private set; }

        private void SeekSubtract(string time)
        {
            PlaybackManager.Player.Position -= Regex.IsMatch(time, PercentOfTimeRegex)
                                            ? TimeSpan.FromMilliseconds(PlaybackManager.Player.Duration.TotalMilliseconds
                                                                        * (double.Parse(time.TrimEnd('%')) / 100))
                                            : TimeSpan.ParseExact(time, SeekTimeSpanFormat, null);
        }

        private bool SeekChangeCanExecute(string time)
        {
            return Regex.IsMatch(time, PercentOfTimeRegex) || TimeSpan.TryParseExact(time, SeekTimeSpanFormat, null, out TimeSpan ts);
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
                if (!MiscUtils.LoadObject(s, true))
                    MessageBox.Show(Application.Current.MainWindow, "Unsupported file!", "LoadObject",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ICommand ShowCommand => new RelayCommand(Application.Current.MainWindow.Show);
        public ICommand HideCommand => new RelayCommand(Application.Current.MainWindow.Hide);
        public ICommand CloseCommand => new RelayCommand(Application.Current.MainWindow.Close);

        #endregion

        public ObservableCollection<TabItem> Tabs { get; private set; }

        private bool _showVolumeSlider;
        public bool ShowVolumeSlider { get => _showVolumeSlider; set { if (_showVolumeSlider == value) return; _showVolumeSlider = value; RaisePropertyChanged(); } }

        private bool _expanded;
        public bool Expanded { get => _expanded; set { _expanded = value; RaisePropertyChanged(); } }

        private bool _shiftMode;
        public bool ShiftMode { get => _shiftMode; set { _shiftMode = value; RaisePropertyChanged(); } }

        private bool _allowDrag = true;
        public bool AllowDrag { get => _allowDrag; set { _allowDrag = value; RaisePropertyChanged(); } }

        public Point CurrentMousePosition => NativeMethods.CorrectGetPosition();
        public double MouseX => CurrentMousePosition.X;
        public double MouseY => CurrentMousePosition.Y;

        public string ScrollingTextFormatString => PlaybackManager.NowPlaying != null
                                                       ? _settings.ScrollingTextFormat
                                                       : LocalizationProvider.GetLocalizedValue<string>("WelcomeText");

        // TODO: shuffle
        public string PositionTextFormatString => /*PlaybackManager.EnableShuffle ? "{0} ({1}) / {2} > {3} / {4}" : */"{0} / {2} > {3} / {4}";
        public int OneBasedCurrentIndex { get => PlaybackManager.List != null && PlaybackManager.List.Count > 0 ? PlaybackManager.Index + 1 : 0; set => PlaybackManager.Index = value - 1; }
        public int OneBasedCurrentIndexAbsolute { get => PlaybackManager.List != null && PlaybackManager.List.Count > 0 ? PlaybackManager.Index + 1 : 0; set => PlaybackManager.Index = value - 1; }

        #region Volume animation hax ++

        // This is a DependencyProperty so it can be animated

        public readonly DependencyProperty VolumeProperty;

        private void VolumePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // TODO
            //((MainViewModel)obj).PlaybackManager.Player.Volume = (decimal)args.NewValue;
            if (((MainViewModel)obj).UpdateRememberVolume) _settings.LastVolume = (decimal)args.NewValue;
        }

        public decimal Volume
        {
            get => (decimal)GetValue(VolumeProperty);
            set => SetValue(VolumeProperty, value);
        }

        public static readonly DependencyProperty UpdateRememberVolumeProperty = DependencyProperty.Register("UpdateRememberVolume", typeof(bool), typeof(MainViewModel), new PropertyMetadata(true));
        private UISettings _settings;

        public bool UpdateRememberVolume
        {
            get => (bool)GetValue(UpdateRememberVolumeProperty);
            set => SetValue(UpdateRememberVolumeProperty, value);
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            NotifyIconManager.Dispose();
            // IMPORTANT!
            PlaybackManager.Dispose();
        }
    }
}