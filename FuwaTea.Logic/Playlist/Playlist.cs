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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FuwaTea.Common.Models;
using FuwaTea.Data.Playlist;
using FuwaTea.Data.Playlist.Tags;
using FuwaTea.Lib.Collections;
using LayerFramework;

namespace FuwaTea.Logic.Playlist
{
    public class Playlist : ObservableCollection<IMusicInfoModel>, IPlaylist
    {
        public Playlist()
        {
            PositionManager = new PlaylistPositionManager(this);
            _shuffleMap = new int[0];
        }

        public void Add(string musicFile)
        {
            Add(LayerFactory.GetElements<ITagReader>().First(r => r.SupportedFileTypes.Contains(Path.GetExtension(musicFile))).ReadTag(musicFile));
        }

        public void AddFromPlaylist(string playlistFile)
        {
            foreach (var s in LayerFactory.GetElements<IPlaylistReader>().First(r => r.SupportedFileTypes.Contains(Path.GetExtension(playlistFile))).GetPlaylistFiles(playlistFile))
                Add(s);
        }

        public void AddRange(IEnumerable<IMusicInfoModel> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Move(int[] selection, int newStartIndex)
        {
            Array.Sort(selection);
            if (newStartIndex <= selection[0])
                for (var i = 0; i < selection.Length; ++i)
                    Move(selection[i], newStartIndex + i);
            else if (newStartIndex >= selection[selection.Length - 1])
                for (var i = selection.Length - 1; i > -1; --i)
                    Move(selection[i], newStartIndex - selection.Length + i);
            else
            {

                var j = 1;
                for (; !(selection[j - 1] < newStartIndex && selection[j] >= newStartIndex); ++j);
                 // IndexOutOfRange happens in unexpected case
                var arr1 = new int[j];
                var arr2 = new int[selection.Length - j];
                Array.Copy(selection, arr1, arr1.Length);
                Array.Copy(selection, j, arr2, 0, arr2.Length);
                Move(arr2, newStartIndex);
                Move(arr1, newStartIndex);
            }
        }

        public void MoveUp(int[] selection)
        {
            Array.Sort(selection);
            for (var i = 0; i < selection.Length; ++i)
                Move(selection[i], selection[i] - 1);
        }

        public void MoveDown(int[] selection)
        {
            Array.Sort(selection);
            for (var i = selection.Length - 1; i > -1; --i)
                Move(selection[i], selection[i] + 1);
        }

        public void MoveToTop(int[] selection)
        {
            Array.Sort(selection);
            for (var i = 0; i < selection.Length; ++i)
                Move(selection[i], i);
        }

        public void MoveToBottom(int[] selection)
        {
            Array.Sort(selection);
            for (var i = selection.Length - 1; i > -1; --i)
                Move(selection[i], Count - selection.Length + i);
        }

        public void Sort(Comparison<IMusicInfoModel> comparison)
        {
            ArrayList.Adapter(this).Sort(new SortComparer<IMusicInfoModel>(comparison));
        }

        public IPlaylistPositionManager PositionManager { get; private set; }

        private int[] _shuffleMap;
        private readonly Random _randomizer = new Random();
        public void Reshuffle()
        {
            _shuffleMap = new int[Count];
            for (var i = 0; i < Count; ++i)
                _shuffleMap[i] = i;
            for (var i = 0; i < Count; ++i)
            {
                var r = _randomizer.Next(Count);
                var temp = _shuffleMap[i];
                _shuffleMap[i] = _shuffleMap[r];
                _shuffleMap[r] = temp;
            }
        }

        public int ShuffledIndexOf(IMusicInfoModel item)
        {
            if (Count == 0) return 0; 
            if (_shuffleMap.Length != Count) Reshuffle();
            return _shuffleMap[IndexOf(item)];
        }

        public int AbsoluteToShuffled(int index)
        {
            if (Count == 0) return 0; 
            if (_shuffleMap.Length != Count) Reshuffle();
            return _shuffleMap[index];
        }

        public int ShuffledToAbsolute(int index)
        {
            return Array.IndexOf(_shuffleMap, index);
        }

        public void Open(string path)
        {
            Clear();
            FileLocation = path;
            AddFromPlaylist(path);
            UnsavedChanges = false;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(FileLocation)) throw new InvalidOperationException("No file open!");
            SaveTo(FileLocation);
        }

        public void SaveTo(string path)
        {
            SaveCopy(FileLocation = path);
            UnsavedChanges = false;
        }

        public void SaveCopy(string path)
        {
            LayerFactory.GetElements<IPlaylistWriter>().First(w => w.SupportedFileTypes.Contains(Path.GetExtension(path))).WritePlaylist(path, this.Select(m => m.FilePath), true); // TODO: place for relative path option etc
        }

        private string _fileLocation;

        public string FileLocation
        {
            get { return _fileLocation; }
            set
            {
                _fileLocation = value;
                UnsavedChanges = true;
                OnPropertyChanged(new PropertyChangedEventArgs("FileLocation"));
            }
        }

        private bool _unsaved;
        public bool UnsavedChanges { get { return _unsaved; } private set { _unsaved = value; OnPropertyChanged(new PropertyChangedEventArgs("UnsavedChanges")); } }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            UnsavedChanges = true;
            base.OnCollectionChanged(e);
        }
    }
}
