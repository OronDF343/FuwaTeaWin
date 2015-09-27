using CSCore.Codecs.FLAC;
using NAudio.Wave;

namespace FuwaTea.Data.Playback.NAudio
{
    public class FlacReader : WaveStream
    {
        private readonly FlacFile _flacFile;

        public FlacReader(string file)
        {
            _flacFile = new FlacFile(file);
            WaveFormat = new WaveFormat(_flacFile.WaveFormat.SampleRate, _flacFile.WaveFormat.BitsPerSample, _flacFile.WaveFormat.Channels);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _flacFile.Read(buffer, offset, count);
        }

        public override WaveFormat WaveFormat { get; }

        public override long Length => _flacFile.Length;
        public override long Position
        {
            get { return _flacFile.Position; }
            set { _flacFile.Position = value; }
        }
    }
}
