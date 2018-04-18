/*
** WavpackContext.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

using System;
using System.IO;

namespace WavpackDecoder
{
    public class WavpackContext
    {
        // This function reads data from the specified stream in search of a valid
        // WavPack 4.0 audio block. If this fails in 1 megabyte (or an invalid or
        // unsupported WavPack block is encountered) then an appropriate message is
        // copied to "error" and NULL is returned, otherwise a pointer to a
        // WavpackContext structure is returned (which is used to call all other
        // functions in this module). This can be initiated at the beginning of a
        // WavPack file, or anywhere inside a WavPack file. To determine the exact
        // position within the file use WavpackGetSampleIndex().  Also,
        // this function will not handle "correction" files, plays only the first
        // two channels of multi-channel files, and is limited in resolution in some
        // large integer or floating point files (but always provides at least 24 bits
        // of resolution).

        public WavpackContext(BinaryReader infile)
        {
            InFile = infile;
            TotalSamples = -1;

            // open the source file for reading and store the size

            while (Stream.Wphdr.BlockSamples == 0)
            {
                Stream.Wphdr = ReadNextHeader(Stream.Wphdr);

                if (Stream.Wphdr.Status == 1)
                {
                    ErrorMessage = "not compatible with this version of WavPack file!";
                    Error = true;
                    return;
                }

                if (Stream.Wphdr.BlockSamples > 0 && Stream.Wphdr.TotalSamples != -1)
                    TotalSamples = Stream.Wphdr.TotalSamples;
                
                if (!UnpackInit())
                {
                    Error = true;
                    return;
                }
            } // end of while

            Config.Flags = Config.Flags & ~ 0xff;
            Config.Flags = Config.Flags | (Stream.Wphdr.Flags & 0xff);

            Config.BytesPerSample = (int)((Stream.Wphdr.Flags & Defines.BYTES_STORED) + 1);
            Config.FloatNormExp = Stream.FloatNormExp;

            Config.BitsPerSample = (int)(Config.BytesPerSample * 8
                                         - ((Stream.Wphdr.Flags & Defines.SHIFT_MASK) >> Defines.SHIFT_LSB));

            if ((Config.Flags & Defines.FLOAT_DATA) > 0)
            {
                Config.BytesPerSample = 3;
                Config.BitsPerSample = 24;
            }

            if (Config.SampleRate == 0)
            {
                if (Stream.Wphdr.BlockSamples == 0 || (Stream.Wphdr.Flags & Defines.SRATE_MASK) == Defines.SRATE_MASK)
                    Config.SampleRate = 44100;
                else
                    Config.SampleRate =
                        SampleRates[(int)((Stream.Wphdr.Flags & Defines.SRATE_MASK) >> Defines.SRATE_LSB)];
            }

            if (Config.NumChannels == 0)
            {
                Config.NumChannels = (Stream.Wphdr.Flags & Defines.MONO_FLAG) > 0 ? 1 : 2;

                Config.ChannelMask = 0x5 - Config.NumChannels;
            }

            if ((Stream.Wphdr.Flags & Defines.FINAL_BLOCK) == 0)
                _reducedChannels = (Stream.Wphdr.Flags & Defines.MONO_FLAG) != 0 ? 1 : 2;
        }

        private static readonly long[] SampleRates =
            { 6000, 8000, 9600, 11025, 12000, 16000, 22050, 24000, 32000, 44100, 48000, 64000, 88200, 96000, 192000 };

        private readonly int _reducedChannels;

        internal readonly WavpackConfig Config = new WavpackConfig();

        internal readonly BinaryReader InFile;

        internal readonly byte[] ReadBuffer = new byte[1024]; // was uchar in C
        internal WavpackStream Stream = new WavpackStream();

        public long SampleIndex => Stream.SampleIndex;
        public long SampleRate => Config.SampleRate.NonZeroOr(44100);
        public int NumChannels => Config.NumChannels.NonZeroOr(2);
        public int BitsPerSample => Config.BitsPerSample.NonZeroOr(16);
        public int BytesPerSample => Config.BytesPerSample.NonZeroOr(2);
        public int ReducedChannels => _reducedChannels.NonZeroOr(NumChannels);
        public string ErrorMessage { get; private set; } = "";
        public bool Error { get; private set; }
        public long TotalSamples { get; } // was uint32_t in C
        public long CrcErrors { get; private set; } // was uint32_t in C

        public int LossyBlocks { get; internal set; }
        ///////////////////////////// executable code ////////////////////////////////

        // This function initializes everything required to unpack a WavPack block
        // and must be called before unpack_samples() is called to obtain audio data.
        // It is assumed that the WavpackHeader has been read into the Stream.wphdr
        // (in the current WavpackStream). This is where all the metadata blocks are
        // scanned up to the one containing the audio bitstream.

        private bool UnpackInit()
        {
            var wpmd = new WavpackMetadata();

            if (Stream.Wphdr.BlockSamples > 0 && Stream.Wphdr.BlockIndex != -1)
                Stream.SampleIndex = Stream.Wphdr.BlockIndex;

            Stream.MuteError = 0;
            Stream.Crc = 0xffffffff;
            Stream.WvBits.Sr = 0;

            while (MetadataUtils.ReadMetadataBuff(this, wpmd))
            {
                if (!MetadataUtils.ProcessMetadata(this, wpmd))
                {
                    Error = true;
                    ErrorMessage = "invalid metadata!";
                    return false;
                }

                if (wpmd.Id == Defines.ID_WV_BITSTREAM)
                    break;
            }

            if (Stream.Wphdr.BlockSamples != 0 && null == Stream.WvBits.File)
            {
                ErrorMessage = "invalid WavPack file!";
                Error = true;
                return false;
            }

            if (Stream.Wphdr.BlockSamples != 0)
            {
                if ((Stream.Wphdr.Flags & Defines.INT32_DATA) != 0 && Stream.Int32SentBits != 0)
                    LossyBlocks = 1;

                if ((Stream.Wphdr.Flags & Defines.FLOAT_DATA) != 0
                    && (Stream.FloatFlags & (Defines.FLOAT_EXCEPTIONS | Defines.FLOAT_ZEROS_SENT
                                                                      | Defines.FLOAT_SHIFT_SENT
                                                                      | Defines.FLOAT_SHIFT_SAME)) != 0)
                    LossyBlocks = 1;
            }

            Error = false;
            return true;
        }

        // This function checks the crc value(s) for an unpacked block, returning the
        // number of actual crc errors detected for the block. The block must be
        // completely unpacked before this test is valid. For losslessly unpacked
        // blocks of float or extended integer data the extended crc is also checked.
        // Note that WavPack's crc is not a CCITT approved polynomial algorithm, but
        // is a much simpler method that is virtually as robust for real world data.

        private int CheckCrcError()
        {
            var result = 0;

            if (Stream.Crc != Stream.Wphdr.Crc) ++result;

            return result;
        }

        // This function obtains general information about an open file and returns
        // a mask with the following bit values:

        // MODE_LOSSLESS:  file is lossless (pure lossless only)
        // MODE_HYBRID:  file is hybrid mode (lossy part only)
        // MODE_FLOAT:  audio data is 32-bit ieee floating point (but will provided
        //               in 24-bit integers for convenience)
        // MODE_HIGH:  file was created in "high" mode (information only)
        // MODE_FAST:  file was created in "fast" mode (information only)

        public int GetMode()
        {
            var mode = 0;

            if ((Config.Flags & Defines.CONFIG_HYBRID_FLAG) != 0)
                mode |= Defines.MODE_HYBRID;
            else if ((Config.Flags & Defines.CONFIG_LOSSY_MODE) == 0)
                mode |= Defines.MODE_LOSSLESS;

            if (LossyBlocks != 0)
                mode &= ~ Defines.MODE_LOSSLESS;

            if ((Config.Flags & Defines.CONFIG_FLOAT_DATA) != 0)
                mode |= Defines.MODE_FLOAT;

            if ((Config.Flags & Defines.CONFIG_HIGH_FLAG) != 0)
                mode |= Defines.MODE_HIGH;

            if ((Config.Flags & Defines.CONFIG_FAST_FLAG) != 0)
                mode |= Defines.MODE_FAST;

            return mode;
        }

        // Unpack the specified number of samples from the current file position.
        // Note that "samples" here refers to "complete" samples, which would be
        // 2 longs for stereo files. The audio data is returned right-justified in
        // 32-bit longs in the endian mode native to the executing processor. So,
        // if the original data was 16-bit, then the values returned would be
        // +/-32k. Floating point data will be returned as 24-bit integers (and may
        // also be clipped). The actual number of samples unpacked is returned,
        // which should be equal to the number requested unless the end of fle is
        // encountered or an error occurs.

        public long UnpackSamples(int[] buffer, long samples)
        {
            var samplesUnpacked = 0L;
            var numChannels = Config.NumChannels;
            var bcounter = 0;

            var tempBuffer = new int[Defines.SAMPLE_BUFFER_SIZE];
            var bufIdx = 0;

            while (samples > 0)
            {
                if (Stream.Wphdr.BlockSamples == 0 || (Stream.Wphdr.Flags & Defines.INITIAL_BLOCK) == 0
                                                   || Stream.SampleIndex >= Stream.Wphdr.BlockIndex
                                                   + Stream.Wphdr.BlockSamples)
                {
                    Stream.Wphdr = ReadNextHeader(Stream.Wphdr);

                    if (Stream.Wphdr.Status == 1)
                        break;

                    if (Stream.Wphdr.BlockSamples == 0 || Stream.SampleIndex == Stream.Wphdr.BlockIndex)
                        if (!UnpackInit())
                            break;
                }

                if (Stream.Wphdr.BlockSamples == 0 || (Stream.Wphdr.Flags & Defines.INITIAL_BLOCK) == 0
                                                   || Stream.SampleIndex >= Stream.Wphdr.BlockIndex
                                                   + Stream.Wphdr.BlockSamples)
                    continue;

                long samplesToUnpack;
                if (Stream.SampleIndex < Stream.Wphdr.BlockIndex)
                {
                    samplesToUnpack = Stream.Wphdr.BlockIndex - Stream.SampleIndex;

                    if (samplesToUnpack > samples)
                        samplesToUnpack = samples;

                    Stream.SampleIndex += samplesToUnpack;
                    samplesUnpacked += samplesToUnpack;
                    samples -= samplesToUnpack;

                    if (_reducedChannels > 0)
                        samplesToUnpack *= _reducedChannels;
                    else
                        samplesToUnpack *= numChannels;

                    while (samplesToUnpack > 0)
                    {
                        tempBuffer[bcounter] = 0;
                        bcounter++;
                        samplesToUnpack--;
                    }

                    continue;
                }

                samplesToUnpack = Stream.Wphdr.BlockIndex + Stream.Wphdr.BlockSamples - Stream.SampleIndex;

                if (samplesToUnpack > samples)
                    samplesToUnpack = samples;

                Stream.UnpackSamples(tempBuffer, samplesToUnpack);

                int bytesReturned;
                if (_reducedChannels > 0)
                    bytesReturned = (int)(samplesToUnpack * _reducedChannels);
                else
                    bytesReturned = (int)(samplesToUnpack * numChannels);

                Array.Copy(tempBuffer, 0, buffer, bufIdx, bytesReturned);

                bufIdx += bytesReturned;

                samplesUnpacked += samplesToUnpack;
                samples -= samplesToUnpack;

                if (Stream.SampleIndex == Stream.Wphdr.BlockIndex + Stream.Wphdr.BlockSamples)
                    if (CheckCrcError() > 0)
                        CrcErrors++;

                if (Stream.SampleIndex == TotalSamples)
                    break;
            }

            return samplesUnpacked;
        }

        // The following seek functionality has not yet been extensively tested

        public void SetTime(long milliseconds)
        {
            var targetSample = milliseconds / 1000 * Config.SampleRate;
            try { Seek(InFile.BaseStream.Position, targetSample); }
            catch (IOException) { }
        }

        public void SetSample(long sample)
        {
            Seek(0, sample);
        }

        // Find the WavPack block that contains the specified sample. If "header_pos"
        // is zero, then no information is assumed except the total number of samples
        // in the file and its size in bytes. If "header_pos" is non-zero then we
        // assume that it is the file position of the valid header image contained in
        // the first stream and we can limit our search to either the portion above
        // or below that point. If a .wvc file is being used, then this must be called
        // for that file also.
        private void Seek(long headerPos, long targetSample)
        {
            try
            {
                long filePos1 = 0;
                var filePos2 = InFile.BaseStream.Length;
                long samplePos1 = 0, samplePos2 = TotalSamples;
                var ratio = 0.96;
                var fileSkip = 0;
                if (targetSample >= TotalSamples)
                    return;
                if (headerPos > 0 && Stream.Wphdr.BlockSamples > 0)
                {
                    if (Stream.Wphdr.BlockIndex > targetSample)
                    {
                        samplePos2 = Stream.Wphdr.BlockIndex;
                        filePos2 = headerPos;
                    }
                    else if (Stream.Wphdr.BlockIndex + Stream.Wphdr.BlockSamples <= targetSample)
                    {
                        samplePos1 = Stream.Wphdr.BlockIndex;
                        filePos1 = headerPos;
                    }
                    else return;
                }

                while (true)
                {
                    double bytesPerSample = filePos2 - filePos1;
                    bytesPerSample /= samplePos2 - samplePos1;
                    var seekPos = filePos1 + (fileSkip > 0 ? 32 : 0);
                    seekPos += (long)(bytesPerSample * (targetSample - samplePos1) * ratio);
                    InFile.BaseStream.Seek(seekPos, 0);

                    //var temppos = InFile.BaseStream.Position;
                    Stream.Wphdr = ReadNextHeader(Stream.Wphdr);

                    if (Stream.Wphdr.Status == 1 || seekPos >= filePos2)
                    {
                        if (ratio > 0.0)
                        {
                            if ((ratio -= 0.24) < 0.0)
                                ratio = 0.0;
                        }
                        else return;
                    }
                    else if (Stream.Wphdr.BlockIndex > targetSample)
                    {
                        samplePos2 = Stream.Wphdr.BlockIndex;
                        filePos2 = seekPos;
                    }
                    else if (Stream.Wphdr.BlockIndex + Stream.Wphdr.BlockSamples <= targetSample)
                    {
                        if (seekPos == filePos1) { fileSkip = 1; }
                        else
                        {
                            samplePos1 = Stream.Wphdr.BlockIndex;
                            filePos1 = seekPos;
                        }
                    }
                    else
                    {
                        var index = (int)(targetSample - Stream.Wphdr.BlockIndex);
                        InFile.BaseStream.Seek(seekPos, 0);
                        var c = new WavpackContext(InFile);
                        Stream = c.Stream;
                        var tempBuf = new int[Defines.SAMPLE_BUFFER_SIZE];
                        while (index > 0)
                        {
                            var toUnpack = Math.Min(index, Defines.SAMPLE_BUFFER_SIZE / ReducedChannels);
                            UnpackSamples(tempBuf, toUnpack);
                            index = index - toUnpack;
                        }

                        return;
                    }
                }
            }
            catch (IOException) { }
        }

        // Read from current file position until a valid 32-byte WavPack 4.0 header is
        // found and read into the specified pointer. If no WavPack header is found within 1 meg,
        // then an error is returned. No additional bytes are read past the header. 

        private WavpackHeader ReadNextHeader(WavpackHeader wphdr)
        {
            var buffer = new byte[32]; // 32 is the size of a WavPack Header
            var temp = new byte[32];

            long bytesSkipped = 0;
            var bleft = 0; // bytes left in buffer

            while (true)
            {
                int i;
                for (i = 0; i < bleft; i++) buffer[i] = buffer[32 - bleft + i];

                var counter = 0;

                try
                {
                    if (InFile.BaseStream.Read(temp, 0, 32 - bleft) != 32 - bleft)
                    {
                        wphdr.Status = 1;
                        return wphdr;
                    }
                }
                catch (Exception)
                {
                    wphdr.Status = 1;
                    return wphdr;
                }

                for (i = 0; i < 32 - bleft; i++) buffer[bleft + i] = temp[i];

                bleft = 32;

                if (buffer[0] == 'w' && buffer[1] == 'v' && buffer[2] == 'p' && buffer[3] == 'k' && (buffer[4] & 1) == 0
                    && buffer[6] < 16 && buffer[7] == 0 && buffer[9] == 4
                    && buffer[8] >= (Defines.MIN_STREAM_VERS & 0xff) && buffer[8] <= (Defines.MAX_STREAM_VERS & 0xff))
                {
                    wphdr.CkId[0] = 'w';
                    wphdr.CkId[1] = 'v';
                    wphdr.CkId[2] = 'p';
                    wphdr.CkId[3] = 'k';

                    wphdr.CkSize = (buffer[7] & 0xFF) << 24;
                    wphdr.CkSize += (buffer[6] & 0xFF) << 16;
                    wphdr.CkSize += (buffer[5] & 0xFF) << 8;
                    wphdr.CkSize += buffer[4] & 0xFF;

                    wphdr.Version = (short)(buffer[9] << 8);
                    wphdr.Version = (short)(wphdr.Version + buffer[8]);

                    wphdr.TrackNo = buffer[10];
                    wphdr.IndexNo = buffer[11];

                    wphdr.TotalSamples = (buffer[15] & 0xFF) << 24;
                    wphdr.TotalSamples += (buffer[14] & 0xFF) << 16;
                    wphdr.TotalSamples += (buffer[13] & 0xFF) << 8;
                    wphdr.TotalSamples += buffer[12] & 0xFF;

                    wphdr.BlockIndex = (buffer[19] & 0xFF) << 24;
                    wphdr.BlockIndex += (buffer[18] & 0xFF) << 16;
                    wphdr.BlockIndex += (buffer[17] & 0xFF) << 8;
                    wphdr.BlockIndex += (long)buffer[16] & 0xFF;

                    wphdr.BlockSamples = (buffer[23] & 0xFF) << 24;
                    wphdr.BlockSamples += (buffer[22] & 0xFF) << 16;
                    wphdr.BlockSamples += (buffer[21] & 0xFF) << 8;
                    wphdr.BlockSamples += buffer[20] & 0xFF;

                    wphdr.Flags = (buffer[27] & 0xFF) << 24;
                    wphdr.Flags += (buffer[26] & 0xFF) << 16;
                    wphdr.Flags += (buffer[25] & 0xFF) << 8;
                    wphdr.Flags += buffer[24] & 0xFF;

                    wphdr.Crc = (buffer[31] & 0xFF) << 24;
                    wphdr.Crc += (buffer[30] & 0xFF) << 16;
                    wphdr.Crc += (buffer[29] & 0xFF) << 8;
                    wphdr.Crc += buffer[28] & 0xFF;

                    wphdr.Status = 0;

                    return wphdr;
                }

                counter++;
                bleft--;

                while (bleft > 0 && buffer[counter] != 'w')
                {
                    counter++;
                    bleft--;
                }

                bytesSkipped = bytesSkipped + counter;

                if (bytesSkipped > 1048576L)
                {
                    wphdr.Status = 1;
                    return wphdr;
                }
            }
        }
    }
}
