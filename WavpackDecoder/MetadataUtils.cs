/*
** MetadataUtils.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

using System;

namespace WavpackDecoder
{
    internal static class MetadataUtils
    {
        internal static bool ReadMetadataBuff(WavpackContext wpc, WavpackMetadata wpmd)
        {
            short tchar;

            if (wpmd.ByteCount >= wpc.Stream.Wphdr.CkSize) return false;

            try
            {
                wpmd.Id = wpc.InFile.ReadByte();
                tchar = wpc.InFile.ReadByte();
            }
            catch (Exception)
            {
                //wpmd.HasError = true;
                return false;
            }

            wpmd.ByteCount += 2;

            wpmd.ByteLength = tchar << 1;

            if ((wpmd.Id & Defines.ID_LARGE) != 0)
            {
                wpmd.Id &= ~ Defines.ID_LARGE;

                try { tchar = wpc.InFile.ReadByte(); }
                catch (Exception)
                {
                    //wpmd.HasError = true;
                    return false;
                }

                wpmd.ByteLength += tchar << 9;

                try { tchar = wpc.InFile.ReadByte(); }
                catch (Exception)
                {
                    //wpmd.HasError = true;
                    return false;
                }

                wpmd.ByteLength += tchar << 17;
                wpmd.ByteCount += 2;
            }

            if ((wpmd.Id & Defines.ID_ODD_SIZE) != 0)
            {
                wpmd.Id &= ~ Defines.ID_ODD_SIZE;
                wpmd.ByteLength--;
            }

            if (wpmd.ByteLength == 0 || wpmd.Id == Defines.ID_WV_BITSTREAM)
            {
                wpmd.HasData = false;
                return true;
            }

            long bytesToRead = wpmd.ByteLength + (wpmd.ByteLength & 1);

            wpmd.ByteCount += bytesToRead;

            if (bytesToRead > wpc.ReadBuffer.Length)
            {
                int bytesRead;
                wpmd.HasData = false;

                while (bytesToRead > wpc.ReadBuffer.Length)
                {
                    try
                    {
                        bytesRead = wpc.InFile.BaseStream.Read(wpc.ReadBuffer, 0, wpc.ReadBuffer.Length);

                        if (bytesRead != wpc.ReadBuffer.Length) return false;
                    }
                    catch (Exception) { return false; }

                    bytesToRead -= wpc.ReadBuffer.Length;
                }
            }
            else
            {
                wpmd.HasData = true;
                wpmd.Data = wpc.ReadBuffer;
            }

            if (bytesToRead != 0)
                try
                {
                    var bytesRead = wpc.InFile.BaseStream.Read(wpc.ReadBuffer, 0, (int)bytesToRead);

                    if (bytesRead != (int)bytesToRead)
                    {
                        wpmd.HasData = false;
                        return false;
                    }
                }
                catch (Exception)
                {
                    wpmd.HasData = false;
                    return false;
                }

            return true;
        }

        internal static bool ProcessMetadata(WavpackContext wpc, WavpackMetadata wpmd)
        {
            var wps = wpc.Stream;

            switch (wpmd.Id)
            {
                case Defines.ID_DUMMY: return true;

                case Defines.ID_DECORR_TERMS: return wps.ReadDecorrTerms(wpmd);

                case Defines.ID_DECORR_WEIGHTS: return wps.ReadDecorrWeights(wpmd);

                case Defines.ID_DECORR_SAMPLES: return wps.ReadDecorrSamples(wpmd);

                case Defines.ID_ENTROPY_VARS: return wps.ReadEntropyVars(wpmd);

                case Defines.ID_HYBRID_PROFILE: return wps.ReadHybridProfile(wpmd);

                case Defines.ID_FLOAT_INFO: return wps.ReadFloatInfo(wpmd);

                case Defines.ID_INT32_INFO: return wps.ReadInt32Info(wpmd);

                case Defines.ID_CHANNEL_INFO: return wpc.ReadChannelInfo(wpmd);

                case Defines.ID_SAMPLE_RATE: return wpc.ReadSampleRate(wpmd);

                case Defines.ID_CONFIG_BLOCK: return wpc.ReadConfigInfo(wpmd);

                case Defines.ID_WV_BITSTREAM: return wpc.InitWvBitStream(wpmd);

                case Defines.ID_SHAPING_WEIGHTS:
                case Defines.ID_WVC_BITSTREAM:
                case Defines.ID_WVX_BITSTREAM:
                    return true;

                default: return (wpmd.Id & Defines.ID_OPTIONAL_DATA) != 0;
            }
        }
    }
}
