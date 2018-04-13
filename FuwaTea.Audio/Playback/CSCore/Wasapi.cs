using System.ComponentModel;
using System.Linq;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using DryIocAttributes;

namespace FuwaTea.Audio.Playback.CSCore
{
    [Reuse(ReuseType.Transient)]
    public class Wasapi : CSCoreApiBase<WasapiOut, WasapiConfig>
    {
        public Wasapi(WasapiConfig config)
            : base(config) { }
        
        protected override WasapiOut CreateSoundOut()
        {
            var so = new WasapiOut(Config.UseEventSync,
                                     Config.UseExclusiveMode
                                         ? AudioClientShareMode.Exclusive
                                         : AudioClientShareMode.Shared,
                                     Config.DesiredLatency,
                                     Config.PlaybackThreadPriority)
            {
                UseChannelMixingMatrices = Config.UseChannelMixingMatrices
            };
            var dev = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render, DeviceState.Active)
                                        .FirstOrDefault(mmd => mmd.DevicePath == Config.Device);
            so.Device = dev ?? throw new AudioDeviceNotReadyException(Config.Device);
            return so;
        }

        protected override void ConfigOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(WasapiConfig.UseChannelMixingMatrices)) SoundOut.UseChannelMixingMatrices = Config.UseChannelMixingMatrices;
            else base.ConfigOnPropertyChanged(sender, args);
        }
    }
}
