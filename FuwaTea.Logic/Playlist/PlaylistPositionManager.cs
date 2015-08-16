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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using FuwaTea.Common.Models;

namespace FuwaTea.Logic.Playlist
{
    public class PlaylistPositionManager : IPlaylistPositionManager // TODO: thread safe?
    {
        public PlaylistPositionManager() // TODO: shouldn't exist
            : this(new Playlist()) { }

        public PlaylistPositionManager(IPlaylist pm)
        {
            _playlist = pm;
            _playlist.CollectionChanged +=
                (sender, args) => { if (args.NewItems?.Contains(Current) == true) OnPropertyChanged(nameof(Current)); OnPropertyChanged(nameof(ElementCount)); };
        }

        private readonly IPlaylist _playlist;

        private bool _shuffle;

        public bool EnableShuffle
        {
            get
            {
                return _shuffle;
            }
            set
            {
                var temp = _shuffle;
                _shuffle = value;
                if (temp != _shuffle) OnPropertyChanged();
                if (ElementCount < 1) return;
                if (!temp && _shuffle) CurrentIndex = _playlist.AbsoluteToShuffled(CurrentIndex);
                else if (temp && !_shuffle) CurrentIndex = _playlist.ShuffledToAbsolute(CurrentIndex);
            }
        }

        public IMusicInfoModel Current => _playlist.Count > CurrentIndex ? _playlist[EnsureAbsoluteIndex(CurrentIndex)] : null;

        private int _index;

        public int CurrentIndex
        {
            get { return _index; }
            private set
            {
                var temp = _index;
                _index = value;
                if (temp == _index) return;
                OnPropertyChanged(nameof(CurrentIndexAbsolute));
                OnPropertyChanged();
                OnPropertyChanged(nameof(Current));
            }
        }

        public int CurrentIndexAbsolute => EnsureAbsoluteIndex(CurrentIndex);
        public int ElementCount => _playlist.Count;

        private int EnsureAbsoluteIndex(int index)
        {
            return EnableShuffle ? _playlist.ShuffledToAbsolute(index) : index;
        }

        private int OptionallyShuffledIndex(int index)
        {
            return EnableShuffle ? _playlist.AbsoluteToShuffled(index) : index;
        }

        public bool Next()
        {
            var isEnd = CurrentIndex >= _playlist.Count - 1;
            JumpTo(isEnd ? 0 : CurrentIndex + 1);
            return !isEnd;
        }

        public bool Previous()
        {
            var isBegin = CurrentIndex == 0;
            JumpTo(isBegin ? _playlist.Count - 1 : CurrentIndex - 1);
            return !isBegin;
        }

        public void JumpTo(int index)
        {
            CurrentIndex = index;
        }

        public void JumpToAbsolute(int index)
        {
            CurrentIndex = OptionallyShuffledIndex(index);
        }

        public void Reset()
        {
            _index = OptionallyShuffledIndex(0);
            OnPropertyChanged(nameof(CurrentIndexAbsolute));
            OnPropertyChanged(nameof(CurrentIndex));
            OnPropertyChanged(nameof(Current));
        }

        public IMusicInfoModel Peek(int index)
        {
            return _playlist[EnsureAbsoluteIndex(index)];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
