using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace FuwaTea.Common.Models
{
    public interface IPlaylist : IList<MusicInfoModel>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        void Add(string musicFile);
        void AddFromPlaylist(string playlistFile);
        void AddRange(IEnumerable<MusicInfoModel> items);

        void Move(int selection, int newIndex);
        void Move(int[] selection, int newStartIndex);
        void MoveUp(int[] selection);
        void MoveDown(int[] selection);
        void MoveToTop(int[] selection);
        void MoveToBottom(int[] selection);

        void Sort(Comparison<MusicInfoModel> comparison);

        IPlaylistPositionManager PositionManager { get; }
        void Reshuffle();
        int ShuffledIndexOf(MusicInfoModel item);
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
