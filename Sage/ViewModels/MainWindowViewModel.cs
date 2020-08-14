using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using DryIoc;
using ReactiveUI;
using Sage.Audio.Effects;
using Sage.Audio.Effects.Impl;
using Sage.Audio.Files;
using Sage.Audio.Files.Impl;
using Sage.Audio.Playback;
using Sage.Audio.Playback.CSCore;
using Sage.Core;
using Sage.Helpers;
using Sage.Lib.Collections;
using Sage.Views;
using Serilog;

namespace Sage.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IResolverContext _container;
        private readonly IPlaybackManager _playMgr;
        private readonly VolumeEffect _volume;
        private readonly IEffectManager _effectManager;
        private readonly DispatcherTimer _progressTimer;

        public MainWindowViewModel()
        {
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                /* handle activation */
                Disposable
                    .Create(() => { /* handle deactivation */ })
                    .DisposeWith(disposables);
            });

            PreviousCommand = new RelayCommand(Previous);
            PlayCommand = new RelayCommand(Play);
            StopCommand = new RelayCommand(Stop);
            NextCommand = new RelayCommand(Next);
            ShuffleCommand = new RelayCommand(Shuffle);
            RepeatCommand = new RelayCommand(Repeat);
            MinimizeCommand = new RelayCommand(Minimize);
            ExitCommand = new RelayCommand(Exit);
            SeekCommand = new RelayCommand<PointerReleasedEventArgs>(Seek);

            if (Design.IsDesignMode) return;

            _container = Program.AppInstance.ExtensibilityContainer.OpenScope(nameof(MainWindowViewModel));

            // Progress
            _progressTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.ApplicationIdle, ProgressTick);
            _progressTimer.Stop();

            // Configure for testing
            var apiSel = _container.Resolve<ApiSelector>();
            apiSel.SelectImplementation(apiSel.Implementations.First(i => i.GetType().Name.Contains("Wasapi")));
            var wCfg = _container.Resolve<WasapiConfig>();
            wCfg.MasterVolume = 1.0f;
            // Load
            _playMgr = _container.Resolve<IPlaybackManager>();
            _playMgr.Player.StateChanging += Player_StateChanging;
            _volume = _container.Resolve<VolumeEffect>();
            _volume.Volume = 0.4f;
            _effectManager = _container.Resolve<IEffectManager>();
            _effectManager.AppendImplementation(_volume);
            _playMgr.List = new ObservableCollection<IFileHandle>();
            if (Program.AppInstance.Args.ContainsKey(AppConstants.Arguments.Files))
                foreach (var file in Program.AppInstance.Args[AppConstants.Arguments.Files])
                    AddFile(file);
            // Auto-play
            if (!Program.AppInstance.Args.ContainsKey(AppConstants.Arguments.AddOnly))
                Play();

            // Drag/Drop
            DropHandler = new FileDropHandler(AddFile);
        }

        private void ProgressTick(object sender, EventArgs args)
        {
            this.RaisePropertyChanged(nameof(PercentProgress));
        }

        private void AddFile(string file)
        {
            try { _playMgr.List.Add(new StandardFileHandle(new FileLocationInfo(new Uri(file)))); }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to load file \"{file}\"");
                // TODO: Notify user
            }
        }

        private void Player_StateChanging(object sender, AudioPlayerStateChangedEventArgs args)
        {
            this.RaisePropertyChanged(nameof(IsPlaying));
            Log.Debug(args.NewState.ToString());
            if (args.NewState == AudioPlayerState.Stopped)
            {
                _progressTimer.Stop();
                this.RaisePropertyChanged(nameof(PercentProgress));
            }
            else if (!_progressTimer.IsEnabled)
            {
                _progressTimer.Start();
            }

            if (args.NewState == AudioPlayerState.Loaded)
            {
                if (_playMgr.NowPlaying.Metadata.TryGetValue(MetadataSource.Decoder, out var decMeta))
                {
                    Message = decMeta.Artist + " - " + decMeta.Title;
                }
            }
        }

        private void Previous()
        {
            _playMgr.Index = (_playMgr.Index - 1).WithCondition(i => i < 0, _playMgr.List.Count - 1);
        }

        private void Play()
        {
            if (_playMgr.Player.State == AudioPlayerState.Playing) _playMgr.Player.Pause();
            else _playMgr.Player.Play();
        }

        private void Stop()
        {
            _playMgr.Player.Stop();
        }

        private void Next()
        {
            _playMgr.Index = (_playMgr.Index + 1).WithCondition(i => i >= _playMgr.List.Count, 0);
        }

        private void Shuffle()
        {
            IsShuffleEnabled = !IsShuffleEnabled;
            this.RaisePropertyChanged(nameof(IsShuffleEnabled));
        }

        private void Repeat()
        {
            _playMgr.Behavior = (PlaybackBehavior)(((int)_playMgr.Behavior + 1) % 3);
            this.RaisePropertyChanged(nameof(RepeatMode));
        }

        private void Minimize()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Exit()
        {
            if (Application.Current.ApplicationLifetime is IControlledApplicationLifetime ctl)
                ctl.Shutdown();
        }

        public string Message { get; set; } = "Sage: Music Player (Development Version)";
        public double MinScrollingMargin => 64;
        public double ScrollingVelocity => 50;

        public bool IsPlaying => _playMgr.Player.State == AudioPlayerState.Playing;
        public bool IsShuffleEnabled { get; private set; } // TODO: Implement shuffle

        public bool? RepeatMode => _playMgr.Behavior == PlaybackBehavior.Normal ? false :
                                   _playMgr.Behavior == PlaybackBehavior.RepeatList ? true : (bool?)null;

        public TimeSpan Position => _playMgr.Player.Position;
        public double PercentProgress => Position / Duration;
        public TimeSpan Duration => _playMgr.Player.Duration;

        public ICommand PreviousCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand ShuffleCommand { get; }
        public ICommand RepeatCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand ExitCommand { get; }
        
        public ICommand SeekCommand { get; }

        private void Seek(PointerReleasedEventArgs e)
        {
            var src = (Control)e.Source;
            var pb = src.Parent;
            var x = e.GetCurrentPoint(pb).Position.X;
            var w = pb.Bounds.Width;
            _playMgr.Player.Position = Duration * (x / w);
            this.RaisePropertyChanged(nameof(PercentProgress));
        }

        public IDropHandler DropHandler { get; }

        private class FileDropHandler : IDropHandler
        {
            public FileDropHandler(Action<string> onDrop)
            {
                OnDrop = onDrop;
            }

            public Action<string> OnDrop { get; }

            public void Enter(object sender, DragEventArgs e)
            {

            }

            public void Leave(object sender, RoutedEventArgs e)
            {

            }

            public void Over(object sender, DragEventArgs e)
            {
                // Only allow Copy or Link as Drop Operations.
                e.DragEffects |= DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;

                // Only allow if the dragged data contains text or filenames.
                if (!e.Data.Contains(DataFormats.FileNames) && !e.Data.Contains(DataFormats.Text))
                    e.DragEffects = DragDropEffects.None;

                //e.Handled = true;
            }

            public void Drop(object sender, DragEventArgs e)
            {
                if (e.Data.Contains(DataFormats.FileNames))
                    foreach (var fileName in e.Data.GetFileNames())
                        OnDrop?.Invoke(fileName);
                e.Handled = true;
            }
        }
    }
}
