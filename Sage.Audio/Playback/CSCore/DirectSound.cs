using CSCore.SoundOut;

namespace Sage.Audio.Playback.CSCore
{
    public class DirectSound : CSCoreApiBase<DirectSoundOut, DirectSoundConfig>
    {
        public DirectSound(DirectSoundConfig config)
            : base(config) { }

        protected override DirectSoundOut CreateSoundOut()
        {
            var so = new DirectSoundOut(Config.DesiredLatency, Config.PlaybackThreadPriority);
            so.Device = Config.Device;
            return so;
        }
    }
}
