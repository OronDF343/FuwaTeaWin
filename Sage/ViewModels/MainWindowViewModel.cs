using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using DryIoc;
using Sage.Audio.Files;
using Sage.Audio.Files.Impl;
using Sage.Audio.Playback;
using Sage.Audio.Playback.CSCore;
using Sage.Helpers;

namespace Sage.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IResolverContext _container;
        private readonly IPlaybackManager _playMgr;
        private readonly StandardFileHandle _file;

        public MainWindowViewModel()
        {
            PlayCommand = new RelayCommand(Play);
            StopCommand = new RelayCommand(Stop);
            ExitCommand = new RelayCommand(Exit);
            
            if (Design.IsDesignMode) return;
            
            _container = Program.AppInstance.ExtensibilityContainer.OpenScope(nameof(MainWindowViewModel));
            // Configure
            var apiSel = _container.Resolve<ApiSelector>();
            apiSel.SelectImplementation(apiSel.Implementations.First(i => i.GetType().Name.Contains("Wasapi")));
            var wCfg = _container.Resolve<WasapiConfig>();
            wCfg.MasterVolume = 0.7f;
            // Load
            _playMgr = _container.Resolve<IPlaybackManager>();
            _file = new StandardFileHandle(new FileLocationInfo(new Uri(@"D:\01. Dangerous Sunshine.wv")));
            _playMgr.List = new ObservableCollection<IFileHandle> { _file };
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

        private void Exit()
        {
            Application.Current.Exit();
        }

        public string Message => "Hello World!";
        public double MinScrollingMargin => 50;
        public double ScrollingVelocity => 50;
        public TimeSpan Duration => TimeSpan.FromSeconds(3);

        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ExitCommand { get; }
    }
}
