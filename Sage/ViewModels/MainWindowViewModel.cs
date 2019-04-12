using System;
using System.Linq;
using System.Windows.Input;
using CSCore;
using DryIoc;
using Sage.Audio.Decoders;
using Sage.Audio.Files;
using Sage.Audio.Files.Impl;
using Sage.Audio.Playback;
using Sage.Audio.Playback.CSCore;
using Sage.Helpers;

namespace Sage.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IResolverContext _container;
        private IAudioPlayer _player;
        private IDecoderManager _decoderMgr;
        private StandardFileHandle _file;

        public MainWindowViewModel()
        {
            PlayCommand = new RelayCommand(Play);
            StopCommand = new RelayCommand(Stop);

            _container = Program.AppInstance.ExtensibilityContainer.OpenScope(nameof(MainWindowViewModel));
            _player = _container.Resolve<IAudioPlayer>();
            _decoderMgr = _container.Resolve<IDecoderManager>();
            _file = new StandardFileHandle(new FileLocationInfo(new Uri(@"D:\01. Dangerous Sunshine.wv")));
            var ss = _decoderMgr.Handle(_file);
            var apiSel = _container.Resolve<ApiSelector>();
            apiSel.SelectImplementation(apiSel.Implementations.First(i => i.GetType().Name.Contains("Wasapi")));
            var wCfg = _container.Resolve<WasapiConfig>();
            wCfg.MasterVolume = 0.7f;
            _player.Load(ss.ToWaveSource());
        }

        private void Play()
        {
            if (_player.State == AudioPlayerState.Playing) _player.Pause();
            else _player.Play();
        }

        private void Stop()
        {
            _player.Stop();
        }

        public string Message => "Hello World!";
        public double MinScrollingMargin => 50;
        public double ScrollingVelocity => 50;
        public TimeSpan Duration => TimeSpan.FromSeconds(3);

        public ICommand PlayCommand { get; }
        public ICommand StopCommand { get; }
    }
}
