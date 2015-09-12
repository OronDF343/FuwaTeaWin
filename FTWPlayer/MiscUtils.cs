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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FuwaTea.Lib;
using FuwaTea.Lib.Exceptions;
using FuwaTea.Logic.Playlist;
using FuwaTea.Presentation.Playback;
using LayerFramework;

namespace FTWPlayer
{
    public static class MiscUtils
    {
        public static bool? ParseClArgs(List<string> clArgs)
        {
            clArgs.RemoveAt(0);
            if (clArgs.Count == 0) return null;
            var file = clArgs.Find(s => !s.StartsWith("--"));
            if (file == default(string)) return null;
            var addOnly = clArgs.Find(s => s.ToLowerInvariant() == "--add") != default(string);

            return LoadObject(file, addOnly);
        }

        public static bool LoadObject(string file, bool addOnly)
        {
            //TODO: update. this is temporary and WIP // TODO: error callback
            var plm = LayerFactory.GetElement<IPlaylistManager>();
            var pm = LayerFactory.GetElement<IPlaybackManager>();

            if (Directory.Exists(file))
            {
                AddFolderQuery(new DirectoryInfo(file));
            }
            else if (!File.Exists(file))
            {
                return false;
            }
            else
            {
                var ext = Path.GetExtension(file);
                if (plm.ReadableFileTypes.Contains(ext))
                {
                    if (addOnly)
                        plm.MergePlaylists(plm.OpenPlaylist(file), plm.SelectedPlaylist);
                    else
                    {
                        plm.LoadedPlaylists.Add(file, plm.OpenPlaylist(file)); // TODO: handle exceptions
                        plm.SelectedPlaylistId = file;
                        return true;
                    }
                }
                else if (pm.SupportedFileTypes.Contains(ext))
                {
                    plm.SelectedPlaylist.Add(file);
                    if (!addOnly) pm.JumpToAbsolute(pm.ElementCount - 1);
                }
                else
                {
                    return false;
                }
            }

            if (pm.CurrentState != PlaybackState.Playing && !addOnly)
                pm.PlayPauseResume();
            return true;

            // Alternative:
            //if (DirectoryEx.Exists(s))
            //    await FileUtils.AddFolder(FileSystemInfoEx.FromString(s) as DirectoryInfoEx, true, FileSystemUtils.DefaultLoadErrorCallback);
            //else if (PlaylistDataManager.Instance.HasReader(s.GetExt()))
            //    Playlist.AddRange(s, FileSystemUtils.DefaultLoadErrorCallback);
            //else if (PlaybackManagerInstance.HasSupportingPlayer(s.GetExt()))
            //    Playlist.Add(MusicInfo.Create(FileSystemInfoEx.FromString(s) as FileInfoEx, FileSystemUtils.DefaultLoadErrorCallback));
        }

        public static void AddFolderQuery(DirectoryInfo di)
        {
            var dr = MessageBoxResult.No;
            if (di.EnumerateDirectories().Any())
                dr = MessageBox.Show(Application.Current.MainWindow, "Add files from subfolders as well?", "Add Folder", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
            var r = dr == MessageBoxResult.Yes;
            if (r || dr == MessageBoxResult.No)
                AddFolder(di, r, ex => MessageBox.Show(Application.Current.MainWindow, "Error loading a file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)); // TODO: move error callback
        }

        public static async void AddFolder(DirectoryInfo dir, bool subfolders, ErrorCallback errorCallback)
        {
            var pm = LayerFactory.GetElement<IPlaybackManager>();
            var plm = LayerFactory.GetElement<IPlaylistManager>();
            var dispatcher = Dispatcher.CurrentDispatcher;
            await Task.Run(() =>
            {
                IEnumerable<string> stuff;
                if (subfolders)
                    stuff = from f in dir.EnumerateFilesEx()
                            where pm.SupportedFileTypes.Contains(f.Extension)
                            select f.FullName;
                else
                    stuff = from f in dir.GetFiles()
                            where pm.SupportedFileTypes.Contains(f.Extension)
                            select f.FullName;
                foreach (var s in stuff)
                {
                    try { dispatcher.InvokeAsync(() => plm.SelectedPlaylist.Add(s), DispatcherPriority.Background); }
                    catch (Exception e)
                    {
                        errorCallback(e);
                    }
                }
            });
        }
    }
}
