using System.ComponentModel;
using System.Linq;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using DryIocAttributes;

namespace FuwaTea.Audio
{
    [Reuse(ReuseType.Transient)]
    public class Wasapi : CSCorePlaybackApiBase<WasapiOut, WasapiConfig>
    {
        public Wasapi(WasapiConfig config)
            : base(config) { }
        
        protected override void CreateSoundOut()
        {
            SoundOut = new WasapiOut(Config.UseEventSync,
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
            if (dev == null) throw new AudioDeviceNotReadyException(Config.Device);
        }

        protected override void ConfigOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(WasapiConfig.UseChannelMixingMatrices)) SoundOut.UseChannelMixingMatrices = Config.UseChannelMixingMatrices;
            else base.ConfigOnPropertyChanged(sender, args);
        }
    }
}
