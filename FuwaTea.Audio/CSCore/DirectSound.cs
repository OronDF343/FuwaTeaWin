using CSCore.SoundOut;

namespace FuwaTea.Audio.CSCore
{
    public class DirectSound : CSCoreApiBase<DirectSoundOut, DirectSoundConfig>
    {
        public DirectSound(DirectSoundConfig config)
            : base(config) { }

        protected override DirectSoundOut CreateSoundOut()
        {
            var so = new DirectSoundOut(Config.DesiredLatency, Config.PlaybackThreadPriority)
            {
                Volume = Config.MasterVolume,
                Device = Config.Device 
            };
            return so;
        }
    }
}
