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
using System.Runtime.CompilerServices;
using FuwaTea.Lib;
using FuwaTea.Metadata;
using FuwaTea.Playlist;
using log4net;
using ModularFramework;

namespace FuwaTea.Playback
{
    [PlaybackElement("Playback Manager")]
    public class PlaybackManager : IPlaybackManager
    {
        public PlaybackManager()
        {
            LogManager.GetLogger(GetType()).Info("Initializing Playback Manager");
            LogManager.GetLogger(GetType()).Debug("Get IPlaylistManager");
            _playlistManager = ModuleFactory.GetElement<IPlaylistManager>();
            _playlistManager.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != nameof(IPlaylistManager.SelectedPlaylist)) return;
                LogManager.GetLogger(GetType()).Debug("SelectedPlaylist changed!");
                Stop();
                ChangePositionManager();
                OnPropertyChanged(nameof(ElementCount));
                OnPropertyChanged(nameof(Current));
                LoadCurrentFile();
            };
            ChangePositionManager(true);
            LogManager.GetLogger(GetType()).Debug("Get all IAudioPlayer");
            _audioPlayers = ModuleFactory.GetElements<IAudioPlayer>(e => LogManager.GetLogger(GetType()).Error("Problem loading an IAudioPlayer: ", e)).ToList();
            EqualizerBands = new ObservableCollection<EqualizerBand>();
        }

        private void ChangePositionManager(bool initial = false)
        {
            if (!initial) _positionManagerRef.PropertyChanged -= PositionManagerRefOnPropertyChanged;
            else LogManager.GetLogger(GetType()).Warn("Initial PositionManager setup");

            if (_playlistManager.SelectedPlaylist == null)
            {
                LogManager.GetLogger(GetType()).Debug("New SelectedPlaylist is null -> Use dummy PositionManager");
                _positionManagerRef = _nullManager;
            }
            else
                _positionManagerRef = _playlistManager.SelectedPlaylist.PositionManager;
            
            _positionManagerRef.EnableShuffle = _shuffle;
            _positionManagerRef.PropertyChanged += PositionManagerRefOnPropertyChanged;
            _positionManagerRef.Reset();
        }

        private void PositionManagerRefOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(IPlaylistPositionManager.Current))
            {
                // No matter what, when the current song changes it is played here. No need to do this anywhere else!
                // If we were playing before, continue playing. If we were stopped, don't continue. Simple.
                // All other logic is built around this, to make jumps work properly automatically.
                var previousState = CurrentState;
                CurrentState = PlaybackState.Stopped;
                LoadCurrentFile();
                if (previousState != PlaybackState.Stopped) PlayPauseResume();
            }
            if (args.PropertyName == nameof(IPlaylistPositionManager.EnableShuffle)) return;
            var handler = PropertyChanged;
            handler?.Invoke(this, args);
        }

        private readonly IPlaylistManager _playlistManager;
        private readonly IPlaylistPositionManager _nullManager = new PlaylistPositionManager();
        private readonly List<IAudioPlayer> _audioPlayers;

        #region IDisposable members

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            Stop();
            UnloadFile();
            foreach (var player in _audioPlayers)
            {
                player.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IPlaylistPositionManager members

        private IPlaylistPositionManager _positionManagerRef;

        private bool _shuffle;

        public bool EnableShuffle
        {
            get { return _shuffle; }
            set
            {
                var temp = _shuffle;
                _shuffle = _positionManagerRef.EnableShuffle = value;
                if (temp != _shuffle) OnPropertyChanged();
            }
        }

        public IMusicInfoModel Current => _positionManagerRef.Current;
        public int CurrentIndex => _positionManagerRef.CurrentIndex;
        public int CurrentIndexAbsolute => _positionManagerRef.CurrentIndexAbsolute;
        public int ElementCount => _positionManagerRef.ElementCount;

        public bool Next()
        {
            // This is what happens when the user clicks the Next button, NOT on PlaybackFinished!!!
            // null check
            if (ElementCount < 1) return false;
            // Save the old state:
            // RULES: Play only if we weren't stopped before
            var state = CurrentState;
            // If only one song is on the list (regardless of the loop setting)
            // stop if there is no loop, otherwise seek to the beginning.
            if (ElementCount < 2)
            {
                if (LoopType == LoopTypes.None) Stop();
                else Position = TimeSpan.Zero;
                return false;
            }
            // If we have a loop of all, just continue to the next song
            // It will work automatically (see: PositionManagerRefOnPropertyChanged)
            if (LoopType == LoopTypes.All) return _positionManagerRef.Next();
            // In any other case (Single or None)
            // Go to the next song, but set the state to stopped so we can control if playback will continue (see: PositionManagerRefOnPropertyChanged)
            // (NOTE: LoopTypes.Single only affects PlaybackFinished and does not imply LoopTypes.All)
            CurrentState = PlaybackState.Stopped;
            var n = _positionManagerRef.Next();
            // If we didn't reach the end, continue playback
            if (n && state != PlaybackState.Stopped) PlayPauseResume();
            return n;
        }

        public bool Previous()
        {
            // This is what happens when the user clicks the Previous button.
            // null check
            if (ElementCount < 1) return false;
            // Save the old state:
            // RULES: Play only if we weren't stopped before
            var state = CurrentState;
            // If only one song is on the list (regardless of the loop setting)
            // stop if there is no loop, otherwise seek to the beginning.
            if (ElementCount < 2)
            {
                if (LoopType == LoopTypes.None) Stop();
                else Position = TimeSpan.Zero;
                return false;
            }
            // If we have a loop of all, just go to the previous song
            // It will work automatically (see: PositionManagerRefOnPropertyChanged)
            if (LoopType == LoopTypes.All) return _positionManagerRef.Previous();
            // In any other case (Single or None)
            // Go to the previous song, but set the state to stopped so we can control if playback will continue (see: PositionManagerRefOnPropertyChanged)
            // (NOTE: LoopTypes.Single only affects PlaybackFinished and does not imply LoopTypes.All)
            CurrentState = PlaybackState.Stopped;
            var n = _positionManagerRef.Previous();
            // If we didn't reach the beginning, continue playback
            if (n && state != PlaybackState.Stopped) PlayPauseResume();
            return n;
        }

        public void JumpTo(int index)
        {
            _positionManagerRef.JumpTo(index);
        }

        public void JumpToAbsolute(int index)
        {
            _positionManagerRef.JumpToAbsolute(index);
        }

        public void Reset()
        {
            Stop();
            UnloadFile();
            _positionManagerRef.Reset();
        }

        public IMusicInfoModel Peek(int index)
        {
            return _positionManagerRef.Peek(index);
        }
        #endregion

        private IAudioPlayer _currentPlayer;

        private void LoadCurrentFile()
        {
            if (IsSomethingLoaded) UnloadFile();

            if (ElementCount < 1)
            {
                LogManager.GetLogger(GetType()).Warn("Tried to load a file from an empty playlist!");
                return;
            }

            if (Current.FilePath.StartsWith("http://") || Current.FilePath.StartsWith("https://"))
            {
                if (!(_currentPlayer is IStreamingAudioPlayer))
                    _currentPlayer = ModuleFactory.GetElement<IStreamingAudioPlayer>();
                ((IStreamingAudioPlayer)_currentPlayer).StreamMetadataChanged += OnStreamMetadataChanged;
            }
            else if (_currentPlayer == null || !_currentPlayer.GetExtensions().Contains(Current.FileType))
                _currentPlayer = _audioPlayers.FirstOrDefault(p => p.GetExtensions().Contains(Current.FileType));

            if (_currentPlayer == null)
            {
                LogManager.GetLogger(GetType()).ErrorFormat("Failed to load {0}: Unsupported file type!", Current.FilePath);
                // TODO: show messages from this function
                return;
            }
            if (_currentPlayer.EqualizerBands == null)
                _currentPlayer.EqualizerBands = EqualizerBands;
            try
            {
                _currentPlayer.Load(Current.FilePath, new AudioOutputDevice()); // TODO: IAudioDevice implementation
            } 
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType()).Error($"Failed to load {Current.FilePath} for playback on the device {"\"default\""} because the selected IAudioPlayer {_currentPlayer.GetType().FullName} threw an exception:", ex);
                // TODO: show messages from this function
                return;
            }
            if (Current.FilePath.StartsWith("http://") || Current.FilePath.StartsWith("https://"))
                Current.Tag = ((IStreamingAudioPlayer)_currentPlayer).StreamMetadata;
            OnPropertyChanged(nameof(Duration));
            _currentPlayer.PlaybackFinished += CurrentPlayer_PlaybackFinished;
            OnPropertyChanged(nameof(CanResume));
            OnPropertyChanged(nameof(CanSeek));
            _currentPlayer.Volume = Volume;
            _currentPlayer.LeftVolume = LeftVolume;
            _currentPlayer.RightVolume = RightVolume;
            OnPropertyChanged(nameof(IsEqualizerSupported));
            _currentPlayer.EnableEqualizer = EnableEqualizer;
            // Make sure the position shown is up-to-date
            SendPositionUpdate();
        }

        private void OnStreamMetadataChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Current));
        }

        private void UnloadFile()
        {
            if (_currentPlayer == null) return;
            _currentPlayer.PlaybackFinished -= CurrentPlayer_PlaybackFinished;
            var player = _currentPlayer as IStreamingAudioPlayer;
            if (player != null) player.StreamMetadataChanged -= OnStreamMetadataChanged;
            _currentPlayer.Unload();
            OnPropertyChanged(nameof(Duration));
            OnPropertyChanged(nameof(CanResume));
            OnPropertyChanged(nameof(CanSeek));
            OnPropertyChanged(nameof(IsEqualizerSupported));
        }

        private void CurrentPlayer_PlaybackFinished(object sender, EventArgs eventArgs)
        {
            // NOTE: use _positionManagerRef.Next() so we don't use the logic for when the user clicks next in Next()
            // If only one song is on the list (or it is looped so virtually we have just one)
            // seek to the beginning of it
            if (ElementCount < 2 || LoopType == LoopTypes.Single)
                Position = TimeSpan.Zero;

            if (LoopType == LoopTypes.None)
            {
                // Go to the next song, but set the state to stopped so we can control if playback will continue (see: PositionManagerRefOnPropertyChanged)
                CurrentState = PlaybackState.Stopped;
                var n = _positionManagerRef.Next();
                // If we didn't reach the end, continue playback
                if (n) PlayPauseResume();
            }
            else if (LoopType == LoopTypes.All && ElementCount > 1)
            {
                // Just continue, it will work automatically (see: PositionManagerRefOnPropertyChanged)
                _positionManagerRef.Next();
            }
            else
            {
                // If we are in a loop, OR we are looping the entire list when it has only one song
                // Set the state to stopped, that will make PlayPauseResume() choose the Play action
                CurrentState = PlaybackState.Stopped;
                PlayPauseResume();
            }
        }

        public bool IsSomethingLoaded => _currentPlayer != null && _currentPlayer.IsSomethingLoaded;

        public IEnumerable<string> SupportedFileTypes { get { return _audioPlayers.SelectMany(p => p.SupportedFileTypes); } }

        public void PlayPauseResume()
        {
            if (ElementCount < 1) return; // TODO: is this the right place to check this?
            switch (CurrentState)
            {
                case PlaybackState.Stopped:
                    if (!IsSomethingLoaded) LoadCurrentFile();
                    _currentPlayer.Play();
                    CurrentState = PlaybackState.Playing;
                    return;
                case PlaybackState.Paused:
                    if (CanResume) _currentPlayer.Resume();
                    else _currentPlayer.Play();
                    CurrentState = PlaybackState.Playing;
                    return;
                case PlaybackState.Playing:
                    _currentPlayer.Pause();
                    CurrentState = PlaybackState.Paused;
                    return;
            }
        }

        public void Stop()
        {
            _currentPlayer?.Stop();
            CurrentState = PlaybackState.Stopped;
        }

        private PlaybackState _state;

        public PlaybackState CurrentState
        {
            get { return _state; }
            private set
            {
                var temp = _state;
                _state = value;
                if (temp != _state) OnPropertyChanged();
            }
        }

        private LoopTypes _loop;
        public LoopTypes LoopType { get { return _loop; } set { _loop = value; OnPropertyChanged(); } }
        
        public TimeSpan Duration => _currentPlayer?.Duration ?? TimeSpan.Zero;

        public TimeSpan Position
        {
            get { return _currentPlayer?.Position ?? TimeSpan.Zero; }
            set
            {
                if (_currentPlayer == null) return;
                _currentPlayer.Position = value;
                OnPropertyChanged();
            }
        }

        public void SendPositionUpdate()
        {
            OnPropertyChanged(nameof(Position));
        }

        public bool CanResume => _currentPlayer != null && _currentPlayer.CanResume;
        public bool CanSeek => _currentPlayer != null && _currentPlayer.CanSeek;

        private decimal _volume = 1.0m;
        public decimal Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                if (_currentPlayer != null) _currentPlayer.Volume = _volume;
                OnPropertyChanged();
            }
        }
        private decimal _leftVolume = 1.0m;
        public decimal LeftVolume
        {
            get { return _leftVolume; }
            set
            {
                _leftVolume = value;
                if (_currentPlayer != null) _currentPlayer.LeftVolume = _leftVolume;
                OnPropertyChanged();
            }
        }
        private decimal _rightVolume = 1.0m;
        public decimal RightVolume
        {
            get { return _rightVolume; }
            set
            {
                _rightVolume = value;
                if (_currentPlayer != null) _currentPlayer.RightVolume = _rightVolume;
                OnPropertyChanged();
            }
        }

        public bool IsEqualizerSupported => _currentPlayer != null && _currentPlayer.IsEqualizerSupported;

        private bool _enableEq;
        public bool EnableEqualizer
        {
            get
            {
                return _enableEq;
            }
            set
            {
                _enableEq = value;
                if (_currentPlayer != null) _currentPlayer.EnableEqualizer = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EqualizerBand> EqualizerBands { get; }
    }
}
