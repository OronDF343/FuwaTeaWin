using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FuwaTea.Annotations;
using FuwaTea.Common.Models;
using FuwaTea.Logic.Playback;
using FuwaTea.Logic.Playlist;
using LayerFramework;

namespace FuwaTea.Presentation.Playback
{
    [PresentationElement("Playback Manager")]
    public class PlaybackManager : IPlaybackManager
    {
        public PlaybackManager()
        {
            _playlistManager = LayerFactory.GetElement<IPlaylistManager>();
            _playlistManager.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != "SelectedPlaylist") return;
                Stop();
                _positionManagerRef.PropertyChanged -= PositionManagerRefOnPropertyChanged;
                _positionManagerRef = _playlistManager.SelectedPlaylist == null ? _nullManager : _playlistManager.SelectedPlaylist.PositionManager;
                _positionManagerRef.EnableShuffle = _shuffle;
                _positionManagerRef.PropertyChanged += PositionManagerRefOnPropertyChanged;
                _positionManagerRef.Reset();
                OnPropertyChangedCustom("ElementCount");
                OnPropertyChangedCustom("Current");
                LoadCurrentFile();
            };
            _positionManagerRef = _playlistManager.SelectedPlaylist == null ? _nullManager : _playlistManager.SelectedPlaylist.PositionManager;
            _positionManagerRef.PropertyChanged += PositionManagerRefOnPropertyChanged;
            _audioPlayers = LayerFactory.GetElements<IAudioPlayer>().ToList();
            EqualizerBands = new ObservableCollection<EqualizerBand>();
        }

        private void PositionManagerRefOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Current")
            {
                // No matter what, when the current song changes it is played here. No need to do this anywhere else!
                // If we were playing before, continue playing. If we were stopped, don't continue. Simple.
                // All other logic is built around this, to make jumps work properly automatically.
                var previousState = CurrentState;
                CurrentState = PlaybackState.Stopped;
                LoadCurrentFile();
                if (previousState != PlaybackState.Stopped) PlayPauseResume();
            }
            if (args.PropertyName == "EnableShuffle") return;
            var handler = PropertyChanged;
            if (handler != null) handler(this, args);
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
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChangedCustom(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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

        public MusicInfoModel Current { get { return _positionManagerRef.Current; } }
        public int CurrentIndex { get { return _positionManagerRef.CurrentIndex; } }
        public int CurrentIndexAbsolute { get { return _positionManagerRef.CurrentIndexAbsolute; } }
        public int ElementCount { get { return _positionManagerRef.ElementCount; } }

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

        public MusicInfoModel Peek(int index)
        {
            return _positionManagerRef.Peek(index);
        }
        #endregion

        private IAudioPlayer _currentPlayer;

        private void LoadCurrentFile()
        {
            if (IsSomethingLoaded) UnloadFile();

            if (ElementCount < 1) return; // TODO: should this throw an exception?

            if (_currentPlayer == null || !_currentPlayer.SupportedFileTypes.Contains(Current.FileType))
                _currentPlayer = _audioPlayers.FirstOrDefault(p => p.SupportedFileTypes.Contains(Current.FileType));
            if (_currentPlayer == null) throw new NotSupportedException("File type not supported"); // TODO: custom exception type
            if (_currentPlayer.EqualizerBands == null)
                _currentPlayer.EqualizerBands = EqualizerBands;
            try
            {
                _currentPlayer.Load(Current.FilePath, new AudioOutputDevice()); // TODO: IAudioDevice implementation
            } 
            catch// (Exception ex)
            {
                throw; // TODO: custom exception type
            }
            OnPropertyChangedCustom("Duration");
            _currentPlayer.PlaybackFinished += CurrentPlayer_PlaybackFinished;
            _currentPlayer.PlaybackError += CurrentPlayer_PlaybackError;
            OnPropertyChangedCustom("CanResume");
            OnPropertyChangedCustom("CanSeek");
            _currentPlayer.Volume = Volume;
            _currentPlayer.LeftVolume = LeftVolume;
            _currentPlayer.RightVolume = RightVolume;
            OnPropertyChangedCustom("IsEqualizerSupported");
            _currentPlayer.EnableEqualizer = EnableEqualizer;
            // Make sure the position shown is up-to-date
            SendPositionUpdate();
        }

        private void UnloadFile()
        {
            if (_currentPlayer == null) return;
            _currentPlayer.PlaybackFinished -= CurrentPlayer_PlaybackFinished;
            _currentPlayer.PlaybackError -= CurrentPlayer_PlaybackError;
            _currentPlayer.Unload();
            OnPropertyChangedCustom("Duration");
            OnPropertyChangedCustom("CanResume");
            OnPropertyChangedCustom("CanSeek");
            OnPropertyChangedCustom("IsEqualizerSupported");
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

        private void CurrentPlayer_PlaybackError(object sender, PlaybackErrorEventArgs args)
        {
            var handler = PlaybackError;
            if (handler != null) handler(this, args);
        }

        public bool IsSomethingLoaded { get { return _currentPlayer != null && _currentPlayer.IsSomethingLoaded; } }

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
            if (_currentPlayer != null) _currentPlayer.Stop();
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
        
        public TimeSpan Duration { get { return _currentPlayer != null ? _currentPlayer.Duration : TimeSpan.Zero; } }

        public TimeSpan Position
        {
            get { return _currentPlayer != null ? _currentPlayer.Position : TimeSpan.Zero; }
            set
            {
                if (_currentPlayer == null) return;
                _currentPlayer.Position = value;
                OnPropertyChanged();
            }
        }

        public void SendPositionUpdate()
        {
            OnPropertyChangedCustom("Position");
        }

        public bool CanResume { get { return _currentPlayer != null && _currentPlayer.CanResume; } }
        public bool CanSeek { get { return _currentPlayer != null && _currentPlayer.CanSeek; } }

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

        public event EventHandler<PlaybackErrorEventArgs> PlaybackError;

        public bool IsEqualizerSupported { get { return _currentPlayer != null && _currentPlayer.IsEqualizerSupported; } }

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

        public ObservableCollection<EqualizerBand> EqualizerBands { get; private set; }
    }
}
