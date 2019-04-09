/*
** WordsUtils.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

namespace Sage.Audio.Decoders.Wavpack
{
    internal static class WordsUtils
    {
        //////////////////////////////// local macros /////////////////////////////////

        private const int LimitOnes = 16; // maximum consecutive 1s sent for "div" data

        // these control the time constant "slow_level" which is used for hybrid mode
        // that controls bitrate as a function of residual level (HYBRID_BITRATE).
        internal const int Sls = 8;
        internal const int Slo = 1 << (Sls - 1);

        // these control the time constant of the 3 median level breakpoints
        private const int Div0 = 128; // 5/7 of samples
        private const int Div1 = 64; // 10/49 of samples
        private const int Div2 = 32; // 20/343 of samples

        ///////////////////////////// local table storage ////////////////////////////

        private static readonly int[] NbitsTable =
        {
            0, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, // 0 - 15
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, // 16 - 31
            6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, // 32 - 47
            6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, // 48 - 63
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, // 64 - 79
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, // 80 - 95
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, // 96 - 111
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, // 112 - 127
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, // 128 - 143
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, // 144 - 159
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, // 160 - 175
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, // 176 - 191
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, // 192 - 207
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, // 208 - 223
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, // 224 - 239
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 // 240 - 255
        };

        private static readonly int[] Log2Table =
        {
            0x00, 0x01, 0x03, 0x04, 0x06, 0x07, 0x09, 0x0a, 0x0b, 0x0d, 0x0e, 0x10, 0x11, 0x12, 0x14, 0x15, 0x16, 0x18,
            0x19, 0x1a, 0x1c, 0x1d, 0x1e, 0x20, 0x21, 0x22, 0x24, 0x25, 0x26, 0x28, 0x29, 0x2a, 0x2c, 0x2d, 0x2e, 0x2f,
            0x31, 0x32, 0x33, 0x34, 0x36, 0x37, 0x38, 0x39, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x41, 0x42, 0x43, 0x44, 0x45,
            0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x52, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5a,
            0x5c, 0x5d, 0x5e, 0x5f, 0x60, 0x61, 0x62, 0x63, 0x64, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e,
            0x6f, 0x70, 0x71, 0x72, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f, 0x80, 0x81,
            0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c, 0x8d, 0x8e, 0x8f, 0x90, 0x91, 0x92, 0x93,
            0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9b, 0x9c, 0x9d, 0x9e, 0x9f, 0xa0, 0xa1, 0xa2, 0xa3, 0xa4,
            0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xae, 0xaf, 0xb0, 0xb1, 0xb2, 0xb2, 0xb3, 0xb4,
            0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 0xbe, 0xbf, 0xc0, 0xc0, 0xc1, 0xc2, 0xc3, 0xc4,
            0xc5, 0xc6, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xcb, 0xcb, 0xcc, 0xcd, 0xce, 0xcf, 0xd0, 0xd0, 0xd1, 0xd2, 0xd3,
            0xd4, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd8, 0xd9, 0xda, 0xdb, 0xdc, 0xdc, 0xdd, 0xde, 0xdf, 0xe0, 0xe0, 0xe1,
            0xe2, 0xe3, 0xe4, 0xe4, 0xe5, 0xe6, 0xe7, 0xe7, 0xe8, 0xe9, 0xea, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xee, 0xef,
            0xf0, 0xf1, 0xf1, 0xf2, 0xf3, 0xf4, 0xf4, 0xf5, 0xf6, 0xf7, 0xf7, 0xf8, 0xf9, 0xf9, 0xfa, 0xfb, 0xfc, 0xfc,
            0xfd, 0xfe, 0xff, 0xff
        };

        private static readonly int[] Exp2Table =
        {
            0x00, 0x01, 0x01, 0x02, 0x03, 0x03, 0x04, 0x05, 0x06, 0x06, 0x07, 0x08, 0x08, 0x09, 0x0a, 0x0b, 0x0b, 0x0c,
            0x0d, 0x0e, 0x0e, 0x0f, 0x10, 0x10, 0x11, 0x12, 0x13, 0x13, 0x14, 0x15, 0x16, 0x16, 0x17, 0x18, 0x19, 0x19,
            0x1a, 0x1b, 0x1c, 0x1d, 0x1d, 0x1e, 0x1f, 0x20, 0x20, 0x21, 0x22, 0x23, 0x24, 0x24, 0x25, 0x26, 0x27, 0x28,
            0x28, 0x29, 0x2a, 0x2b, 0x2c, 0x2c, 0x2d, 0x2e, 0x2f, 0x30, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x35, 0x36,
            0x37, 0x38, 0x39, 0x3a, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46,
            0x47, 0x48, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f, 0x50, 0x51, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56,
            0x57, 0x58, 0x59, 0x5a, 0x5b, 0x5c, 0x5d, 0x5e, 0x5e, 0x5f, 0x60, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67,
            0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 0x6e, 0x6f, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79,
            0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f, 0x80, 0x81, 0x82, 0x83, 0x84, 0x85, 0x87, 0x88, 0x89, 0x8a, 0x8b, 0x8c,
            0x8d, 0x8e, 0x8f, 0x90, 0x91, 0x92, 0x93, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0x9b, 0x9c, 0x9d, 0x9f, 0xa0,
            0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa8, 0xa9, 0xaa, 0xab, 0xac, 0xad, 0xaf, 0xb0, 0xb1, 0xb2, 0xb3, 0xb4,
            0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbc, 0xbd, 0xbe, 0xbf, 0xc0, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc8, 0xc9, 0xca,
            0xcb, 0xcd, 0xce, 0xcf, 0xd0, 0xd2, 0xd3, 0xd4, 0xd6, 0xd7, 0xd8, 0xd9, 0xdb, 0xdc, 0xdd, 0xde, 0xe0, 0xe1,
            0xe2, 0xe4, 0xe5, 0xe6, 0xe8, 0xe9, 0xea, 0xec, 0xed, 0xee, 0xf0, 0xf1, 0xf2, 0xf4, 0xf5, 0xf6, 0xf8, 0xf9,
            0xfa, 0xfc, 0xfd, 0xff
        };

        private static readonly int[] OnesCountTable =
        {
            0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5,
            0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 6,
            0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5,
            0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 7,
            0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5,
            0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 6,
            0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5,
            0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4, 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 8
        };

        ///////////////////////////// executable code ////////////////////////////////

        // Read the median log2 values from the specifed metadata structure, convert
        // them back to 32-bit unsigned values and store them. If length is not
        // exactly correct then we flag and return an error.

        internal static bool ReadEntropyVars(this WavpackStream wps, WavpackMetadata wpmd)
        {
            var byteptr = wpmd.Data;
            var bArray = new int[12];
            int i;
            var w = new WordsData();

            for (i = 0; i < 6; i++) bArray[i] = byteptr[i] & 0xff;

            w.HoldingOne = 0;
            w.HoldingZero = 0;

            if (wpmd.ByteLength != 12)
                if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                    return false;

            w.C[0].Median[0] = Exp2S(bArray[0] + (bArray[1] << 8));
            w.C[0].Median[1] = Exp2S(bArray[2] + (bArray[3] << 8));
            w.C[0].Median[2] = Exp2S(bArray[4] + (bArray[5] << 8));

            if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
            {
                for (i = 6; i < 12; i++) bArray[i] = byteptr[i] & 0xff;
                w.C[1].Median[0] = Exp2S(bArray[6] + (bArray[7] << 8));
                w.C[1].Median[1] = Exp2S(bArray[8] + (bArray[9] << 8));
                w.C[1].Median[2] = Exp2S(bArray[10] + (bArray[11] << 8));
            }

            wps.W = w;

            return true;
        }

        // Read the hybrid related values from the specifed metadata structure, convert
        // them back to their internal formats and store them. The extended profile
        // stuff is not implemented yet, so return an error if we get more data than
        // we know what to do with.

        internal static bool ReadHybridProfile(this WavpackStream wps, WavpackMetadata wpmd)
        {
            var byteptr = wpmd.Data;
            var bytecnt = wpmd.ByteLength;
            var bufferCounter = 0;
            int unsBuf;
            int unsBufPlusone;

            if ((wps.Wphdr.Flags & Defines.HYBRID_BITRATE) != 0)
            {
                unsBuf = byteptr[bufferCounter] & 0xff;
                unsBufPlusone = byteptr[bufferCounter + 1] & 0xff;

                wps.W.C[0].SlowLevel = Exp2S(unsBuf + (unsBufPlusone << 8));
                bufferCounter = bufferCounter + 2;

                if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                {
                    unsBuf = byteptr[bufferCounter] & 0xff;
                    unsBufPlusone = byteptr[bufferCounter + 1] & 0xff;
                    wps.W.C[1].SlowLevel = Exp2S(unsBuf + (unsBufPlusone << 8));
                    bufferCounter = bufferCounter + 2;
                }
            }

            unsBuf = byteptr[bufferCounter] & 0xff;
            unsBufPlusone = byteptr[bufferCounter + 1] & 0xff;

            wps.W.BitrateAcc[0] = (unsBuf + (unsBufPlusone << 8)) << 16;
            bufferCounter = bufferCounter + 2;

            if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
            {
                unsBuf = byteptr[bufferCounter] & 0xff;
                unsBufPlusone = byteptr[bufferCounter + 1] & 0xff;

                wps.W.BitrateAcc[1] = (unsBuf + (unsBufPlusone << 8)) << 16;
                bufferCounter = bufferCounter + 2;
            }

            if (bufferCounter < bytecnt)
            {
                unsBuf = byteptr[bufferCounter] & 0xff;
                unsBufPlusone = byteptr[bufferCounter + 1] & 0xff;

                wps.W.BitrateDelta[0] = Exp2S((short)(unsBuf + (unsBufPlusone << 8)));
                bufferCounter = bufferCounter + 2;

                if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                {
                    unsBuf = byteptr[bufferCounter] & 0xff;
                    unsBufPlusone = byteptr[bufferCounter + 1] & 0xff;
                    wps.W.BitrateDelta[1] = Exp2S((short)(unsBuf + (unsBufPlusone << 8)));
                    bufferCounter = bufferCounter + 2;
                }

                if (bufferCounter < bytecnt)
                    return false;
            }
            else wps.W.BitrateDelta[0] = wps.W.BitrateDelta[1] = 0;

            return true;
        }

        // Read the next word from the bitstream "wvbits" and return the value. This
        // function can be used for hybrid or lossless streams, but since an
        // optimized version is available for lossless this function would normally
        // be used for hybrid only. If a hybrid lossless stream is being read then
        // the "correction" offset is written at the specified pointer. A return value
        // of WORD_EOF indicates that the end of the bitstream was reached (all 1s) or
        // some other error occurred.

        internal static int GetWords(long nsamples, long flags, WordsData w, BitStream bs, int[] buffer)
        {
            var c = w.C;
            int csamples;
            var bufferCounter = 0;
            var entidx = 1;

            if ((flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                // if not mono
                nsamples *= 2;
            else entidx = 0;

            for (csamples = 0; csamples < nsamples; ++csamples)
            {
                int onesCount, low, high, mid;

                if ((flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                    // if not mono
                    entidx = entidx == 1 ? 0 : 1;

                if ((w.C[0].Median[0] & ~ 1) == 0 && w.HoldingZero == 0 && w.HoldingOne == 0
                    && (w.C[1].Median[0] & ~ 1) == 0)
                {
                    if (w.ZerosAcc > 0)
                    {
                        --w.ZerosAcc;

                        if (w.ZerosAcc > 0)
                        {
                            c[entidx].SlowLevel -= (c[entidx].SlowLevel + Slo) >> Sls;
                            buffer[bufferCounter] = 0;
                            bufferCounter++;
                            continue;
                        }
                    }
                    else
                    {
                        var cbits = 0;
                        bs.GetBit();

                        while (cbits < 33 && bs.BitVal > 0)
                        {
                            cbits++;
                            bs.GetBit();
                        }

                        if (cbits == 33) break;

                        if (cbits < 2) { w.ZerosAcc = cbits; }
                        else
                        {
                            --cbits;

                            long mask;
                            for (mask = 1, w.ZerosAcc = 0; cbits > 0; mask <<= 1)
                            {
                                bs.GetBit();

                                if (bs.BitVal > 0)
                                    w.ZerosAcc |= mask;
                                cbits--;
                            }

                            w.ZerosAcc |= mask;
                        }

                        if (w.ZerosAcc > 0)
                        {
                            c[entidx].SlowLevel -= (c[entidx].SlowLevel + Slo) >> Sls;
                            w.C[0].Median[0] = 0;
                            w.C[0].Median[1] = 0;
                            w.C[0].Median[2] = 0;
                            w.C[1].Median[0] = 0;
                            w.C[1].Median[1] = 0;
                            w.C[1].Median[2] = 0;

                            buffer[bufferCounter] = 0;
                            bufferCounter++;
                            continue;
                        }
                    }
                }

                if (w.HoldingZero > 0) { onesCount = w.HoldingZero = 0; }
                else
                {
                    int next8;
                    int unsBuf;

                    if (bs.Bc < 8)
                    {
                        bs.Ptr++;
                        bs.BufIndex++;

                        if (bs.Ptr == bs.End)
                            bs.Read();

                        unsBuf = bs.Buf[bs.BufIndex] & 0xff;

                        bs.Sr = bs.Sr | (unsBuf << bs.Bc); // values in buffer must be unsigned

                        next8 = bs.Sr & 0xff;

                        bs.Bc += 8;
                    }
                    else { next8 = bs.Sr & 0xff; }

                    if (next8 == 0xff)
                    {
                        bs.Bc -= 8;
                        bs.Sr >>= 8;

                        onesCount = 8;
                        bs.GetBit();

                        while (onesCount < LimitOnes + 1 && bs.BitVal > 0)
                        {
                            onesCount++;
                            bs.GetBit();
                        }

                        if (onesCount == LimitOnes + 1) break;

                        if (onesCount == LimitOnes)
                        {
                            int mask;
                            int cbits;

                            cbits = 0;
                            bs.GetBit();

                            while (cbits < 33 && bs.BitVal > 0)
                            {
                                cbits++;
                                bs.GetBit();
                            }

                            if (cbits == 33) break;

                            if (cbits < 2) { onesCount = cbits; }
                            else
                            {
                                for (mask = 1, onesCount = 0; --cbits > 0; mask <<= 1)
                                {
                                    bs.GetBit();

                                    if (bs.BitVal > 0)
                                        onesCount |= mask;
                                }

                                onesCount |= mask;
                            }

                            onesCount += LimitOnes;
                        }
                    }
                    else
                    {
                        bs.Bc = bs.Bc - ((onesCount = OnesCountTable[next8]) + 1);
                        bs.Sr = bs.Sr >> (onesCount + 1); // needs to be unsigned
                    }

                    if (w.HoldingOne > 0)
                    {
                        w.HoldingOne = onesCount & 1;
                        onesCount = (onesCount >> 1) + 1;
                    }
                    else
                    {
                        w.HoldingOne = onesCount & 1;
                        onesCount >>= 1;
                    }

                    w.HoldingZero = (int)(~ w.HoldingOne & 1);
                }

                if ((flags & Defines.HYBRID_FLAG) > 0
                    && ((flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) > 0 || (csamples & 1) == 0))
                    w.UpdateErrorLimit(flags);

                if (onesCount == 0)
                {
                    low = 0;
                    high = (c[entidx].Median[0] >> 4) + 1 - 1;

                    // for c# I replace the division by DIV0 with >> 7
                    c[entidx].Median[0] -= ((c[entidx].Median[0] + (Div0 - 2)) >> 7) * 2;
                }
                else
                {
                    low = (c[entidx].Median[0] >> 4) + 1;

                    // for c# I replace the division by DIV0 with >> 7
                    c[entidx].Median[0] += ((c[entidx].Median[0] + Div0) >> 7) * 5;

                    if (onesCount == 1)
                    {
                        high = low + (c[entidx].Median[1] >> 4) + 1 - 1;
                        // for c# I replace the division by DIV1 with >> 6
                        c[entidx].Median[1] -= ((c[entidx].Median[1] + (Div1 - 2)) >> 6) * 2;
                    }
                    else
                    {
                        low += (c[entidx].Median[1] >> 4) + 1;
                        // for c# I replace the division by DIV1 with >> 6
                        c[entidx].Median[1] += ((c[entidx].Median[1] + Div1) >> 6) * 5;

                        if (onesCount == 2)
                        {
                            high = low + (c[entidx].Median[2] >> 4) + 1 - 1;
                            // for c# I replace the division by DIV2 with >> 5
                            c[entidx].Median[2] -= ((c[entidx].Median[2] + (Div2 - 2)) >> 5) * 2;
                        }
                        else
                        {
                            low += (onesCount - 2) * ((c[entidx].Median[2] >> 4) + 1);
                            high = low + (c[entidx].Median[2] >> 4) + 1 - 1;
                            // for c# I replace the division by DIV2 with >> 5
                            c[entidx].Median[2] += ((c[entidx].Median[2] + Div2) >> 5) * 5;
                        }
                    }
                }

                mid = (high + low + 1) >> 1;

                if (c[entidx].ErrorLimit == 0)
                {
                    mid = ReadCode(bs, high - low);

                    mid = mid + low;
                }
                else
                {
                    while (high - low > c[entidx].ErrorLimit)
                    {
                        bs.GetBit();

                        if (bs.BitVal > 0) mid = (high + (low = mid) + 1) >> 1;
                        else mid = ((high = mid - 1) + low + 1) >> 1;
                    }
                }

                bs.GetBit();

                if (bs.BitVal > 0) buffer[bufferCounter] = ~ mid;
                else buffer[bufferCounter] = mid;

                bufferCounter++;

                if ((flags & Defines.HYBRID_BITRATE) > 0)
                    c[entidx].SlowLevel = c[entidx].SlowLevel - ((c[entidx].SlowLevel + Slo) >> Sls) + MyLog2(mid);
            }

            w.C = c;

            if ((flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) != 0) return csamples;

            return csamples / 2;
        }

        private static int CountBits(int av)
        {
            if (av < 1 << 8) return NbitsTable[av];

            if (av < 1 << 16) return NbitsTable[av >> 8] + 8;

            if (av < 1 << 24) return NbitsTable[av >> 16] + 16;

            return NbitsTable[av >> 24] + 24;
        }

        // Read a single unsigned value from the specified bitstream with a value
        // from 0 to maxcode. If there are exactly a power of two number of possible
        // codes then this will read a fixed number of bits; otherwise it reads the
        // minimum number of bits and then determines whether another bit is needed
        // to define the code.

        private static int ReadCode(this BitStream bs, int maxcode)
        {
            var bitcount = CountBits(maxcode);
            int extras = (1 << bitcount) - maxcode - 1, code;

            if (bitcount == 0) return 0;

            code = bs.GetBits(bitcount - 1);

            code &= (1 << (bitcount - 1)) - 1;

            if (code >= extras)
            {
                code = (code << 1) - extras;

                bs.GetBit();

                if (bs.BitVal > 0)
                    ++code;
            }

            return code;
        }

        // The concept of a base 2 logarithm is used in many parts of WavPack. It is
        // a way of sufficiently accurately representing 32-bit signed and unsigned
        // values storing only 16 bits (actually fewer). It is also used in the hybrid
        // mode for quickly comparing the relative magnitude of large values (i.e.
        // division) and providing smooth exponentials using only addition.

        // These are not strict logarithms in that they become linear around zero and
        // can therefore represent both zero and negative values. They have 8 bits
        // of precision and in "roundtrip" conversions the total error never exceeds 1
        // part in 225 except for the cases of +/-115 and +/-195 (which error by 1).

        // This function returns the log2 for the specified 32-bit unsigned value.
        // The maximum value allowed is about 0xff800000 and returns 8447.

        private static int MyLog2(long avalue)
        {
            int dbits;

            if ((avalue += avalue >> 9) < 1 << 8)
            {
                dbits = NbitsTable[(int)avalue];
                return (dbits << 8) + Log2Table[(int)(avalue << (9 - dbits)) & 0xff];
            }

            if (avalue < 1L << 16)
                dbits = NbitsTable[(int)(avalue >> 8)] + 8;
            else if (avalue < 1L << 24)
                dbits = NbitsTable[(int)(avalue >> 16)] + 16;
            else
                dbits = NbitsTable[(int)(avalue >> 24)] + 24;

            return (dbits << 8) + Log2Table[(int)(avalue >> (dbits - 9)) & 0xff];
        }

        // This function returns the log2 for the specified 32-bit signed value.
        // All input values are valid and the return values are in the range of
        // +/- 8192.

        internal static int Log2S(int valueRenamed)
        {
            if (valueRenamed < 0) return -MyLog2(-valueRenamed);

            return MyLog2(valueRenamed);
        }

        // This function returns the original integer represented by the supplied
        // logarithm (at least within the provided accuracy). The log is signed,
        // but since a full 32-bit value is returned this can be used for unsigned
        // conversions as well (i.e. the input range is -8192 to +8447).

        internal static int Exp2S(int log)
        {
            if (log < 0)
                return -Exp2S(-log);

            long valueRenamed = Exp2Table[log & 0xff] | 0x100;

            if ((log >>= 8) <= 9)
                return (int)(valueRenamed >> (9 - log));
            return (int)(valueRenamed << (log - 9));
        }

        // These two functions convert internal weights (which are normally +/-1024)
        // to and from an 8-bit signed character version for storage in metadata. The
        // weights are clipped here in the case that they are outside that range.

        internal static int RestoreWeight(sbyte weight)
        {
            int result;

            if ((result = weight << 3) > 0)
                result += (result + 64) >> 7;

            return result;
        }
    }
}
