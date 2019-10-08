using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sage.Core.Ipc
{
    public class IpcCommandJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IpcCommand);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            return jo["CommandId"].Value<IpcCommandId>() switch
            {
                IpcCommandId.Nop => jo.ToObject<IpcNopCommand>(serializer),
                IpcCommandId.Open => jo.ToObject<IpcOpenCommand>(serializer),
                IpcCommandId.PlaybackControl => jo.ToObject<IpcPlaybackControlCommand>(serializer),
                _ => (IpcCommand)null
            };
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
    }
}
