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
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using FuwaTea.Lib;
using FuwaTea.Metadata.Tags;
using FuwaTea.Playback.NAudio.Utils;
using log4net;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace FuwaTea.Playback.NAudio
{
    // TODO [ConfigurableElement] see below!!!
    //[PlaybackElement("NAudio playback engine")]
    [Export(typeof(IAudioPlayer))]
    [Export(typeof(IStreamingAudioPlayer))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class NAudioPlayer : IAudioPlayer, IStreamingAudioPlayer
    {
        [ImportingConstructor]
        public NAudioPlayer([ImportMany] IEnumerable<ICodecProvider> codecs, [ImportMany] IEnumerable<IEffectProvider> effects)
        {
            _codecs = codecs.ToList();
            _effects = effects.ToList();
            //EqualizerBands = new ObservableCollection<EqualizerBand>(); :warning: DON'T DO THIS! Leave it set to null!
        }

        private readonly List<ICodecProvider> _codecs;
        private readonly List<IEffectProvider> _effects;

        #region IAudioPlayer implementation

        public void Dispose()
        {
            Unload();
            if (_wavePlayer == null) return;
            // TODO: double-check this
            _wavePlayer.Dispose();
            _wavePlayer = null;
        }

        #region Private fields

        private bool _stream;
        private ShoutcastStream _shoutcastStream;

        private IWavePlayer _wavePlayer;
        private ICodecProvider _currentCodec;
        private WaveStream _waveStream;
        private BalanceSampleProvider _balanceSampleProvider;
        private VolumeSampleProvider _volumeSampleProvider;
        private Equalizer _equalizer;
        #endregion

        public enum OutputApis
        {
            DirectSound,
            WaveOut,
            Wasapi,
            Asio
        }

        //[ConfigurableProperty(nameof(OutputApi), DefaultValue = OutputApis.Wasapi)]
        public OutputApis OutputApi { get; set; }

        //[ConfigurableProperty(nameof(DirectSoundDevice))]
        public Guid DirectSoundDevice { get; set; }

        //[PropertyOptionsEnumerator(nameof(DirectSoundDevice))]
        public Dictionary<Guid, string> DirectSoundDevices => DirectSoundOut.Devices.ToDictionary(d => d.Guid, d => d.Description);

        //[ConfigurableProperty(nameof(WasapiDevice))]
        public string WasapiDevice { get; set; }

        //[PropertyOptionsEnumerator(nameof(WasapiDevice))]
        public Dictionary<string, string> WasapiDevices
            => new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
                                       .ToDictionary(mmd => mmd.ID, mmd => mmd.FriendlyName);

        //[ConfigurableProperty(nameof(AsioDevice))]
        public string AsioDevice { get; set; }

        //[PropertyOptionsEnumerator(nameof(AsioDevice))]
        public string[] AsioDevices => AsioOut.GetDriverNames();

        //[ConfigurableProperty(nameof(WasapiExclusive))]
        public bool WasapiExclusive { get; set; }
        
        //[ConfigurableProperty(nameof(DesiredLatency), DefaultValue = -1)]
        public int DesiredLatency { get; set; }

        public void Load(string path)
        {
            // Check if we are loading a URL
            if (path.StartsWith("http://") || path.StartsWith("https://"))
            {
                _stream = true;
                // First load the stream
                _shoutcastStream = new ShoutcastStream(path);
                if (StreamMetadataChanged != null) _shoutcastStream.StreamTitleChanged += (sender, args) => StreamMetadataChanged(sender, args);
                if (_shoutcastStream.MimeType.ToLowerInvariant() != "audio/mpeg") throw new NotSupportedException("Only MP3 stream is supported!");
                // Init codec TODO: Create proper implementation, this is just for testing purposes
                _waveStream = new BufferedWaveStream(new Mp3FrameReader(new ReadFullyStream(_shoutcastStream)));
            }
            else
            {
                _stream = false;
                var cext = Path.GetExtension(path).ToLowerInvariant();
                // Find codec
                if (_currentCodec == null || !_currentCodec.GetExtensions().Contains(cext)) _currentCodec = _codecs.First(v => v.GetExtensions().Contains(cext));
                // Init codec
                _waveStream = _currentCodec.CreateWaveStream(path);
            }
            // Create player (see Unload() for why we need this every time)
            _wavePlayer?.Dispose();
            switch (OutputApi)
            {
                case OutputApis.DirectSound:
                    _wavePlayer = new DirectSoundOut(DirectSoundDevice, DesiredLatency > 0 ? DesiredLatency : 40);
                    break;
                case OutputApis.Wasapi:
                    var denum = new MMDeviceEnumerator();
                    var dev = string.IsNullOrWhiteSpace(WasapiDevice)
                                  ? denum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)
                                  : denum.GetDevice(WasapiDevice);
                    _wavePlayer = new WasapiOut(dev,
                                                WasapiExclusive
                                                    ? AudioClientShareMode.Exclusive
                                                    : AudioClientShareMode.Shared, true,
                                                DesiredLatency > 0 ? DesiredLatency : 200);
                    break;
                case OutputApis.WaveOut:
                    _wavePlayer = new WaveOut();
                    break;
                case OutputApis.Asio:
                    _wavePlayer = string.IsNullOrEmpty(AsioDevice) ? new AsioOut() : new AsioOut(AsioDevice);
                    break;
            }
            // Check if the WaveStream supports ISampleProvider and is IeeeFloat format. If so, cast it.
            // Otherwise, convert it to a sample provider
            var sp = (_currentCodec?.IsSampleProvider ?? false) &&
                     _waveStream.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat
                         ? _waveStream as ISampleProvider
                         : _waveStream.ToSampleProvider();
            // Balance only works for stereo, skip it if not stereo
            if (_waveStream.WaveFormat.Channels == 2)
            {
                // Create BalanceSampleProvider if stereo
                _balanceSampleProvider = new BalanceSampleProvider(sp);
                // Create VolumeSampleProvider
                _volumeSampleProvider = new VolumeSampleProvider(_balanceSampleProvider);
                // Restore volume settings
                _balanceSampleProvider.LeftVolume = (float)LeftVolume;
                _balanceSampleProvider.RightVolume = (float)RightVolume;
            }
            else _volumeSampleProvider = new VolumeSampleProvider(sp);
            // Create Equalizer
            _equalizer = new Equalizer(_volumeSampleProvider, EqualizerBands) { Enabled = EnableEqualizer };
            ISampleProvider lastSampleProvider = _equalizer;
            // Apply effects
            foreach (var effect in _effects)
            {
                try { lastSampleProvider = effect.ApplyEffect(lastSampleProvider); }
                catch (Exception e)
                {
                    /* TODO LogManager.GetLogger(GetType()).Error("Failed to load effect: "
                                                           + effect.GetType().GetAttribute<NAudioExtensionAttribute>().ElementName, e);*/
                }
            }
            // Init player
            _wavePlayer.Init(lastSampleProvider);
            _volumeSampleProvider.Volume = (float)Volume;
        }

        private void WavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception == null && PlaybackFinished != null) PlaybackFinished(this, new EventArgs());
            else LogManager.GetLogger(GetType()).Error("Playback has stopped due to an error!", e.Exception);
        }

        public void Unload()
        {
            Stop();
            _shoutcastStream?.Dispose();
            // BUG: The waveplayer needs to be disposed because it doesn't unload the previous stream properly. TODO: Report this to NAudio.
            _wavePlayer?.Dispose();
            _wavePlayer = null;
            if (_waveStream != null)
            {
                try { _waveStream.Dispose(); }
                catch (Exception e)
                {
                    LogManager.GetLogger(GetType()).Error("Failed to dispose WaveStream!", e);
                }
                _waveStream = null;
            }
            _balanceSampleProvider = null;
            _volumeSampleProvider = null;
            _equalizer = null;
            _stream = false;
        }

        public bool IsSomethingLoaded => _waveStream != null;
        public IEnumerable<string> SupportedFileTypes { get { return _codecs.SelectMany(c => c.SupportedFileTypes).Distinct(); } }

        public void Play()
        {
            _wavePlayer.PlaybackStopped += WavePlayer_PlaybackStopped;
            _wavePlayer.Play();
        }

        public void Pause()
        {
            _wavePlayer.PlaybackStopped -= WavePlayer_PlaybackStopped;
            _wavePlayer.Pause();
        }

        public void Resume()
        {
            _wavePlayer.PlaybackStopped += WavePlayer_PlaybackStopped;
            _wavePlayer.Play();
        }

        public void Stop()
        {
            if (_wavePlayer != null)
            {
                _wavePlayer.PlaybackStopped -= WavePlayer_PlaybackStopped;
                _wavePlayer.Stop();
            }
            if (!_stream) Position = TimeSpan.Zero;
        }

        public TimeSpan Duration => _waveStream?.TotalTime ?? TimeSpan.Zero;

        public TimeSpan Position
        {
            get { return _waveStream?.CurrentTime ?? TimeSpan.Zero; }
            set { if (_waveStream != null) _waveStream.CurrentTime = value; }
        }

        public bool CanResume => !_stream;
        public bool CanSeek => !_stream && _waveStream != null && _waveStream.CanSeek;

        public event EventHandler PlaybackFinished;

        private decimal _volume = 1.0m;
        public decimal Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                if (_volumeSampleProvider != null) _volumeSampleProvider.Volume = (float)_volume;
            }
        }
        private decimal _leftVolume = 1.0m;
        public decimal LeftVolume
        {
            get { return _leftVolume; }
            set
            {
                _leftVolume = value;
                if (_balanceSampleProvider != null) _balanceSampleProvider.LeftVolume = (float)_leftVolume;
            }
        }
        private decimal _rightVolume = 1.0m;
        public decimal RightVolume
        {
            get { return _rightVolume; }
            set
            {
                _rightVolume = value;
                if (_balanceSampleProvider != null) _balanceSampleProvider.RightVolume = (float)_rightVolume;
            }
        }

        public bool IsEqualizerSupported => true;
        private bool _enableEqualizer;
        public bool EnableEqualizer
        {
            get { return _enableEqualizer; }
            set
            {
                _enableEqualizer = value;
                if (_equalizer != null) _equalizer.Enabled = value;
            }
        }
        public ObservableCollection<EqualizerBand> EqualizerBands { get; set; }
        #endregion

        public event EventHandler StreamMetadataChanged;
        public Tag StreamMetadata => _shoutcastStream?.StreamMetadata;
    }
}
