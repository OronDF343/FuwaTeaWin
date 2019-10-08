using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sage.Core.Ipc
{
    public abstract class IpcCommand
    {
        [JsonRequired]
        public abstract IpcCommandId CommandId { get; }
    }

    public enum IpcCommandId
    {
        Close = 0,
        Nop = 1,
        Open = 2,
        PlaybackControl = 3,
    }

    public sealed class IpcCloseCommand : IpcCommand
    {
        public override IpcCommandId CommandId => IpcCommandId.Close;
    }

    public sealed class IpcNopCommand : IpcCommand
    {
        public override IpcCommandId CommandId => IpcCommandId.Nop;

        public int Delay { get; set; }
    }

    public sealed class IpcOpenCommand : IpcCommand
    {
        public override IpcCommandId CommandId => IpcCommandId.Open;

        public List<string> Files { get; set; }
    }

    public sealed class IpcPlaybackControlCommand : IpcCommand
    {
        public override IpcCommandId CommandId => IpcCommandId.PlaybackControl;

        public IpcPlaybackOpCode OpCode { get; set; }

        public int Parameter { get; set; }
    }

    public enum IpcPlaybackOpCode
    {
        Invalid = 0,
        PlayerToggle = 1,
        PlayerStop = 2,
        PlayerPause = 3,
        PlayerResume = 4,
        PlayerPlay = 5,
        TrackPositionSeek = 6,
        TrackPositionSet = 7,
        ListSeek = 8,
        ListSetPos = 9,
        BehaviorToggle = 10,
        BehaviorSet = 11
    }
}
