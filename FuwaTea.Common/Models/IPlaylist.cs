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
using System.Collections.Specialized;
using System.ComponentModel;

namespace FuwaTea.Common.Models
{
    public interface IPlaylist : IList<IMusicInfoModel>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        void Add(string musicFile);
        void AddFromPlaylist(string playlistFile);
        void AddRange(IEnumerable<IMusicInfoModel> items);

        void Move(int selection, int newIndex);
        void Move(int[] selection, int newStartIndex);
        void MoveUp(int[] selection);
        void MoveDown(int[] selection);
        void MoveToTop(int[] selection);
        void MoveToBottom(int[] selection);

        void Sort(Comparison<IMusicInfoModel> comparison);

        IPlaylistPositionManager PositionManager { get; }
        void Reshuffle();
        int ShuffledIndexOf(IMusicInfoModel item);
        int AbsoluteToShuffled(int index);
        int ShuffledToAbsolute(int index);

        void Open(string path);
        void Save();
        void SaveTo(string path);
        void SaveCopy(string path);
        string FileLocation { get; set; }
        bool UnsavedChanges { get; }
    }
}
