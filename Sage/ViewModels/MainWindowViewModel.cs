using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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
using Serilog;

namespace Sage.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IResolverContext _container;
        private readonly IPlaybackManager _playMgr;
        private readonly VolumeEffect _volume;
        private readonly IEffectManager _effectManager;

        public MainWindowViewModel()
        {
            PreviousCommand = new RelayCommand(Previous);
            PlayCommand = new RelayCommand(Play);
            StopCommand = new RelayCommand(Stop);
            NextCommand = new RelayCommand(Next);
            ShuffleCommand = new RelayCommand(Shuffle);
            RepeatCommand = new RelayCommand(Repeat);
            MinimizeCommand = new RelayCommand(Minimize);
            ExitCommand = new RelayCommand(Exit);

            if (Design.IsDesignMode) return;

            _container = Program.AppInstance.ExtensibilityContainer.OpenScope(nameof(MainWindowViewModel));

            // Configure for testing
            var apiSel = _container.Resolve<ApiSelector>();
            apiSel.SelectImplementation(apiSel.Implementations.First(i => i.GetType().Name.Contains("Wasapi")));
            var wCfg = _container.Resolve<WasapiConfig>();
            wCfg.MasterVolume = 1.0f;
            // Load
            _playMgr = _container.Resolve<IPlaybackManager>();
            _volume = _container.Resolve<VolumeEffect>();
            _volume.Volume = 0.7f;
            _effectManager = _container.Resolve<IEffectManager>();
            _effectManager.AppendImplementation(_volume);
            _playMgr.List = new ObservableCollection<IFileHandle>();
            if (Program.AppInstance.Args.ContainsKey(AppConstants.Arguments.Files))
                foreach (var file in Program.AppInstance.Args[AppConstants.Arguments.Files])
                    AddFile(file);
            // Required:
            _playMgr.Player.StateChanged += Player_StateChanged;
            // Auto-play
            if (!Program.AppInstance.Args.ContainsKey(AppConstants.Arguments.AddOnly))
                Play();

            // Drag/Drop
            DropHandler = new FileDropHandler(AddFile);
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

        private void Player_StateChanged(object sender, AudioPlayerStateChangedEventArgs args)
        {
            this.RaisePropertyChanged(nameof(IsPlaying));
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
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Exit()
        {
            Application.Current.Exit();
        }

        public string Message => "Sage: Music Player (Development Version)";
        public double MinScrollingMargin => 64;
        public double ScrollingVelocity => 50;
        public TimeSpan Duration => TimeSpan.FromSeconds(3);

        public bool IsPlaying => _playMgr.Player.State == AudioPlayerState.Playing;
        public bool IsShuffleEnabled { get; private set; } // TODO: Implement shuffle

        public bool? RepeatMode => _playMgr.Behavior == PlaybackBehavior.Normal ? false :
                                   _playMgr.Behavior == PlaybackBehavior.RepeatList ? true : (bool?)null;

        public ICommand PreviousCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand ShuffleCommand { get; }
        public ICommand RepeatCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand ExitCommand { get; }

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
                if (!e.Data.Contains(DataFormats.FileNames) || !e.Data.Contains(DataFormats.Text))
                    e.DragEffects = DragDropEffects.None;

                e.Handled = true;
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
