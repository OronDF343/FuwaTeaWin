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
using FuwaTea.Lib;
using FuwaTea.Lib.Collections;
using FuwaTea.Metadata;
using FuwaTea.Metadata.Tags;

namespace FuwaTea.Playlist
{
    public class Playlist : ObservableCollection<IMusicInfoModel>, IPlaylist
    {
        public Playlist()
        {
            PositionManager = new PlaylistPositionManager(this);
            _shuffleMap = new int[0];
        }

        public Playlist(List<ITagProvider> tagProviders) // TODO: Potential issues because tag providers are shared between playlists by reference!!!
        {
            _tagProviders = tagProviders;
            PositionManager = new PlaylistPositionManager(this);
            _shuffleMap = new int[0];
        }

        private bool _init;
        private readonly List<ITagProvider> _tagProviders;

        public void Init(string path, IEnumerable<string> items)
        {
            FileLocation = path;
            _init = true;
            AddRange(items);
            _init = false;
        }

        public void Init(string path, IEnumerable<IMusicInfoModel> items)
        {
            FileLocation = path;
            _init = true;
            AddRange(items);
            _init = false;
        }

        public void Add(string musicFile)
        {
            var tag = _tagProviders?.FirstOrDefault(r => r.GetExtensions().Contains(Path.GetExtension(musicFile).ToLowerInvariant()));
            Add(new MusicInfoModel(musicFile, tag?.Create(musicFile)));
        }

        public void AddRange(IEnumerable<IMusicInfoModel> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void AddRange(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
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
                for (; !(selection[j - 1] < newStartIndex && selection[j] >= newStartIndex); ++j) { }
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
            // ReSharper disable once ForCanBeConvertedToForeach
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

        public IPlaylistPositionManager PositionManager { get; }

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

        private string _fileLocation;

        public string FileLocation
        {
            get => _fileLocation;
            set
            {
                _fileLocation = value;
                UnsavedChanges = true;
                OnPropertyChanged(new PropertyChangedEventArgs("FileLocation"));
            }
        }

        private bool _unsaved;
        public bool UnsavedChanges { get => _unsaved; set { _unsaved = value; OnPropertyChanged(new PropertyChangedEventArgs("UnsavedChanges")); } }
        public string Title { get; set; }
        public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            UnsavedChanges = !_init;
            base.OnCollectionChanged(e);
        }
    }
}
