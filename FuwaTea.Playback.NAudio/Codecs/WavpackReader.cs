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
            var fistream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var bstream = new BufferedStream(fistream, 16384);
            _br = new BinaryReader(bstream);
            _wavpackContext = WavpackUtils.WavpackOpenFileInput(_br);
            if (_wavpackContext.error) throw new Exception("Wavpack Error: " + _wavpackContext.error_message);
            WaveFormat = new WaveFormat((int)WavpackUtils.WavpackGetSampleRate(_wavpackContext), WavpackUtils.WavpackGetBitsPerSample(_wavpackContext), WavpackUtils.WavpackGetNumChannels(_wavpackContext));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_repositionLock)
            {
                var samplesRequired = count / WaveFormat.BlockAlign;
                var tempBuffer = new int[samplesRequired * WaveFormat.Channels];
                var read = WavpackUtils.WavpackUnpackSamples(_wavpackContext, tempBuffer, samplesRequired);
                var cnv = WavpackExtraUtils.FormatSamples(WavpackUtils.WavpackGetBytesPerSample(_wavpackContext), tempBuffer, read * WaveFormat.Channels);
                Array.Copy(cnv, 0, buffer, offset, read * WaveFormat.BlockAlign);
                return (int)read * WaveFormat.BlockAlign;
            }
        }

        public override WaveFormat WaveFormat { get; }

        public override long Length => WavpackUtils.WavpackGetNumSamples(_wavpackContext) * WaveFormat.BlockAlign;

        public override long Position
        {
            get { return WavpackUtils.WavpackGetSampleIndex(_wavpackContext) * WaveFormat.BlockAlign; }
            set { lock (_repositionLock) { WavpackUtils.setSample(_wavpackContext, value / WaveFormat.BlockAlign); } }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _br.Close();
            base.Dispose(disposing);
        }
    }
}
