using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sage.Core.Ipc
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "CommandId")]
    [JsonDerivedType(typeof(IpcCloseCommand), 0)]
    [JsonDerivedType(typeof(IpcNopCommand), 1)]
    [JsonDerivedType(typeof(IpcOpenFileCommand), 2)]
    [JsonDerivedType(typeof(IpcPlaybackControlCommand), 3)]
    [JsonDerivedType(typeof(IpcPlaybackQueryCommand), 4)]
    [JsonDerivedType(typeof(IpcMetadataQueryCommand), 5)]
    public abstract class IpcCommand
    {
        [JsonRequired]
        public abstract IpcCommandId CommandId { get; }
    }

    public enum IpcCommandId
    {
        Close = 0,
        Nop = 1,
        OpenFile = 2,
        PlaybackControl = 3,
        PlaybackQuery = 4,
        MetadataQuery = 5
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

    public sealed class IpcOpenFileCommand : IpcCommand
    {
        public override IpcCommandId CommandId => IpcCommandId.OpenFile;

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

    public sealed class IpcPlaybackQueryCommand : IpcCommand
    {
        public override IpcCommandId CommandId => IpcCommandId.PlaybackQuery;

        public IpcPlaybackProperty Property { get; set; }
    }

    public enum IpcPlaybackProperty
    {
        Invalid = 0,
        PlaybackState = 1,
        Behavior = 2,
        TrackPosition = 3,
        TrackDuration = 4,
        ListPos = 5,
        ListLen = 6,
        TrackId = 7
    }

    public sealed class IpcMetadataQueryCommand : IpcCommand
    {
        public override IpcCommandId CommandId => IpcCommandId.MetadataQuery;

        public string FieldName { get; set; }
    }
}
