/*
** WvDemo.cs
**
** Copyright (c) 2010-2011 Peter McQuillan
**
** All Rights Reserved.
**                       
** Distributed under the BSD Software License (see license.txt)  
***/

namespace WavpackDecoder
{
    public class WavpackExtraUtils
    {
        // Reformat samples from longs in processor's native endian mode to
        // little-endian data with (possibly) less than 4 bytes / sample.
	
        public static byte[] FormatSamples(int bps, int[] src, long samcnt)
        {
            int temp;
            int counter = 0;
            int counter2 = 0;
            byte[] dst = new byte[4 * Defines.SAMPLE_BUFFER_SIZE];
		
            switch (bps)
            {
			
                case 1: 
                    while (samcnt > 0)
                    {
                        dst[counter] = (byte) (0x00FF & (src[counter] + 128));
                        counter++;
                        samcnt--;
                    }
                    break;
			
			
                case 2: 
                    while (samcnt > 0)
                    {
                        temp = src[counter2];
                        dst[counter] = (byte) temp;
                        counter++;
                        //dst[counter] = (byte) (SupportClass.URShift(temp, 8));
                        dst[counter] = (byte) (temp >> 8);
                        counter++;
                        counter2++;
                        samcnt--;
                    }
				
                    break;
			
			
                case 3: 
                    while (samcnt > 0)
                    {
                        temp = src[counter2];
                        dst[counter] = (byte) temp;
                        counter++;
                        dst[counter] = (byte)(temp >> 8);
                        counter++;
                        dst[counter] = (byte)(temp >> 16);
                        counter++;
                        counter2++;
                        samcnt--;
                    }
				
                    break;
			
			
                case 4: 
                    while (samcnt > 0)
                    {
                        temp = src[counter2];
                        dst[counter] = (byte) temp;
                        counter++;
                        dst[counter] = (byte) SupportClass.URShift(temp, 8);
                        counter++;
                        dst[counter] = (byte) SupportClass.URShift(temp, 16);
                        counter++;
                        dst[counter] = (byte) SupportClass.URShift(temp, 24);
                        counter++;
                        counter2++;
                        samcnt--;
                    }
				
                    break;
            }
		
            return dst;
        }
    }
}
