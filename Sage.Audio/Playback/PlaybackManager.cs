using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using CSCore;
using JetBrains.Annotations;
using Sage.Audio.Decoders;
using Sage.Audio.Effects;
using Sage.Audio.Files;

namespace Sage.Audio.Playback
{
    public class PlaybackManager : IPlaybackManager
    {
        public PlaybackManager([Import] IDecoderManager decoderManager, [Import] IAudioPlayer player)
        {
            _decoderManager = decoderManager;
            Player = player;
            Effects = new ObservableCollection<IEffect>();
            List = new ObservableCollection<IFileHandle>();
        }

        private int _index;
        private IAudioPlayer _player;
        private ObservableCollection<IFileHandle> _list;
        private readonly IDecoderManager _decoderManager;
        private ObservableCollection<IEffect> _effects;

        public void Reload()
        {
            if (BehaviorOnLoadOverrideOnce == null)
            {
                if ((int)Player.State < (int)AudioPlayerState.Playing || !AllowPlayOnReload)
                    BehaviorOnLoadOverrideOnce = false;
                else BehaviorOnLoadOverrideOnce = true;
            }
            Load();
        }

        [CanBeNull]
        public IAudioPlayer Player
        {
            get => _player;
            set
            {
                if (ReferenceEquals(_player, value)) return;
                if (_player != null) _player.StateChanged -= PlayerOnStateChanged;
                _player = value;
                if (_player != null) _player.StateChanged += PlayerOnStateChanged;
                OnPropertyChanged();
                Reload();
            }
        }

        [CanBeNull]
        public ObservableCollection<IFileHandle> List
        {
            get => _list;
            set
            {
                if (_list != null) _list.CollectionChanged -= ListOnCollectionChanged;
                _list = value;
                if (_list != null) _list.CollectionChanged += ListOnCollectionChanged;
                OnPropertyChanged();
                ForceReset();
            }
        }

