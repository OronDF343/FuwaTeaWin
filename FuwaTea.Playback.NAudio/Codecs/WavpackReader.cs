using System;
using System.IO;
using NAudio.Wave;
using WavpackDecoder;

namespace FuwaTea.Playback.NAudio.Codecs
{
    public class WavpackReader : WaveStream
    {
        private readonly BinaryReader _br;
        private readonly WavpackContext _wavpackContext;
        private readonly object _repositionLock = new object();

        public WavpackReader(string file)
        {
            _br = new BinaryReader(new BufferedStream(new FileStream(file, FileMode.Open, FileAccess.Read), 16384));
            _wavpackContext = new WavpackContext(_br);
            if (_wavpackContext.Error) throw new Exception("Wavpack Error: " + _wavpackContext.ErrorMessage);
            WaveFormat = new WaveFormat((int)_wavpackContext.SampleRate, _wavpackContext.BitsPerSample, _wavpackContext.NumChannels);
        }

        public override int Read(byte[] buffer, int offset, int count)
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

        public override WaveFormat WaveFormat { get; }

        public override long Length => _wavpackContext.TotalSamples * WaveFormat.BlockAlign;

        public override long Position
        {
            get => _wavpackContext.SampleIndex * WaveFormat.BlockAlign;
            set { lock (_repositionLock) { _wavpackContext.SetSample(value / WaveFormat.BlockAlign); } }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _br.Close();
            base.Dispose(disposing);
        }
    }
}
