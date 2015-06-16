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
using System.IO;
using System.Linq;
using System.Threading;
using FuwaTea.Common.Models;
using FuwaTea.Data.Playback.NAudio;
using LayerFramework;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace FuwaTea.Logic.Playback.NAudio
{
    [LogicElement("NAudio playback engine")]
    public sealed class NAudioPlayer : IAudioPlayer
    {
        public NAudioPlayer()
        {
            _codecs = LayerFactory.GetElements<IWaveStreamProvider>().ToList();
            //EqualizerBands = new ObservableCollection<EqualizerBand>(); DON'T DO THIS! Leave it set to null!
        }

        private readonly List<IWaveStreamProvider> _codecs;

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

        private IWavePlayer _wavePlayer;
        private Guid _lastDevice;
        private IWaveStreamProvider _currentCodec;
        private WaveStream _waveStream;
        private BalanceSampleProvider _balanceSampleProvider;
        private VolumeSampleProvider _volumeSampleProvider;
        private Equalizer _equalizer;
        private volatile bool _userStopped = true;
        #endregion

        public void Load(string path, IAudioOutputDevice device)
        {
            var cext = Path.GetExtension(path);
            // Find codec
            if (_currentCodec == null || !_currentCodec.SupportedFileTypes.Contains(cext))
                _currentCodec = _codecs.First(v => v.SupportedFileTypes.Contains(cext));
            // Create player if needed
            if (_lastDevice != device.AsGuid() || _wavePlayer == null)
            {
                if (_wavePlayer != null) _wavePlayer.Dispose();
                _wavePlayer = new DirectSoundOut(_lastDevice = device.AsGuid());
            }
            // Init codec
            _waveStream = _currentCodec.CreateWaveStream(path);
            // Check if the WaveStream supports ISampleProvider and is IeeeFloat format. If so, cast it.
            var sp = _currentCodec.IsSampleProvider &&
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
            // Init player
            _wavePlayer.Init(_equalizer);
            // Subscribe to stop event
            _wavePlayer.PlaybackStopped += WavePlayer_PlaybackStopped;
            _volumeSampleProvider.Volume = (float)Volume;
        }

        private void WavePlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (_userStopped) return;
            if (e.Exception == null && PlaybackFinished != null) PlaybackFinished(this, new EventArgs());
            else if (PlaybackError != null) PlaybackError(this, new PlaybackErrorEventArgs("Playback has stopped due to an error!", e.Exception, false));
        }

        public void Unload()
        {
            if (_wavePlayer != null)
            {
                _wavePlayer.PlaybackStopped -= WavePlayer_PlaybackStopped;
                _wavePlayer.Stop();
            }
            if (_waveStream != null)
            {
                _waveStream.Dispose();
                _waveStream = null;
            }
            _balanceSampleProvider = null;
            _volumeSampleProvider = null;
            _equalizer = null;
        }

        public bool IsSomethingLoaded { get { return _waveStream != null; } }
        public IEnumerable<string> SupportedFileTypes { get { return _codecs.SelectMany(c => c.SupportedFileTypes).Distinct(); } }

        public void Play()
        {
            _userStopped = false;
            _wavePlayer.Play();
        }

        public void Pause()
        {
            _userStopped = true;
            _wavePlayer.Pause();
        }

        public void Resume()
        {
            _userStopped = false;
            _wavePlayer.Play();
        }

        public void Stop()
        {
            _userStopped = true;
            if (_wavePlayer != null) _wavePlayer.Stop();
            Position = TimeSpan.Zero;
            // BUG: delay needed here, otherwise PlaybackFinished is fired. 40ms is minimum on my PC. Consider unsubscribing from event instead of using bool value
            Thread.Sleep(100);
        }

        public TimeSpan Duration
        {
            get { return _waveStream != null ? _waveStream.TotalTime : TimeSpan.Zero; }
        }

        public TimeSpan Position
        {
            get { return _waveStream != null ? _waveStream.CurrentTime : TimeSpan.Zero; }
            set { if (_waveStream != null) _waveStream.CurrentTime = value; }
        }

        public bool CanResume { get { return true; } }
        public bool CanSeek { get { return _waveStream != null && _waveStream.CanSeek; } }

        public event EventHandler PlaybackFinished;
        public event EventHandler<PlaybackErrorEventArgs> PlaybackError;

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

        public bool IsEqualizerSupported { get { return true; } }
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
    }
}