        [CanBeNull]
        public IFileHandle NowPlaying =>  List != null && List.Count > 0 ? List[Index] : null;
        public int Index
        {
            get => _index;
            set
            {
                if (SneakyUpdateIndex(value)) Reload();
            }
        }
        public bool AllowUnload { get; set; }
        public bool NextOnError { get; set; }
        public bool AllowPlayOnReload { get; set; } = true;
        public PlaybackBehavior Behavior { get; set; }
        public bool? BehaviorOnLoadOverrideOnce { get; set; }
        public event EventHandler<PlaybackErrorEventArgs> Error;
        // TODO: Save effects config
        public ObservableCollection<IEffect> Effects
        {
            get => _effects;
            set
            {
                if (_effects != null) _effects.CollectionChanged -= EffectsOnCollectionChanged;
                _effects = value;
                if (_effects != null) _effects.CollectionChanged += EffectsOnCollectionChanged;
                OnPropertyChanged();
                Reload();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnError(PlaybackErrorEventArgs e)
        {
            // if [NextOnError] then FileEnd; else nop
            // TODO: Log
            Error?.Invoke(this, e);
            if (NextOnError) FileEnd();
        }

        /* From here on: The logic */
        
        private bool SneakyUpdateIndex(int i)
        {
            if (_index == i) return false;
            _index = i;
            OnPropertyChanged(nameof(Index));
            return true;
        }

        private void ForceReset(int i = 0)
        {
            BehaviorOnLoadOverrideOnce = false;
            _index = i;
            OnPropertyChanged(nameof(Index));
            Load();
        }

        private void Load()
        {
            // TODO: Where to handle errors?
            Player?.Unload();
            if (NowPlaying == null) return;
            var wavSrc = _decoderManager.Handle(NowPlaying);
            if (_effects != null && _effects.Count > 0)
            {
                var sampleSrc = wavSrc.ToSampleSource();
                // TODO: Handle duplicate effects...
                foreach (var effect in _effects)
                {
                    effect.BaseSource = sampleSrc;
                    sampleSrc = effect;
                }
                wavSrc = sampleSrc.ToWaveSource();
            }
            Player?.Load(wavSrc);
        }

        private void FileEnd()
        {
            /* Possible reasons to get Loaded, Manual:
             * Reload - Always copy state
             * Index changed - See below
             * Manual load (should not happen) - Never play
             *
             * Possible reasons to get Index changed from user's standpoint:
             * Next on Error - Respect behavior
             * Playback finished - Respect behavior
             * Clicked "Next" - Always copy state
             * Clicked "Previous" - Always copy state
             * Selected specific file - Always play
             *
             * Solution: On index change always call reload. On error or finished, respect. On select, use override manually from UI.
             */
            // TODO ISSUE: Behavior is not self-contained!
            if (Behavior == PlaybackBehavior.RepeatTrack && List.Count > 0)
            {
                Player?.Play();
                return;
            }
            if (List.Count > 0 && SneakyUpdateIndex((Index + 1) % List.Count))
                Load();
        }

        private void PlayerOnStateChanged(object sender, AudioPlayerStateChangedEventArgs args)
        {
            /* Possible transitions and actions:
             * NotReady, Automatic - We need to reload the file due to API configuration changes. Equivalent to switching players. => Reload
             * NotReady, Manual - Unload() was called. This is intentional => nop
             * NotReady, Error - Failed to load the file. Notify the user and move to the next one. => Error, FileEnd
             * Loaded, Automatic - Should not happen; Warn. => Warn
             * Loaded, Manual - File loaded. => if [CheckBehavior()] then Play; else nop
             * Loaded, Error - Initial playback error. Similar to Stopped, Error. => OnError
             * Stopped, Automatic - Playback finished. => FileEnd
             * Stopped, Manual - Stop() was called. => nop
             * Stopped, Error - Playback error. => OnError
             * Paused, Automatic - Should not happen; Warn. => Warn
             * Paused, Manual - Pause() was called. => nop
             * Paused, Error - Resume error. Stop() and Play(). => Stop, Error, Play 
             * Playing, Automatic - Should not happen; Warn. => Warn
             * Playing, Manual - Play() was called. => nop
             * Playing, Error - Pause error. Call Stop() and Error. => Stop, Error
             */
            switch (args.NewState)
            {
                case AudioPlayerState.NotReady:
                    switch (args.TransitionInitiator)
                    {
                        case TransitionInitiator.Automatic:
                            Reload();
                            break;
                        case TransitionInitiator.Manual:
                            break;
                        case TransitionInitiator.Error:
                            OnError(new PlaybackErrorEventArgs(args.ErrorInfo));
                            FileEnd();
                            break;
                    }
                    break;
                case AudioPlayerState.Loaded:
                    switch (args.TransitionInitiator)
                    {
                        case TransitionInitiator.Automatic:
                            // TODO: Warn
                            break;
                        case TransitionInitiator.Manual:
                            if (CheckBehavior()) Player?.Play();
                            break;
                        case TransitionInitiator.Error:
                            OnError(new PlaybackErrorEventArgs(args.ErrorInfo));
                            break;
                    }
                    break;
                case AudioPlayerState.Stopped:
                    switch (args.TransitionInitiator)
                    {
                        case TransitionInitiator.Automatic:
                            FileEnd();
                            break;
                        case TransitionInitiator.Manual:
                            // nop
                            break;
                        case TransitionInitiator.Error:
                            OnError(new PlaybackErrorEventArgs(args.ErrorInfo));
                            break;
                    }
                    break;
                case AudioPlayerState.Paused:
                    switch (args.TransitionInitiator)
                    {
                        case TransitionInitiator.Automatic:
                            // TODO: Warn
                            break;
                        case TransitionInitiator.Manual:
                            // nop
                            break;
                        case TransitionInitiator.Error:
                            Player?.Stop();
                            // TODO: Proper error reporting
                            OnError(new PlaybackErrorEventArgs("Resume error"));
                            Player?.Play();
                            break;
                    }
                    break;
                case AudioPlayerState.Playing:
                    switch (args.TransitionInitiator)
                    {
                        case TransitionInitiator.Automatic:
                            // TODO: Warn
                            break;
                        case TransitionInitiator.Manual:
                            // nop
                            break;
                        case TransitionInitiator.Error:
                            Player?.Stop();
                            // TODO: Proper error reporting
                            OnError(new PlaybackErrorEventArgs("Pause error"));
                            break;
                    }
                    break;
            }
        }

        private bool CheckBehavior()
        {
            if (BehaviorOnLoadOverrideOnce != null)
            {
                var r = BehaviorOnLoadOverrideOnce.Value;
                BehaviorOnLoadOverrideOnce = null;
                return r;
            }

            switch (Behavior)
            {
                case PlaybackBehavior.Normal:
                    return _index > 0;
                case PlaybackBehavior.RepeatList:
                    return true;
                case PlaybackBehavior.RepeatTrack:
                    // Should not happen, ever. In theory...
                    return false;
            }

            return false;
        }

        private void ListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    // Handle Insert as well
                    // If the change is beyond us, ignore the change
                    if (e.NewStartingIndex > Index) return;
                    // If the list used to be empty, load the first item
                    if (List.Count == e.NewItems.Count) Reload();
                    // Otherwise, correct our index quietly
                    else SneakyUpdateIndex(Index + e.NewItems.Count);
                    break;
                case NotifyCollectionChangedAction.Move:
                    // First case: Items moved all from after our index
                    if (e.OldStartingIndex > Index)
                    {
                        // If the new position is also after us, ignore the change
                        if (e.NewStartingIndex > Index) return;
                        // Otherwise, correct our index quietly forwards
                        SneakyUpdateIndex(Index + e.NewItems.Count);
                    }
                    // Second case: Items moved all from before our index
                    else if (e.OldStartingIndex + e.NewItems.Count < Index)
                    {
                        // If the new position is before us and does not overlap us, ignore the change
                        if (e.NewStartingIndex + e.NewItems.Count < Index) return;
                        // If the moved items overlap or are beyond our old index, correct our index quietly backwards
                        SneakyUpdateIndex(Index - e.NewItems.Count);
                    }
                    // Final case: We are one of the moved items
                    else
                    {
                        // First get our index within NewItems
                        var relativeIndex = Index - e.OldStartingIndex;
                        // Correct us to this offset from the new location
                        SneakyUpdateIndex(e.NewStartingIndex + relativeIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    // First case: Items removed after us - ignore
                    if (e.OldStartingIndex > Index) return;
                    // Second case: Items removed before us - correct
                    if (e.OldStartingIndex + e.OldItems.Count < Index)
                        SneakyUpdateIndex(Index - e.OldItems.Count);
                    // Final case: We were removed as well - move back and stop
                    else ForceReset(Math.Max(e.OldStartingIndex - 1, 0));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    // If we are not affected - ignore
                    if (e.OldStartingIndex + e.OldItems.Count < Index || e.OldStartingIndex > Index) return;
                    // If we are replaced - reload and stop
                    ForceReset(Index);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // Move to start and stop
                    ForceReset();
                    break;
            }
        }

        private void EffectsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Reload();
        }

        public void Dispose()
        {
            _player?.Dispose();
        }
    }
}
