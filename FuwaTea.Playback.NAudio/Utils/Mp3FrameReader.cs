using System;
using System.IO;
using log4net;
using NAudio.Wave;

namespace FuwaTea.Playback.NAudio.Utils
{
    public class Mp3FrameReader : WaveStream
    {
        public Mp3FrameReader(Stream src)
        {
            _src = src;
            Read(new byte[8], 0, 0);
            
        }

        private readonly Stream _src;
        private IMp3FrameDecompressor _decompressor;
        private byte[] _remainingBytes;

        public override sealed int Read(byte[] buffer, int offset, int count)
        {
            var readBytes = 0;
            do
            {
                if (_remainingBytes != null && readBytes == 0)
                {
                    Array.Copy(_remainingBytes, 0, buffer, offset, _remainingBytes.Length);
                    readBytes += _remainingBytes.Length;
                    continue;
                }
                Mp3Frame frame;
                try { frame = Mp3Frame.LoadFromStream(_src); }
                catch (Exception e)
                {
                    LogManager.GetLogger(GetType()).Debug("Read failed:", e);
                    frame = null;
                }
                if (frame == null)
                {
                    LogManager.GetLogger(GetType()).Debug("Null frame");
                    break;
                }
                if (_decompressor == null) _decompressor = CreateFrameDecompressor(frame);
                var frameBuffer = new byte[16384 * 4];
                int decompressed;
                try { decompressed = _decompressor.DecompressFrame(frame, frameBuffer, 0); }
                catch (Exception e)
                {
                    LogManager.GetLogger(GetType()).Debug("Decompress failed:", e);
                    break;
                }
                Array.Copy(frameBuffer, 0, buffer, offset + readBytes, count - readBytes >= decompressed ? decompressed : count - readBytes);
                if (count - readBytes < decompressed)
                {
                    _remainingBytes = new byte[decompressed - (count - readBytes)];
                    Array.Copy(frameBuffer, count - readBytes, _remainingBytes, 0, _remainingBytes.Length);
                }
                readBytes += decompressed;
            } while (readBytes < count);
            return readBytes >= count ? count : readBytes;
        }
        
        public override WaveFormat WaveFormat => _decompressor?.OutputFormat;

        public override long Length => 0;
        public override TimeSpan TotalTime => TimeSpan.Zero;
        public override long Position { get { return 0; } set { throw new NotSupportedException(); } }
        public override TimeSpan CurrentTime { get { return TimeSpan.Zero; } set { } }

        private static IMp3FrameDecompressor CreateFrameDecompressor(Mp3Frame frame)
        {
            WaveFormat waveFormat = new Mp3WaveFormat(frame.SampleRate, frame.ChannelMode == ChannelMode.Mono ? 1 : 2,
                frame.FrameLength, frame.BitRate);
            return new AcmMp3FrameDecompressor(waveFormat);
        }
    }
}
