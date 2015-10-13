#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using NAudio.Wave;

namespace FuwaTea.Playback.NAudio.Utils
{
    /// <summary>
    /// Very simple sample provider supporting adjustable balance
    /// </summary>
    public class BalanceSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;

        /// <summary>
        /// Initializes a new instance of BalanceSampleProvider
        /// </summary>
        /// <param name="source">Source Sample Provider</param>
        public BalanceSampleProvider(ISampleProvider source)
        {
            if (source.WaveFormat.Channels != 2)
                throw new InvalidOperationException("Input wave format must be stereo!");
            _source = source;
            LeftVolume = 1.0f;
            RightVolume = 1.0f;
        }

        /// <summary>
        /// WaveFormat
        /// </summary>
        public WaveFormat WaveFormat => _source.WaveFormat;

        /// <summary>
        /// Reads samples from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="sampleCount">Number of samples desired</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int sampleCount)
        {
            int samplesRead;
            try { samplesRead = _source.Read(buffer, offset, sampleCount); }
            catch
            {
                return 0;
            }
            if (Math.Abs(RightVolume - 1.0f) < 0.005f && Math.Abs(LeftVolume - 1.0f) < 0.005f) return samplesRead;
            for (var n = 0; n < sampleCount; n += 2)
            {
                buffer[offset + n] *= LeftVolume;
                buffer[offset + n + 1] *= RightVolume;
            }
            return samplesRead;
        }

        /// <summary>
        /// Allows adjusting the left channel volume
        /// </summary>
        public float LeftVolume { get; set; }

        /// <summary>
        /// Allows adjusting the right channel volume
        /// </summary>
        public float RightVolume { get; set; }
    }
}
