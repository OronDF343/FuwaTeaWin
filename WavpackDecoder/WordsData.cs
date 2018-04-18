/*
** words_data.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

namespace WavpackDecoder
{
    internal class WordsData
    {
        public WordsData()
        {
            InitBlock();
        }

        internal readonly long[] BitrateAcc = new long[2]; // was uint32_t  in C
        internal readonly long[] BitrateDelta = new long[2]; // was uint32_t  in C
        internal EntropyData[] C;
        internal long HoldingOne, ZerosAcc; // was uint32_t  in C
        internal int HoldingZero;

        private readonly EntropyData _tempEd1 = new EntropyData();
        private readonly EntropyData _tempEd2 = new EntropyData();

        private void InitBlock()
        {
            C = new[] { _tempEd1, _tempEd2 };
        }

        // This function is called during both encoding and decoding of hybrid data to
        // update the "error_limit" variable which determines the maximum sample error
        // allowed in the main bitstream. In the HYBRID_BITRATE mode (which is the only
        // currently implemented) this is calculated from the slow_level values and the
        // bitrate accumulators. Note that the bitrate accumulators can be changing.

        internal void UpdateErrorLimit(long flags)
        {
            var bitrate0 = (int)((BitrateAcc[0] += BitrateDelta[0]) >> 16);

            if ((flags & (Defines.MONO_FLAG | Defines.FALSE_STEREO)) != 0)
            {
                if ((flags & Defines.HYBRID_BITRATE) != 0)
                {
                    var slowLog0 = (int)((C[0].SlowLevel + WordsUtils.Slo) >> WordsUtils.Sls);

                    C[0].ErrorLimit = slowLog0 - bitrate0 > -0x100 ? WordsUtils.Exp2S(slowLog0 - bitrate0 + 0x100) : 0;
                }
                else { C[0].ErrorLimit = WordsUtils.Exp2S(bitrate0); }
            }
            else
            {
                var bitrate1 = (int)((BitrateAcc[1] += BitrateDelta[1]) >> 16);

                if ((flags & Defines.HYBRID_BITRATE) != 0)
                {
                    var slowLog0 = (int)((C[0].SlowLevel + WordsUtils.Slo) >> WordsUtils.Sls);
                    var slowLog1 = (int)((C[1].SlowLevel + WordsUtils.Slo) >> WordsUtils.Sls);

                    if ((flags & Defines.HYBRID_BALANCE) != 0)
                    {
                        var balance = (slowLog1 - slowLog0 + bitrate1 + 1) >> 1;

                        if (balance > bitrate0)
                        {
                            bitrate1 = bitrate0 * 2;
                            bitrate0 = 0;
                        }
                        else if (-balance > bitrate0)
                        {
                            bitrate0 = bitrate0 * 2;
                            bitrate1 = 0;
                        }
                        else
                        {
                            bitrate1 = bitrate0 + balance;
                            bitrate0 = bitrate0 - balance;
                        }
                    }

                    C[0].ErrorLimit = slowLog0 - bitrate0 > -0x100 ? WordsUtils.Exp2S(slowLog0 - bitrate0 + 0x100) : 0;

                    C[1].ErrorLimit = slowLog1 - bitrate1 > -0x100 ? WordsUtils.Exp2S(slowLog1 - bitrate1 + 0x100) : 0;
                }
                else
                {
                    C[0].ErrorLimit = WordsUtils.Exp2S(bitrate0);
                    C[1].ErrorLimit = WordsUtils.Exp2S(bitrate1);
                }
            }
        }
    }
}
