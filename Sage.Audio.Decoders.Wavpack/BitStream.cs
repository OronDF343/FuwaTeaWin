/*
** Bitstream.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)
***/

using System;
using System.IO;

namespace Sage.Audio.Decoders.Wavpack
{
    internal class BitStream
    {
        internal BitStream() { }

        internal BitStream(byte[] stream, short bufferStart, short bufferEnd, BinaryReader file, int fileBytes,
                           int passed)
        {
            Buf = stream;
            BufIndex = bufferStart;
            End = bufferEnd;
            Sr = 0;
            Bc = 0;

            if (passed != 0)
            {
                Ptr = (short)(End - 1);
                FileBytes = fileBytes;
                File = file;
            }
            else
            {
                /* Strange to set an index to -1, but the very first call to getbit will iterate this */
                BufIndex = -1;
                Ptr = -1;
            }
        }

        internal int BitVal;
        internal byte[] Buf = new byte[1024];
        internal int BufIndex;
        internal short End, Ptr; // was uchar in c
        internal int Error, Bc;
        internal BinaryReader File;
        internal int FileBytes;
        internal int Sr;

        internal void Read()
        {
            if (FileBytes > 0)
            {
                int bytesRead;

                var bytesToRead = 1024;

                if (bytesToRead > FileBytes)
                    bytesToRead = FileBytes;

                var buf = new byte[1024];

                try
                {
                    bytesRead = File.BaseStream.Read(buf, 0, bytesToRead);

                    BufIndex = 0;
                    Buf = buf;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Big error while reading file: " + e);
                    bytesRead = 0;
                }

                if (bytesRead > 0)
                {
                    End = (short)bytesRead;
                    FileBytes -= bytesRead;
                }
                else
                {
                    for (var i = 0; i < End - BufIndex; i++) Buf[i] = unchecked((byte)-1);
                    Error = 1;
                }
            }
            else { Error = 1; }

            if (Error > 0)
                for (var i = 0; i < End - BufIndex; i++)
                    Buf[i] = unchecked((byte)-1);

            Ptr = 0;
            BufIndex = 0;

            //return bs;
        }

        internal void GetBit()
        {
            if (Bc > 0) { Bc--; }
            else
            {
                Ptr++;
                BufIndex++;
                Bc = 7;

                if (Ptr == End) Read();
                Sr = Buf[BufIndex] & 0xff;
            }

            if ((Sr & 1) > 0)
            {
                Sr = Sr >> 1;
                BitVal = 1;
            }
            else
            {
                Sr = Sr >> 1;
                BitVal = 0;
            }
        }

        internal int GetBits(int nbits)
        {
            while (nbits > Bc)
            {
                Ptr++;
                BufIndex++;

                if (Ptr == End) Read();
                var unsBuf = Buf[BufIndex] & 0xff;
                Sr = Sr | (unsBuf << Bc); // values in buffer must be unsigned
                Bc += 8;
            }

            var retval = Sr;

            if (Bc > 32)
            {
                Bc -= nbits;
                Sr = Ptr >> (8 - Bc);
            }
            else
            {
                Bc -= nbits;
                Sr >>= nbits;
            }

            return retval;
        }
    }
}
