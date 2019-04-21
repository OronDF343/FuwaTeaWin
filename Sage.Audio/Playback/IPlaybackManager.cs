using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Sage.Audio.Files;

namespace Sage.Audio.Playback
{
    /// <summary>
    /// 
    /// </summary>
    [InheritedExport]
    public interface IPlaybackManager : INotifyPropertyChanged, IDisposable
    {
        // if [< Playing || !AllowPlayOnReload] then BehaviorOnLoadOverrideOnce = false; else BehaviorOnLoadOverrideOnce = true; Load
        void Reload();

        // OnChanged => Reload
        IAudioPlayer Player { get; set; }
        
        // TODO: These should leverage the IPlaylist type
        // OnChanged => _index = 0, Reload
        ObservableCollection<IFileHandle> List { get; set; }
        IFileHandle NowPlaying { get; }
        // Core of user functions
        // OnChanged => Reload
        int Index { get; set; }
        
        // Behavior settings
        /// <summary>
        /// Allow unloading of files externally
        /// </summary>
        bool AllowUnload { get; set; }
        /// <summary>
        /// Automatically go to next file if an error occurs *during* playback
        /// </summary>
        bool NextOnError { get; set; }
        /// <summary>
        /// Allow playing immediately after a reload
        /// </summary>
        bool AllowPlayOnReload { get; set; }
        /// <summary>
        /// The playback behavior
        /// </summary>
        PlaybackBehavior Behavior { get; set; }
        /// <summary>
        /// Set to true or false to override *once* whether playback should begin immediately or not. A value of null will respect the current Behavior.
        /// </summary>
        bool? BehaviorOnLoadOverrideOnce { get; set; }

        event EventHandler<PlaybackErrorEventArgs> Error;
    }

    public enum PlaybackBehavior
    {
        Normal,
        RepeatTrack,
        RepeatList
    }
}
