/*
** Bitstream.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)
***/

namespace WavpackDecoder
{
    class Bitstream
    {
        internal short end, ptr; // was uchar in c
        internal int sr;
        internal int file_bytes;
        internal int error, bc;
        internal System.IO.BinaryReader file;
        internal int bitval = 0;
        internal byte[] buf = new byte[1024];
        internal int buf_index = 0;
    }
}
