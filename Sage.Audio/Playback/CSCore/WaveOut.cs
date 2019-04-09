﻿using System.Linq;
using CSCore.SoundOut;

namespace Sage.Audio.Playback.CSCore
{
    public class WaveOut : CSCoreApiBase<global::CSCore.SoundOut.WaveOut, WaveOutConfig>
    {
        public WaveOut(WaveOutConfig config)
            : base(config) { }

        protected override global::CSCore.SoundOut.WaveOut CreateSoundOut()
        {
            var so = new global::CSCore.SoundOut.WaveOut(Config.DesiredLatency)
            {
                UseChannelMixingMatrices = Config.UseChannelMixingMatrices
            };
            var dev = WaveOutDevice.EnumerateDevices().FirstOrDefault(d => d.DeviceId == Config.Device);
            so.Device = dev ?? throw new AudioDeviceNotReadyException(Config.Device.ToString());
            return so;
        }
    }
}