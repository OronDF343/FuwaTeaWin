/*
** WavpackMetadata.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

namespace WavpackDecoder
{
    internal class WavpackMetadata
    {
        internal long ByteCount = 24; // we use this to determine if we have read all the metadata 
        internal int ByteLength;
        internal byte[] Data;
        internal bool HasData = false; // 0 does not have data, 1 has data
        //internal bool HasError = false; // 0 ok, 1 error

        internal short Id; // was uchar in C
        // in a block by checking bytecount again the block length
        // ckSize is block size minus 8. WavPack header is 32 bytes long so we start at 24
    }
}
