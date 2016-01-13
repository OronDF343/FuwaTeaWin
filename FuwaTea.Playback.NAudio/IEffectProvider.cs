using NAudio.Wave;

namespace FuwaTea.Playback.NAudio
{
    public interface IEffectProvider : INAudioExtension
    {
        ISampleProvider ApplyEffect(ISampleProvider src);
    }
}
