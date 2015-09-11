using System;
using System.Threading;
using log4net;
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
        private readonly Thread _th;

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _bwp.Read(buffer, offset, count);
        }
        
        private void FillBuffer()
        {
            while (true)
            {
                // If buffer is filled enough, wait.
                if (_bwp.BufferedBytes > _bwp.BufferLength / 100 * RefillAtPercent) { Thread.Sleep(500); continue; }
                // Try to fill the buffer completely
                var bytesToFill = _bwp.BufferLength - _bwp.BufferedBytes;
                // Do it in RefillBatches
                for (var i = 0; i < RefillBatches; i++)
                {
                    var buffer = new byte[bytesToFill / RefillBatches];
                    var read = _src.Read(buffer, 0, bytesToFill / RefillBatches);
                    _bwp.AddSamples(buffer, 0, read);
                }
            }
        }

        public override WaveFormat WaveFormat => _src.WaveFormat;

        public TimeSpan BufferDuration { get { return _bwp.BufferDuration; } set { _bwp.BufferDuration = value; } }
        public int RefillAtPercent { get; set; } = 33;
        public int RefillBatches { get; set; } = 3;

        public override long Length => (long)(_bwp.BufferDuration.TotalSeconds * WaveFormat.AverageBytesPerSecond);
        public override long Position { get { return (long)(_bwp.BufferedDuration.TotalSeconds * WaveFormat.AverageBytesPerSecond); } set { LogManager.GetLogger(GetType()).Warn("Attempt to change position!"); } }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _th.Abort();
            base.Dispose(disposing);
        }
    }
}
