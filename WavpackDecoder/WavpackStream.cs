/*
** Wavpackcs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

using System;
using System.Collections.Generic;

namespace WavpackDecoder
{
    internal class WavpackStream
    {
        public WavpackStream()
        {
            InitBlock();
        }

        private readonly DecorrPass _dp1 = new DecorrPass();
        private readonly DecorrPass _dp10 = new DecorrPass();
        private readonly DecorrPass _dp11 = new DecorrPass();
        private readonly DecorrPass _dp12 = new DecorrPass();
        private readonly DecorrPass _dp13 = new DecorrPass();
        private readonly DecorrPass _dp14 = new DecorrPass();
        private readonly DecorrPass _dp15 = new DecorrPass();
        private readonly DecorrPass _dp16 = new DecorrPass();
        private readonly DecorrPass _dp2 = new DecorrPass();
        private readonly DecorrPass _dp3 = new DecorrPass();
        private readonly DecorrPass _dp4 = new DecorrPass();
        private readonly DecorrPass _dp5 = new DecorrPass();
        private readonly DecorrPass _dp6 = new DecorrPass();
        private readonly DecorrPass _dp7 = new DecorrPass();
        private readonly DecorrPass _dp8 = new DecorrPass();
        private readonly DecorrPass _dp9 = new DecorrPass();

        internal DecorrPass[] DecorrPasses;
        internal short FloatFlags, FloatShift, FloatMaxExp, FloatNormExp; // was uchar in C

        internal short Int32SentBits, Int32Zeros, Int32Ones, Int32Dups; // was uchar in C
        internal int MuteError;

        internal int NumTerms = 0;
        internal long SampleIndex, Crc; // was uint32_t in C

        internal WordsData W = new WordsData();
        internal WavpackHeader Wphdr = new WavpackHeader();
        internal BitStream WvBits = new BitStream();

        private void InitBlock()
        {
            DecorrPasses = new[]
            {
                _dp1, _dp2, _dp3, _dp4, _dp5, _dp6, _dp7, _dp8, _dp9, _dp10, _dp11, _dp12, _dp13, _dp14, _dp15, _dp16
            };
        }

        // This monster actually unpacks the WavPack bitstream(s) into the specified
        // buffer as 32-bit integers or floats (depending on orignal data). Lossy
        // samples will be clipped to their original limits (i.e. 8-bit samples are
        // clipped to -128/+127) but are still returned in ints. It is up to the
        // caller to potentially reformat this for the final output including any
        // multichannel distribution, block alignment or endian compensation. The
        // function unpack_init() must have been called and the entire WavPack block
        // must still be visible (although blockbuff will not be accessed again).
        // For maximum clarity, the function is broken up into segments that handle
        // various modes. This makes for a few extra infrequent flag checks, but
        // makes the code easier to follow because the nesting does not become so
        // deep. For maximum efficiency, the conversion is isolated to tight loops
        // that handle an entire buffer. The function returns the total number of
        // samples unpacked, which can be less than the number requested if an error
        // occurs or the end of the block is reached.

        internal long UnpackSamples(int[] buffer, long sampleCount)
        {
            var flags = Wphdr.Flags;
            long i;
            var crc = (int)Crc;

            var muteLimit = (int)((1L << (int)((flags & Defines.MAG_MASK) >> Defines.MAG_LSB)) + 2);
            DecorrPass dpp;
            int tcount;
            var bufferCounter = 0;
            var tempBuffer = new int[Defines.SAMPLE_BUFFER_SIZE];

            if (SampleIndex + sampleCount > Wphdr.BlockIndex + Wphdr.BlockSamples)
                sampleCount = Wphdr.BlockIndex + Wphdr.BlockSamples - SampleIndex;

            if (MuteError > 0)
            {
                long tempc;

                if ((flags & Defines.MONO_FLAG) > 0) tempc = sampleCount;
                else tempc = 2 * sampleCount;

                while (tempc > 0)
                {
                    buffer[bufferCounter] = 0;
                    tempc--;
                    bufferCounter++;
                }

                SampleIndex += sampleCount;

                return sampleCount;
            }

            if ((flags & Defines.HYBRID_FLAG) > 0)
                muteLimit *= 2;

            ///////////////////// handle version 4 mono data /////////////////////////

            if ((flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) > 0)
            {
                var dppIndex = 0;

                i = WordsUtils.GetWords(sampleCount, flags, W, WvBits, tempBuffer);

                Array.Copy(tempBuffer, 0, buffer, 0, (int)sampleCount);

                for (tcount = NumTerms - 1; tcount >= 0; tcount--)
                {
                    dpp = DecorrPasses[dppIndex];
                    dpp.MonoPass(buffer, sampleCount, bufferCounter);
                    dppIndex++;
                }

                for (var q = 0; q < sampleCount; q++)
                {
                    var bfAbs = buffer[bufferCounter] < 0 ? -buffer[bufferCounter] : buffer[bufferCounter];

                    if (bfAbs > muteLimit)
                    {
                        i = q;
                        break;
                    }

                    crc = crc * 3 + buffer[q];
                }
            }
            //////////////////// handle version 4 stereo data ////////////////////////
            else
            {
                var samplesProcessed = WordsUtils.GetWords(sampleCount, flags, W, WvBits, tempBuffer);

                i = samplesProcessed;

                Array.Copy(tempBuffer, 0, buffer, 0, (int)(sampleCount * 2));

                if (sampleCount < 16)
                {
                    var dppIndex = 0;

                    for (tcount = NumTerms - 1; tcount >= 0; tcount--)
                    {
                        dpp = DecorrPasses[dppIndex];
                        dpp.StereoPass(buffer, sampleCount, bufferCounter);
                        DecorrPasses[dppIndex] = dpp;
                        dppIndex++;
                    }
                }
                else
                {
                    var dppIndex = 0;

                    for (tcount = NumTerms - 1; tcount >= 0; tcount--)
                    {
                        dpp = DecorrPasses[dppIndex];

                        dpp.StereoPass(buffer, 8, bufferCounter);

                        dpp.StereoPassCont(buffer, sampleCount - 8, bufferCounter + 16);

                        DecorrPasses[dppIndex] = dpp;

                        dppIndex++;
                    }
                }

                if ((flags & Defines.JOINT_STEREO) > 0)
                    for (bufferCounter = 0; bufferCounter < sampleCount * 2; bufferCounter += 2)
                    {
                        buffer[bufferCounter] += buffer[bufferCounter + 1] -= buffer[bufferCounter] >> 1;

                        var bfAbs = buffer[bufferCounter] < 0 ? -buffer[bufferCounter] : buffer[bufferCounter];
                        var bf1Abs = buffer[bufferCounter + 1] < 0
                                         ? -buffer[bufferCounter + 1]
                                         : buffer[bufferCounter + 1];

                        if (bfAbs > muteLimit || bf1Abs > muteLimit)
                        {
                            i = bufferCounter / 2;
                            break;
                        }

                        crc = (crc * 3 + buffer[bufferCounter]) * 3 + buffer[bufferCounter + 1];
                    }
                else
                    for (bufferCounter = 0; bufferCounter < sampleCount * 2; bufferCounter += 2)
                    {
                        var bfAbs = buffer[bufferCounter] < 0 ? -buffer[bufferCounter] : buffer[bufferCounter];
                        var bf1Abs = buffer[bufferCounter + 1] < 0
                                         ? -buffer[bufferCounter + 1]
                                         : buffer[bufferCounter + 1];

                        if (bfAbs > muteLimit || bf1Abs > muteLimit)
                        {
                            i = bufferCounter / 2;
                            break;
                        }

                        crc = (crc * 3 + buffer[bufferCounter]) * 3 + buffer[bufferCounter + 1];
                    }
            }

            if (i != sampleCount)
            {
                var sc = sampleCount;
                if ((flags & Defines.MONO_FLAG) <= 0) sc *= 2;

                bufferCounter = 0;

                while (sc > 0)
                {
                    buffer[bufferCounter] = 0;
                    sc--;
                    bufferCounter++;
                }

                MuteError = 1;
                i = sampleCount;
            }

            FixupSamples(buffer, i);

            if ((flags & Defines.FALSE_STEREO) > 0)
            {
                var destIdx = (int)i * 2;
                var srcIdx = (int)i;
                var c = (int)i;

                destIdx--;
                srcIdx--;

                while (c > 0)
                {
                    buffer[destIdx] = buffer[srcIdx];
                    destIdx--;
                    buffer[destIdx] = buffer[srcIdx];
                    destIdx--;
                    srcIdx--;
                    c--;
                }
            }

            SampleIndex += i;
            Crc = crc;

            return i;
        }

        // This is a helper function for unpack_samples() that applies several final
        // operations. First, if the data is 32-bit float data, then that conversion
        // is done in the float.c module (whether lossy or lossless) and we return.
        // Otherwise, if the extended integer data applies, then that operation is
        // executed first. If the unpacked data is lossy (and not corrected) then
        // it is clipped and shifted in a single operation. Otherwise, if it's
        // lossless then the last step is to apply the final shift (if any).

        private void FixupSamples(int[] buffer, long sampleCount)
        {
            var flags = Wphdr.Flags;
            var shift = (int)((flags & Defines.SHIFT_MASK) >> Defines.SHIFT_LSB);

            if ((flags & Defines.FLOAT_DATA) > 0)
            {
                var sc = sampleCount;
                if ((flags & Defines.MONO_FLAG) <= 0) sc *= 2;

                FloatValues(buffer, sc);
            }

            if ((flags & Defines.INT32_DATA) > 0)
            {
                int sentBits = Int32SentBits, zeros = Int32Zeros;
                int ones = Int32Ones, dups = Int32Dups;
                var bufferCounter = 0;

                var count = sampleCount;
                if ((flags & Defines.MONO_FLAG) <= 0) count *= 2;

                if ((flags & Defines.HYBRID_FLAG) == 0 && sentBits == 0 && zeros + ones + dups != 0)
                    while (count > 0)
                    {
                        if (zeros != 0)
                            buffer[bufferCounter] <<= zeros;
                        else if (ones != 0)
                            buffer[bufferCounter] = ((buffer[bufferCounter] + 1) << ones) - 1;
                        else if (dups != 0)
                            buffer[bufferCounter] = ((buffer[bufferCounter] + (buffer[bufferCounter] & 1)) << dups)
                                                    - (buffer[bufferCounter] & 1);

                        bufferCounter++;
                        count--;
                    }
                else
                    shift += zeros + sentBits + ones + dups;
            }

            if ((flags & Defines.HYBRID_FLAG) > 0)
            {
                int minValue, maxValue, minShifted, maxShifted;
                var bufferCounter = 0;

                switch ((int)(flags & Defines.BYTES_STORED))
                {
                    case 0:
                        minShifted = (minValue = -128 >> shift) << shift;
                        maxShifted = (maxValue = 127 >> shift) << shift;
                        break;

                    case 1:
                        minShifted = (minValue = -32768 >> shift) << shift;
                        maxShifted = (maxValue = 32767 >> shift) << shift;
                        break;

                    case 2:
                        minShifted = (minValue = -8388608 >> shift) << shift;
                        maxShifted = (maxValue = 8388607 >> shift) << shift;
                        break;

                    default:
                        minShifted = (minValue = unchecked((int)0x80000000L) >> shift) << shift;
                        maxShifted = (maxValue = 0x7FFFFFFF >> shift) << shift;
                        break;
                }

                if ((flags & Defines.MONO_FLAG) == 0)
                    sampleCount *= 2;

                while (sampleCount > 0)
                {
                    if (buffer[bufferCounter] < minValue)
                        buffer[bufferCounter] = minShifted;
                    else if (buffer[bufferCounter] > maxValue)
                        buffer[bufferCounter] = maxShifted;
                    else
                        buffer[bufferCounter] <<= shift;

                    bufferCounter++;
                    sampleCount--;
                }
            }
            else if (shift != 0)
            {
                var bufferCounter = 0;

                if ((flags & Defines.MONO_FLAG) == 0)
                    sampleCount *= 2;

                while (sampleCount > 0)
                {
                    buffer[bufferCounter] = buffer[bufferCounter] << shift;
                    bufferCounter++;
                    sampleCount--;
                }
            }
        }

        private void FloatValues(IList<int> values, long numValues)
        {
            var shift = FloatMaxExp - FloatNormExp + FloatShift;
            var valueCounter = 0;

            if (shift > 32)
                shift = 32;
            else if (shift < -32)
                shift = -32;

            while (numValues > 0)
            {
                if (shift > 0)
                    values[valueCounter] <<= shift;
                else if (shift < 0)
                    values[valueCounter] >>= -shift;

                if (values[valueCounter] > 8388607L)
                    values[valueCounter] = (int)8388607L;
                else if (values[valueCounter] < -8388608L)
                    values[valueCounter] = (int)-8388608L;

                valueCounter++;
                numValues--;
            }
        }
    }
}
