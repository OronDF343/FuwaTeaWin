using System;
using System.IO;
using CSCore;

namespace Sage.Audio.Decoders.Wavpack
{
    public class WavpackWaveSource : IWaveSource
    {
        private readonly BinaryReader _br;
        private readonly WavpackContext _wavpackContext;
        private readonly object _repositionLock = new object();

        public WavpackWaveSource(Stream file)
        {
            //file = new BufferedStream(file, 16384);
            _br = new BinaryReader(file);
            _wavpackContext = new WavpackContext(_br);
            if (_wavpackContext.Error) throw new Exception("Wavpack Error: " + _wavpackContext.ErrorMessage);
            WaveFormat = new WaveFormat((int)_wavpackContext.SampleRate, _wavpackContext.BitsPerSample, _wavpackContext.NumChannels);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            lock (_repositionLock)
            {
                var samplesRequired = count / WaveFormat.BlockAlign;
                var tempBuffer = new int[samplesRequired * WaveFormat.Channels];
                var read = _wavpackContext.UnpackSamples(tempBuffer, samplesRequired);
                var cnv = WavpackExtraUtils.FormatSamples(_wavpackContext.BytesPerSample, tempBuffer, read * WaveFormat.Channels);
                Array.Copy(cnv, 0, buffer, offset, read * WaveFormat.BlockAlign);
                return (int)read * WaveFormat.BlockAlign;
            }
        }

        public WaveFormat WaveFormat { get; }

        public long Length => _wavpackContext.TotalSamples * WaveFormat.BlockAlign;

        public long Position
        {
            get => _wavpackContext.SampleIndex * WaveFormat.BlockAlign;
            set { lock (_repositionLock) { _wavpackContext.SetSample(value / WaveFormat.BlockAlign); } }
        }

        public bool CanSeek => _br.BaseStream.CanSeek;

        public void Dispose()
        {
            _br.Close();
        }
    }
}
