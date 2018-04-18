/*
** WavpackHeader.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

namespace WavpackDecoder
{
    internal class WavpackHeader
    {
        internal char[] CkId = new char[4];
        internal long CkSize; // was uint32_t in C
        internal int Status = 0; // 1 means error
        internal long TotalSamples, BlockIndex, BlockSamples, Flags, Crc; // was uint32_t in C
        internal short TrackNo, IndexNo; // was uchar in C
        internal short Version;
    }
}
