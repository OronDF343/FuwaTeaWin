using System.ComponentModel;

namespace FuwaTea.Common.Models
{
    public interface IPlaylistPositionManager : INotifyPropertyChanged
    {
        bool EnableShuffle { get; set; }
        MusicInfoModel Current { get; }
        int CurrentIndex { get; }
        int CurrentIndexAbsolute { get; }
        int ElementCount { get; }

        bool Next();
        bool Previous();
        void JumpTo(int index);
        void JumpToAbsolute(int index);
        void Reset();
        MusicInfoModel Peek(int index);
    }
}