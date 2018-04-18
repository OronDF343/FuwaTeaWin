/*
** decorr_pass.cs
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
    internal class DecorrPass
    {
        public DecorrPass()
        {
            InitBlock();
        }

        internal int[] SamplesA;
        internal int[] SamplesB;
        internal short Term, Delta, WeightA, WeightB;

        private void InitBlock()
        {
            SamplesA = new int[Defines.MAX_TERM];
            SamplesB = new int[Defines.MAX_TERM];
        }

        internal void StereoPass(int[] buffer, long sampleCount, int bufIdx)
        {
            int delta = Delta;
            int weightA = WeightA;
            int weightB = WeightB;
            int samA, samB;
            int m, k;
            int bptrCounter;

            switch (Term)
            {
                case 17:
                    for (bptrCounter = bufIdx; bptrCounter < bufIdx + sampleCount * 2; bptrCounter += 2)
                    {
                        samA = 2 * SamplesA[0] - SamplesA[1];
                        SamplesA[1] = SamplesA[0];
                        SamplesA[0] = (int)((weightA * (long)samA + 512) >> 10) + buffer[bptrCounter];

                        if (samA != 0 && buffer[bptrCounter] != 0)
                        {
                            if ((samA ^ buffer[bptrCounter]) < 0) weightA = weightA - delta;
                            else weightA = weightA + delta;
                        }

                        buffer[bptrCounter] = SamplesA[0];

                        samA = 2 * SamplesB[0] - SamplesB[1];
                        SamplesB[1] = SamplesB[0];
                        SamplesB[0] = (int)((weightB * (long)samA + 512) >> 10) + buffer[bptrCounter + 1];

                        if (samA != 0 && buffer[bptrCounter + 1] != 0)
                        {
                            if ((samA ^ buffer[bptrCounter + 1]) < 0) weightB = weightB - delta;
                            else weightB = weightB + delta;
                        }

                        buffer[bptrCounter + 1] = SamplesB[0];
                    }

                    break;

                case 18:
                    for (bptrCounter = bufIdx; bptrCounter < bufIdx + sampleCount * 2; bptrCounter += 2)
                    {
                        samA = (3 * SamplesA[0] - SamplesA[1]) >> 1;
                        SamplesA[1] = SamplesA[0];
                        SamplesA[0] = (int)((weightA * (long)samA + 512) >> 10) + buffer[bptrCounter];

                        if (samA != 0 && buffer[bptrCounter] != 0)
                        {
                            if ((samA ^ buffer[bptrCounter]) < 0) weightA = weightA - delta;
                            else weightA = weightA + delta;
                        }

                        buffer[bptrCounter] = SamplesA[0];

                        samA = (3 * SamplesB[0] - SamplesB[1]) >> 1;
                        SamplesB[1] = SamplesB[0];
                        SamplesB[0] = (int)((weightB * (long)samA + 512) >> 10) + buffer[bptrCounter + 1];

                        if (samA != 0 && buffer[bptrCounter + 1] != 0)
                        {
                            if ((samA ^ buffer[bptrCounter + 1]) < 0) weightB = weightB - delta;
                            else weightB = weightB + delta;
                        }

                        buffer[bptrCounter + 1] = SamplesB[0];
                    }

                    break;

                case -1:
                    for (bptrCounter = bufIdx; bptrCounter < bufIdx + sampleCount * 2; bptrCounter += 2)
                    {
                        samA = buffer[bptrCounter] + (int)((weightA * (long)SamplesA[0] + 512) >> 10);

                        if ((SamplesA[0] ^ buffer[bptrCounter]) < 0)
                        {
                            if (SamplesA[0] != 0 && buffer[bptrCounter] != 0 && (weightA -= delta) < -1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }
                        else
                        {
                            if (SamplesA[0] != 0 && buffer[bptrCounter] != 0 && (weightA += delta) > 1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }

                        buffer[bptrCounter] = samA;
                        SamplesA[0] = buffer[bptrCounter + 1] + (int)((weightB * (long)samA + 512) >> 10);

                        if ((samA ^ buffer[bptrCounter + 1]) < 0)
                        {
                            if (samA != 0 && buffer[bptrCounter + 1] != 0 && (weightB -= delta) < -1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }
                        else
                        {
                            if (samA != 0 && buffer[bptrCounter + 1] != 0 && (weightB += delta) > 1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }

                        buffer[bptrCounter + 1] = SamplesA[0];
                    }

                    break;

                case -2:
                    //sam_B = 0;
                    //sam_A = 0;

                    for (bptrCounter = bufIdx; bptrCounter < bufIdx + sampleCount * 2; bptrCounter += 2)
                    {
                        samB = buffer[bptrCounter + 1] + (int)((weightB * (long)SamplesB[0] + 512) >> 10);

                        if ((SamplesB[0] ^ buffer[bptrCounter + 1]) < 0)
                        {
                            if (SamplesB[0] != 0 && buffer[bptrCounter + 1] != 0 && (weightB -= delta) < -1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }
                        else
                        {
                            if (SamplesB[0] != 0 && buffer[bptrCounter + 1] != 0 && (weightB += delta) > 1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }

                        buffer[bptrCounter + 1] = samB;

                        SamplesB[0] = buffer[bptrCounter] + (int)((weightA * (long)samB + 512) >> 10);

                        if ((samB ^ buffer[bptrCounter]) < 0)
                        {
                            if (samB != 0 && buffer[bptrCounter] != 0 && (weightA -= delta) < -1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }
                        else
                        {
                            if (samB != 0 && buffer[bptrCounter] != 0 && (weightA += delta) > 1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }

                        buffer[bptrCounter] = SamplesB[0];
                    }

                    break;

                case -3:
                    //sam_A = 0;

                    for (bptrCounter = bufIdx; bptrCounter < bufIdx + sampleCount * 2; bptrCounter += 2)
                    {
                        samA = buffer[bptrCounter] + (int)((weightA * (long)SamplesA[0] + 512) >> 10);

                        if ((SamplesA[0] ^ buffer[bptrCounter]) < 0)
                        {
                            if (SamplesA[0] != 0 && buffer[bptrCounter] != 0 && (weightA -= delta) < -1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }
                        else

                        {
                            if (SamplesA[0] != 0 && buffer[bptrCounter] != 0 && (weightA += delta) > 1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }

                        samB = buffer[bptrCounter + 1] + (int)((weightB * (long)SamplesB[0] + 512) >> 10);

                        if ((SamplesB[0] ^ buffer[bptrCounter + 1]) < 0)
                        {
                            if (SamplesB[0] != 0 && buffer[bptrCounter + 1] != 0 && (weightB -= delta) < -1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }
                        else
                        {
                            if (SamplesB[0] != 0 && buffer[bptrCounter + 1] != 0 && (weightB += delta) > 1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }

                        buffer[bptrCounter] = SamplesB[0] = samA;
                        buffer[bptrCounter + 1] = SamplesA[0] = samB;
                    }

                    break;

                default:

                    //sam_A = 0;

                    for (m = 0, k = Term & (Defines.MAX_TERM - 1), bptrCounter = bufIdx;
                         bptrCounter < bufIdx + sampleCount * 2;
                         bptrCounter += 2)
                    {
                        samA = SamplesA[m];
                        SamplesA[k] = (int)((weightA * (long)samA + 512) >> 10) + buffer[bptrCounter];

                        if (samA != 0 && buffer[bptrCounter] != 0)
                        {
                            if ((samA ^ buffer[bptrCounter]) < 0) weightA = weightA - delta;
                            else weightA = weightA + delta;
                        }

                        buffer[bptrCounter] = SamplesA[k];

                        samA = SamplesB[m];
                        SamplesB[k] = (int)((weightB * (long)samA + 512) >> 10) + buffer[bptrCounter + 1];

                        if (samA != 0 && buffer[bptrCounter + 1] != 0)
                        {
                            if ((samA ^ buffer[bptrCounter + 1]) < 0) weightB = weightB - delta;
                            else weightB = weightB + delta;
                        }

                        buffer[bptrCounter + 1] = SamplesB[k];

                        m = (m + 1) & (Defines.MAX_TERM - 1);
                        k = (k + 1) & (Defines.MAX_TERM - 1);
                    }

                    if (m != 0)
                    {
                        var tempSamples = new int[Defines.MAX_TERM];

                        for (var t = 0; t < SamplesA.Length; t++) tempSamples[t] = SamplesA[t];

                        for (k = 0; k < Defines.MAX_TERM; k++, m++)
                            SamplesA[k] = tempSamples[m & (Defines.MAX_TERM - 1)];

                        Array.Copy(SamplesB, 0, tempSamples, 0, SamplesB.Length);

                        for (k = 0; k < Defines.MAX_TERM; k++, m++)
                            SamplesB[k] = tempSamples[m & (Defines.MAX_TERM - 1)];
                    }

                    break;
            }

            WeightA = (short)weightA;
            WeightB = (short)weightB;
        }

        internal void StereoPassCont(int[] buffer, long sampleCount, int bufIdx)
        {
            int delta = Delta, weightA = WeightA, weightB = WeightB;
            int tptr;
            int samA, samB;
            int k, i;
            int bufferIndex;
            var endIndex = bufIdx + sampleCount * 2;

            switch (Term)
            {
                case 17:
                    for (bufferIndex = bufIdx; bufferIndex < endIndex; bufferIndex += 2)
                    {
                        samA = 2 * buffer[bufferIndex - 2] - buffer[bufferIndex - 4];

                        buffer[bufferIndex] = (int)((weightA * (long)samA + 512) >> 10) + (samB = buffer[bufferIndex]);

                        if (samA != 0 && samB != 0)
                            weightA += (((samA ^ samB) >> 30) | 1) * delta;

                        //update_weight (weight_A, delta, sam_A, sam_B);

                        samA = 2 * buffer[bufferIndex - 1] - buffer[bufferIndex - 3];

                        buffer[bufferIndex + 1] =
                            (int)((weightB * (long)samA + 512) >> 10) + (samB = buffer[bufferIndex + 1]);

                        if (samA != 0 && samB != 0)
                            weightB += (((samA ^ samB) >> 30) | 1) * delta;
                    }

                    SamplesB[0] = buffer[bufferIndex - 1];
                    SamplesA[0] = buffer[bufferIndex - 2];
                    SamplesB[1] = buffer[bufferIndex - 3];
                    SamplesA[1] = buffer[bufferIndex - 4];
                    break;

                case 18:
                    for (bufferIndex = bufIdx; bufferIndex < endIndex; bufferIndex += 2)
                    {
                        samA = (3 * buffer[bufferIndex - 2] - buffer[bufferIndex - 4]) >> 1;

                        buffer[bufferIndex] = (int)((weightA * (long)samA + 512) >> 10) + (samB = buffer[bufferIndex]);

                        if (samA != 0 && samB != 0)
                            weightA += (((samA ^ samB) >> 30) | 1) * delta;

                        samA = (3 * buffer[bufferIndex - 1] - buffer[bufferIndex - 3]) >> 1;

                        buffer[bufferIndex + 1] =
                            (int)((weightB * (long)samA + 512) >> 10) + (samB = buffer[bufferIndex + 1]);

                        if (samA != 0 && samB != 0)
                            weightB += (((samA ^ samB) >> 30) | 1) * delta;
                    }

                    SamplesB[0] = buffer[bufferIndex - 1];
                    SamplesA[0] = buffer[bufferIndex - 2];
                    SamplesB[1] = buffer[bufferIndex - 3];
                    SamplesA[1] = buffer[bufferIndex - 4];
                    break;

                case -1:
                    for (bufferIndex = bufIdx; bufferIndex < endIndex; bufferIndex += 2)
                    {
                        buffer[bufferIndex] = (int)((weightA * (long)buffer[bufferIndex - 1] + 512) >> 10)
                                              + (samA = buffer[bufferIndex]);

                        if ((buffer[bufferIndex - 1] ^ samA) < 0)
                        {
                            if (buffer[bufferIndex - 1] != 0 && samA != 0 && (weightA -= delta) < -1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }
                        else
                        {
                            if (buffer[bufferIndex - 1] != 0 && samA != 0 && (weightA += delta) > 1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }

                        buffer[bufferIndex + 1] = (int)((weightB * (long)buffer[bufferIndex] + 512) >> 10)
                                                  + (samA = buffer[bufferIndex + 1]);

                        if ((buffer[bufferIndex] ^ samA) < 0)
                        {
                            if (buffer[bufferIndex] != 0 && samA != 0 && (weightB -= delta) < -1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }
                        else
                        {
                            if (buffer[bufferIndex] != 0 && samA != 0 && (weightB += delta) > 1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }
                    }

                    SamplesA[0] = buffer[bufferIndex - 1];
                    break;

                case -2:
                    //sam_A = 0;
                    //sam_B = 0;

                    for (bufferIndex = bufIdx; bufferIndex < endIndex; bufferIndex += 2)
                    {
                        buffer[bufferIndex + 1] = (int)((weightB * (long)buffer[bufferIndex - 2] + 512) >> 10)
                                                  + (samA = buffer[bufferIndex + 1]);

                        if ((buffer[bufferIndex - 2] ^ samA) < 0)
                        {
                            if (buffer[bufferIndex - 2] != 0 && samA != 0 && (weightB -= delta) < -1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }
                        else
                        {
                            if (buffer[bufferIndex - 2] != 0 && samA != 0 && (weightB += delta) > 1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }

                        buffer[bufferIndex] = (int)((weightA * (long)buffer[bufferIndex + 1] + 512) >> 10)
                                              + (samA = buffer[bufferIndex]);

                        if ((buffer[bufferIndex + 1] ^ samA) < 0)
                        {
                            if (buffer[bufferIndex + 1] != 0 && samA != 0 && (weightA -= delta) < -1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }
                        else
                        {
                            if (buffer[bufferIndex + 1] != 0 && samA != 0 && (weightA += delta) > 1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }
                    }

                    SamplesB[0] = buffer[bufferIndex - 2];
                    break;

                case -3:
                    for (bufferIndex = bufIdx; bufferIndex < endIndex; bufferIndex += 2)
                    {
                        buffer[bufferIndex] = (int)((weightA * (long)buffer[bufferIndex - 1] + 512) >> 10)
                                              + (samA = buffer[bufferIndex]);

                        if ((buffer[bufferIndex - 1] ^ samA) < 0)
                        {
                            if (buffer[bufferIndex - 1] != 0 && samA != 0 && (weightA -= delta) < -1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }
                        else
                        {
                            if (buffer[bufferIndex - 1] != 0 && samA != 0 && (weightA += delta) > 1024)
                            {
                                if (weightA < 0) weightA = -1024;
                                else weightA = 1024;
                            }
                        }

                        buffer[bufferIndex + 1] = (int)((weightB * (long)buffer[bufferIndex - 2] + 512) >> 10)
                                                  + (samA = buffer[bufferIndex + 1]);

                        if ((buffer[bufferIndex - 2] ^ samA) < 0)
                        {
                            if (buffer[bufferIndex - 2] != 0 && samA != 0 && (weightB -= delta) < -1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }
                        else
                        {
                            if (buffer[bufferIndex - 2] != 0 && samA != 0 && (weightB += delta) > 1024)
                            {
                                if (weightB < 0) weightB = -1024;
                                else weightB = 1024;
                            }
                        }
                    }

                    SamplesA[0] = buffer[bufferIndex - 1];
                    SamplesB[0] = buffer[bufferIndex - 2];
                    break;

                default:
                    tptr = bufIdx - Term * 2;

                    for (bufferIndex = bufIdx; bufferIndex < endIndex; bufferIndex += 2)
                    {
                        buffer[bufferIndex] = (int)((weightA * (long)buffer[tptr] + 512) >> 10)
                                              + (samA = buffer[bufferIndex]);

                        if (buffer[tptr] != 0 && samA != 0)
                            weightA += (((buffer[tptr] ^ samA) >> 30) | 1) * delta;

                        buffer[bufferIndex + 1] = (int)((weightB * (long)buffer[tptr + 1] + 512) >> 10)
                                                  + (samA = buffer[bufferIndex + 1]);

                        if (buffer[tptr + 1] != 0 && samA != 0)
                            weightB += (((buffer[tptr + 1] ^ samA) >> 30) | 1) * delta;

                        tptr += 2;
                    }

                    bufferIndex--;

                    for (k = Term - 1, i = 8; i > 0; k--)
                    {
                        i--;
                        SamplesB[k & (Defines.MAX_TERM - 1)] = buffer[bufferIndex];
                        bufferIndex--;
                        SamplesA[k & (Defines.MAX_TERM - 1)] = buffer[bufferIndex];
                        bufferIndex--;
                    }

                    break;
            }

            WeightA = (short)weightA;
            WeightB = (short)weightB;
        }

        internal void MonoPass(int[] buffer, long sampleCount, int bufIdx)
        {
            int delta = Delta, weightA = WeightA;
            int samA;
            int m, k;
            int bptrCounter;

            switch (Term)
            {
                case 17:
                    for (bptrCounter = bufIdx; bptrCounter < bufIdx + sampleCount; bptrCounter++)
                    {
                        samA = 2 * SamplesA[0] - SamplesA[1];
                        SamplesA[1] = SamplesA[0];
                        SamplesA[0] = (int)((weightA * (long)samA + 512) >> 10) + buffer[bptrCounter];

                        if (samA != 0 && buffer[bptrCounter] != 0)
                        {
                            if ((samA ^ buffer[bptrCounter]) < 0) weightA = weightA - delta;
                            else weightA = weightA + delta;
                        }

                        buffer[bptrCounter] = SamplesA[0];
                    }

                    break;

                case 18:
                    for (bptrCounter = bufIdx; bptrCounter < bufIdx + sampleCount; bptrCounter++)
                    {
                        samA = (3 * SamplesA[0] - SamplesA[1]) >> 1;
                        SamplesA[1] = SamplesA[0];
                        SamplesA[0] = (int)((weightA * (long)samA + 512) >> 10) + buffer[bptrCounter];

                        if (samA != 0 && buffer[bptrCounter] != 0)
                        {
                            if ((samA ^ buffer[bptrCounter]) < 0) weightA = weightA - delta;
                            else weightA = weightA + delta;
                        }

                        buffer[bptrCounter] = SamplesA[0];
                    }

                    break;

                default:
                    for (m = 0, k = Term & (Defines.MAX_TERM - 1), bptrCounter = bufIdx;
                         bptrCounter < bufIdx + sampleCount;
                         bptrCounter++)
                    {
                        samA = SamplesA[m];
                        SamplesA[k] = (int)((weightA * (long)samA + 512) >> 10) + buffer[bptrCounter];

                        if (samA != 0 && buffer[bptrCounter] != 0)
                        {
                            if ((samA ^ buffer[bptrCounter]) < 0) weightA = weightA - delta;
                            else weightA = weightA + delta;
                        }

                        buffer[bptrCounter] = SamplesA[k];
                        m = (m + 1) & (Defines.MAX_TERM - 1);
                        k = (k + 1) & (Defines.MAX_TERM - 1);
                    }

                    if (m != 0)
                    {
                        var tempSamples = new int[Defines.MAX_TERM];

                        Array.Copy(SamplesA, 0, tempSamples, 0, SamplesA.Length);

                        for (k = 0; k < Defines.MAX_TERM; k++, m++)
                            SamplesA[k] = tempSamples[m & (Defines.MAX_TERM - 1)];
                    }

                    break;
            }

            WeightA = (short)weightA;
        }
    }
}
