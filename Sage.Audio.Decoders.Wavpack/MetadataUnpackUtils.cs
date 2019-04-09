/*
** UnpackUtils.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

namespace Sage.Audio.Decoders.Wavpack
{
    internal static class MetadataUnpackUtils
    {
        // This function initialzes the main bitstream for audio samples, which must
        // be in the "wv" file.

        internal static bool InitWvBitStream(this WavpackContext wpc, WavpackMetadata wpmd)
        {
            if (wpmd.HasData)
            {
                wpc.Stream.WvBits = new BitStream(wpmd.Data, 0, (short)wpmd.ByteLength, wpc.InFile, 0, 0);
            }
            else if (wpmd.ByteLength > 0)
            {
                var len = wpmd.ByteLength & 1;
                wpc.Stream.WvBits = new BitStream(wpc.ReadBuffer, -1, (short)wpc.ReadBuffer.Length, wpc.InFile,
                                                  wpmd.ByteLength + len, 1);
            }

            return true;
        }

        // Read decorrelation terms from specified metadata block into the
        // decorr_passes array. The terms range from -3 to 8, plus 17 & 18;
        // other values are reserved and generate errors for now. The delta
        // ranges from 0 to 7 with all values valid. Note that the terms are
        // stored in the opposite order in the decorr_passes array compared
        // to packing.

        internal static bool ReadDecorrTerms(this WavpackStream wps, WavpackMetadata wpmd)
        {
            var termcnt = wpmd.ByteLength;
            var byteptr = wpmd.Data;
            var tmpwps = new WavpackStream();

            var counter = 0;
            int dcounter;

            if (termcnt > Defines.MAX_NTERMS)
                return false;

            tmpwps.NumTerms = termcnt;

            //dcounter = termcnt - 1;

            for (dcounter = termcnt - 1; dcounter >= 0; dcounter--)
            {
                tmpwps.DecorrPasses[dcounter].Term = (short)((byteptr[counter] & 0x1f) - 5);
                tmpwps.DecorrPasses[dcounter].Delta = (short)((byteptr[counter] >> 5) & 0x7);

                counter++;

                if (tmpwps.DecorrPasses[dcounter].Term < -3
                    || tmpwps.DecorrPasses[dcounter].Term > Defines.MAX_TERM && tmpwps.DecorrPasses[dcounter].Term < 17
                    || tmpwps.DecorrPasses[dcounter].Term > 18)
                    return false;
            }

            wps.DecorrPasses = tmpwps.DecorrPasses;
            wps.NumTerms = tmpwps.NumTerms;

            return true;
        }

        // Read decorrelation weights from specified metadata block into the
        // decorr_passes array. The weights range +/-1024, but are rounded and
        // truncated to fit in signed chars for metadata storage. Weights are
        // separate for the two channels and are specified from the "last" term
        // (first during encode). Unspecified weights are set to zero.

        internal static bool ReadDecorrWeights(this WavpackStream wps, WavpackMetadata wpmd)
        {
            int termcnt = wpmd.ByteLength, tcount;
            var byteptr = wpmd.Data;
            var dpp = new DecorrPass();
            var counter = 0;

            if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                termcnt /= 2;

            if (termcnt > wps.NumTerms) return false;

            for (tcount = wps.NumTerms; tcount > 0; tcount--)
                dpp.WeightA = dpp.WeightB = 0;

            var myiterator = wps.NumTerms;

            while (termcnt > 0)
            {
                var dppIdx = myiterator - 1;
                dpp.WeightA = (short)WordsUtils.RestoreWeight((sbyte)byteptr[counter]);

                wps.DecorrPasses[dppIdx].WeightA = dpp.WeightA;

                counter++;

                if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                {
                    dpp.WeightB = (short)WordsUtils.RestoreWeight((sbyte)byteptr[counter]);
                    counter++;
                }

                wps.DecorrPasses[dppIdx].WeightB = dpp.WeightB;

                myiterator--;
                termcnt--;
            }

            return true;
        }

        // Read decorrelation samples from specified metadata block into the
        // decorr_passes array. The samples are signed 32-bit values, but are
        // converted to signed log2 values for storage in metadata. Values are
        // stored for both channels and are specified from the "last" term
        // (first during encode) with unspecified samples set to zero. The
        // number of samples stored varies with the actual term value, so
        // those must obviously come first in the metadata.

        internal static bool ReadDecorrSamples(this WavpackStream wps, WavpackMetadata wpmd)
        {
            var byteptr = wpmd.Data;
            var dpp = new DecorrPass();
            int tcount;
            var counter = 0;

            var dppIndex = 0;

            for (tcount = wps.NumTerms; tcount > 0; tcount--)
            {
                dpp.Term = wps.DecorrPasses[dppIndex].Term;

                for (var internalc = 0; internalc < Defines.MAX_TERM; internalc++)
                {
                    dpp.SamplesA[internalc] = 0;
                    dpp.SamplesB[internalc] = 0;
                    wps.DecorrPasses[dppIndex].SamplesA[internalc] = 0;
                    wps.DecorrPasses[dppIndex].SamplesB[internalc] = 0;
                }

                dppIndex++;
            }

            if (wps.Wphdr.Version == 0x402 && (wps.Wphdr.Flags & Defines.HYBRID_FLAG) > 0)
            {
                counter += 2;

                if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                    counter += 2;
            }

            dppIndex--;

            while (counter < wpmd.ByteLength)
            {
                int unsBuf0, unsBuf1, unsBuf2, unsBuf3;
                if (dpp.Term > Defines.MAX_TERM)
                {
                    unsBuf0 = byteptr[counter] & 0xff;
                    unsBuf1 = byteptr[counter + 1] & 0xff;
                    unsBuf2 = byteptr[counter + 2] & 0xff;
                    unsBuf3 = byteptr[counter + 3] & 0xff;

                    dpp.SamplesA[0] = WordsUtils.Exp2S((short)(unsBuf0 + (unsBuf1 << 8)));
                    dpp.SamplesA[1] = WordsUtils.Exp2S((short)(unsBuf2 + (unsBuf3 << 8)));
                    counter += 4;

                    if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                    {
                        unsBuf0 = byteptr[counter] & 0xff;
                        unsBuf1 = byteptr[counter + 1] & 0xff;
                        unsBuf2 = byteptr[counter + 2] & 0xff;
                        unsBuf3 = byteptr[counter + 3] & 0xff;

                        dpp.SamplesB[0] = WordsUtils.Exp2S((short)(unsBuf0 + (unsBuf1 << 8)));
                        dpp.SamplesB[1] = WordsUtils.Exp2S((short)(unsBuf2 + (unsBuf3 << 8)));
                        counter += 4;
                    }
                }
                else if (dpp.Term < 0)
                {
                    unsBuf0 = byteptr[counter] & 0xff;
                    unsBuf1 = byteptr[counter + 1] & 0xff;
                    unsBuf2 = byteptr[counter + 2] & 0xff;
                    unsBuf3 = byteptr[counter + 3] & 0xff;

                    dpp.SamplesA[0] = WordsUtils.Exp2S((short)(unsBuf0 + (unsBuf1 << 8)));
                    dpp.SamplesB[0] = WordsUtils.Exp2S((short)(unsBuf2 + (unsBuf3 << 8)));

                    counter += 4;
                }
                else
                {
                    int m = 0, cnt = dpp.Term;

                    while (cnt > 0)
                    {
                        unsBuf0 = byteptr[counter] & 0xff;
                        unsBuf1 = byteptr[counter + 1] & 0xff;

                        dpp.SamplesA[m] = WordsUtils.Exp2S((short)(unsBuf0 + (unsBuf1 << 8)));
                        counter += 2;

                        if ((wps.Wphdr.Flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) == 0)
                        {
                            unsBuf0 = byteptr[counter] & 0xff;
                            unsBuf1 = byteptr[counter + 1] & 0xff;
                            dpp.SamplesB[m] = WordsUtils.Exp2S((short)(unsBuf0 + (unsBuf1 << 8)));
                            counter += 2;
                        }

                        m++;
                        cnt--;
                    }
                }

                int sampleCounter;
                for (sampleCounter = 0; sampleCounter < Defines.MAX_TERM; sampleCounter++)
                {
                    wps.DecorrPasses[dppIndex].SamplesA[sampleCounter] = dpp.SamplesA[sampleCounter];
                    wps.DecorrPasses[dppIndex].SamplesB[sampleCounter] = dpp.SamplesB[sampleCounter];
                }

                dppIndex--;
            }

            return true;
        }

        // Read the int32 data from the specified metadata into the specified stream.
        // This data is used for integer data that has more than 24 bits of magnitude
        // or, in some cases, used to eliminate redundant bits from any audio stream.

        internal static bool ReadInt32Info(this WavpackStream wps, WavpackMetadata wpmd)
        {
            var bytecnt = wpmd.ByteLength;
            var byteptr = wpmd.Data;
            var counter = 0;

            if (bytecnt != 4)
                return false; // should also return 0

            wps.Int32SentBits = byteptr[counter];
            counter++;
            wps.Int32Zeros = byteptr[counter];
            counter++;
            wps.Int32Ones = byteptr[counter];
            counter++;
            wps.Int32Dups = byteptr[counter];

            return true;
        }

        // Read multichannel information from metadata. The first byte is the total
        // number of channels and the following bytes represent the channel_mask
        // as described for Microsoft WAVEFORMATEX.

        internal static bool ReadChannelInfo(this WavpackContext wpc, WavpackMetadata wpmd)
        {
            int bytecnt = wpmd.ByteLength, shift = 0;
            var byteptr = wpmd.Data;
            var counter = 0;
            long mask = 0;

            if (bytecnt == 0 || bytecnt > 5)
                return false;

            wpc.Config.NumChannels = byteptr[counter];
            counter++;

            while (bytecnt >= 0)
            {
                mask |= (byteptr[counter] & 0xFFu) << shift;
                counter++;
                shift += 8;
                bytecnt--;
            }

            wpc.Config.ChannelMask = mask;
            return true;
        }

        // Read configuration information from metadata.

        internal static bool ReadConfigInfo(this WavpackContext wpc, WavpackMetadata wpmd)
        {
            var bytecnt = wpmd.ByteLength;
            var byteptr = wpmd.Data;
            var counter = 0;

            if (bytecnt >= 3)
            {
                wpc.Config.Flags &= 0xFFu;
                wpc.Config.Flags |= (byteptr[counter] & 0xFFu) << 8;
                counter++;
                wpc.Config.Flags |= (byteptr[counter] & 0xFFu) << 16;
                counter++;
                wpc.Config.Flags |= (byteptr[counter] & 0xFFu) << 24;
            }

            return true;
        }

        // Read non-standard sampling rate from metadata.

        internal static bool ReadSampleRate(this WavpackContext wpc, WavpackMetadata wpmd)
        {
            var bytecnt = wpmd.ByteLength;
            var byteptr = wpmd.Data;
            var counter = 0;

            if (bytecnt == 3)
            {
                wpc.Config.SampleRate = byteptr[counter] & 0xFFu;
                counter++;
                wpc.Config.SampleRate |= (byteptr[counter] & 0xFFu) << 8;
                counter++;
                wpc.Config.SampleRate |= (byteptr[counter] & 0xFFu) << 16;
            }

            return true;
        }

        internal static bool ReadFloatInfo(this WavpackStream wps, WavpackMetadata wpmd)
        {
            var bytecnt = wpmd.ByteLength;
            var byteptr = wpmd.Data;
            var counter = 0;

            if (bytecnt != 4)
                return false;

            wps.FloatFlags = byteptr[counter];
            counter++;
            wps.FloatShift = byteptr[counter];
            counter++;
            wps.FloatMaxExp = byteptr[counter];
            counter++;
            wps.FloatNormExp = byteptr[counter];

            return true;
        }
    }
}
