/*
** WavpackConfig.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

namespace WavpackDecoder
{
    internal class WavpackConfig
    {
        internal int BitsPerSample, BytesPerSample;
        internal long Flags, SampleRate, ChannelMask; // was uint32_t in C
        internal int NumChannels, FloatNormExp;
    }
}
