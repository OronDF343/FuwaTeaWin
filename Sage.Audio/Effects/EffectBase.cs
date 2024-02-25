using System;
using CSCore;
using System.Text.Json.Serialization;

namespace Sage.Audio.Effects
{
    public abstract class EffectBase : IEffect
    {
        public abstract int Read(float[] buffer, int offset, int count);

        [JsonIgnore]
        public WaveFormat WaveFormat => BaseSource.WaveFormat;

        [JsonIgnore]
        public virtual long Position
        {
            get => BaseSource?.Position ?? 0;
            set { if (BaseSource != null) BaseSource.Position = value; }
        }

        [JsonIgnore]
        public virtual long Length => BaseSource?.Length ?? 0;

        [JsonIgnore]
        public bool CanSeek => BaseSource?.CanSeek ?? false;

        [JsonIgnore]
        public ISampleSource BaseSource { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) BaseSource?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
