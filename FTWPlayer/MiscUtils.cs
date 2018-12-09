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
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DryIoc;
using FuwaTea.Audio.Decoders;
using FuwaTea.Audio.Files;
using FuwaTea.Audio.Files.Impl;
using FuwaTea.Audio.Playback;
using FuwaTea.Lib;
using FuwaTea.Lib.Collections;
using FuwaTea.Lib.DataModel;
using log4net;

namespace FTWPlayer
{
    /// <summary>
    /// Utils for loading media
    /// </summary>
    public static class MiscUtils
    {
        /// <summary>
        /// Load media specified in command-line arguments
        /// </summary>
        /// <param name="clArgs">The command-line arguments</param>
        /// <returns>True if media was successfully loaded, false if not successfully loaded, null if no media was specified</returns>
        public static bool? ParseClArgs(List<string> clArgs)
        {
            clArgs.RemoveAt(0);
            if (clArgs.Count == 0) return null;
            var file = clArgs.Find(s => !s.StartsWith("--"));
            if (file == default) return null;
            var addOnly = clArgs.Find(s => s.ToLowerInvariant() == "--add") != default;

            return LoadObject(file, addOnly);
        }

        /// <summary>
        /// Load media
        /// </summary>
        /// <param name="file">The path to the media</param>
        /// <param name="addOnly">Specifies if the media should only be added and not played</param>
        /// <returns>True if media was successfully loaded, false if not successfully loaded</returns>
        public static bool LoadObject(string file, bool addOnly)
        {
            //TODO: update. this is temporary and WIP // TODO: error callback
            var scope = ((App)Application.Current).MainContainer.OpenScope(nameof(MiscUtils));
            var stem = scope.Resolve<ISubTrackEnumerationManager>();
            var dm = scope.Resolve<IDecoderManager>();
            var plm = scope.Resolve<IPlaybackManager>();
            var pm = scope.Resolve<IProtocolManager>();
            scope.Dispose();

            if (Directory.Exists(file))
            {
                try { if (!AddFolderQuery(new DirectoryInfo(file))) return false; }
                catch (SecurityException se)
                {
                    LogManager.GetLogger(typeof(MiscUtils)).Error("Missing permissions to folder: " + file, se);
                    MessageBox.Show(Application.Current.MainWindow, "Missing permissions to folder: " + file,
                                    "LoadObject", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else if (!File.Exists(file))
            {
                return false;
            }
            else
            {
                var locationInfo = new FileLocationInfo(new Uri(file));
                var ext = locationInfo.Extension;
                var handle = pm.Handle(locationInfo);
                if (stem.SupportedFormats.Contains(ext))
                {
                    var impl = stem.FirstCanHandleOrDefault(handle);
                    var list = impl.Handle(handle).Select(l => impl.Handle(l));
                    if (plm.List != null && addOnly)
                    {
                        //plm.MergePlaylists(plm.OpenPlaylist(file), plm.SelectedPlaylist);
                        plm.List.AddMany(list);
                    }
                    else
                    {
                        // plm.LoadedPlaylists.Add(file, plm.OpenPlaylist(file));
                        // plm.SelectedPlaylistId = file;
                        plm.List = new ObservableCollection<IFileHandle>(list);
                        return true;
                    }
                }
                else if (dm.SupportedFormats.Contains(ext))
                {
                    if (plm.List == null) plm.List = new ObservableCollection<IFileHandle>();
                    plm.List.Add(handle);
                    if (!addOnly) plm.Index = plm.List.Count - 1;
                }
                else
                {
                    return false;
                }
            }

            if (plm.Player.State != AudioPlayerState.Playing && !addOnly)
                plm.Player.Play();
            
            return true;

            // Alternative:
            //if (DirectoryEx.Exists(s))
            //    await FileUtils.AddFolder(FileSystemInfoEx.FromString(s) as DirectoryInfoEx, true, FileSystemUtils.DefaultLoadErrorCallback);
            //else if (PlaylistDataManager.Instance.HasReader(s.GetExt()))
            //    Playlist.AddRange(s, FileSystemUtils.DefaultLoadErrorCallback);
            //else if (PlaybackManagerInstance.HasSupportingPlayer(s.GetExt()))
            //    Playlist.Add(MusicInfo.Create(FileSystemInfoEx.FromString(s) as FileInfoEx, FileSystemUtils.DefaultLoadErrorCallback));
        }

        /// <summary>
        /// Add a folder and show a query asking the user if subfolders should be loaded
        /// </summary>
        /// <param name="di">The directory to load</param>
        /// <returns>True if the operation succeded</returns>
        public static bool AddFolderQuery(DirectoryInfo di)
        {
            var dr = MessageBoxResult.No;
            IEnumerable<DirectoryInfo> dirs;
            try { dirs = di.EnumerateDirectories(); }
            catch (DirectoryNotFoundException dnfe)
            {
                LogManager.GetLogger(typeof(MiscUtils)).Error("Invalid folder!", dnfe);
                MessageBox.Show(Application.Current.MainWindow, "Invalid folder!",
                                "LoadObject", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (SecurityException se)
            {
                LogManager.GetLogger(typeof(MiscUtils)).Error("Missing permissions to folder!", se);
                MessageBox.Show(Application.Current.MainWindow, "Missing permissions to folder!",
                                "LoadObject", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (dirs.Any())
                dr = MessageBox.Show(Application.Current.MainWindow, "Add files from subfolders as well?", "Add Folder", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
            var r = dr == MessageBoxResult.Yes;
            if (r || dr == MessageBoxResult.No)
                AddFolder(di, r, ex => MessageBox.Show(Application.Current.MainWindow, "Error loading a file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)); // TODO: move error callback
            return true;
        }

        /// <summary>
        /// Add files from a folder
        /// </summary>
        /// <param name="dir">The folder to load</param>
        /// <param name="subfolders">Specifies whether subfolders should be loaded as well</param>
        /// <param name="errorCallback">An error callback</param>
        public static async void AddFolder(DirectoryInfo dir, bool subfolders, Action<Exception> errorCallback)
        {
            var scope = ((App)Application.Current).MainContainer.OpenScope(nameof(MiscUtils));
            var dm = scope.Resolve<IDecoderManager>();
            var plm = scope.Resolve<IPlaybackManager>();
            var pm = scope.Resolve<IProtocolManager>();
            scope.Dispose();
            var dispatcher = Dispatcher.CurrentDispatcher;
            await Task.Run(() =>
            {
                IEnumerable<string> stuff;
                if (subfolders)
                    stuff = from f in dir.EnumerateFilesEx()
                            where dm.SupportedFormats.Contains(f.Extension)
                            select f.FullName;
                else
                    stuff = from f in dir.GetFiles()
                            where dm.SupportedFormats.Contains(f.Extension)
                            select f.FullName;
                foreach (var s in stuff)
                {
                    // TODO: Handle SubTracks here
                    var locationInfo = new FileLocationInfo(new Uri(s));
                    var ext = locationInfo.Extension;
                    var handle = pm.Handle(locationInfo);
                    try { dispatcher.InvokeAsync(() => plm.List.Add(handle), DispatcherPriority.Background); }
                    catch (Exception e)
                    {
                        errorCallback(e);
                    }
                }
            }).ConfigureAwait(false);
        }
    }
}
