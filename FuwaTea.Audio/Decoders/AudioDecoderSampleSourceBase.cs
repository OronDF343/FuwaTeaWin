using System;
using System.IO;
using CSCore;

namespace FuwaTea.Audio.Decoders
{
    public abstract class AudioDecoderSampleSourceBase : ISampleSource
    {
        public AudioDecoderSampleSourceBase(Stream stream)
        {
            SourceStream = stream;
        }

        public abstract int Read(float[] buffer, int offset, int count);

        protected Stream SourceStream { get; }
        public virtual bool CanSeek => SourceStream.CanSeek;
        public abstract WaveFormat WaveFormat { get; }
        public abstract long Position { get; set; }
        public abstract long Length { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                SourceStream?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}