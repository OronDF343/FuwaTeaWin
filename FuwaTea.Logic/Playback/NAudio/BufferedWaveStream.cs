using System;
using System.Threading;
using NAudio.Wave;

namespace FuwaTea.Logic.Playback.NAudio
{
    public class BufferedWaveStream : WaveStream
    {
        public BufferedWaveStream(IWaveProvider src)
        {
            _src = src;
            _bwp = new BufferedWaveProvider(_src.WaveFormat) {BufferDuration = TimeSpan.FromSeconds(10)};
            // Initial fill buffer
            (_th = new Thread(FillBuffer)).Start();
        }

        private readonly IWaveProvider _src;
        private readonly BufferedWaveProvider _bwp;
        private Thread _th;

        public override int Read(byte[] buffer, int offset, int count)
        {
            // If buffer is at less than 20%, fill it
            if (!_filling && _bwp.BufferedBytes < _bwp.BufferLength / 5)
                (_th = new Thread(FillBuffer)).Start();
            return _bwp.Read(buffer, offset, count);
        }

        private volatile bool _filling;
        private void FillBuffer()
        {
            _filling = true;
            // Fill buffer to 100%
            var bytesToFill = _bwp.BufferLength - _bwp.BufferedBytes;
            // Do it in 4 batches
            for (var i = 0; i < 4; i++)
            {
                var buffer = new byte[bytesToFill / 4];
                var read = _src.Read(buffer, 0, bytesToFill / 4);
                _bwp.AddSamples(buffer, 0, read);
            }
            _filling = false;
        }

        public override WaveFormat WaveFormat => _src.WaveFormat;

        public TimeSpan BufferSize { get { return _bwp.BufferDuration; } set { _bwp.BufferDuration = value; } }

        public override long Length => (long)(_bwp.BufferDuration.TotalSeconds * WaveFormat.AverageBytesPerSecond);
        public override long Position { get { return (long)(_bwp.BufferedDuration.TotalSeconds * WaveFormat.AverageBytesPerSecond); } set { throw new NotSupportedException(); } }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _th.IsAlive) _th.Abort();
            base.Dispose(disposing);
        }
    }
}
