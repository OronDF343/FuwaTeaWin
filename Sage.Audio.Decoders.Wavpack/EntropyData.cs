/*
** entropy_data.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

namespace Sage.Audio.Decoders.Wavpack
{
    internal class EntropyData
    {
        internal long ErrorLimit; // was uint32_t in C
        internal readonly int[] Median = { 0, 0, 0 }; // was uint32_t in C, we initialize in order to remove run time errors
        internal long SlowLevel;
    }
}
