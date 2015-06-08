using NAudio.Wave;

namespace FuwaTea.Data.Playback.NAudio
{
    public interface IWaveStreamProvider : IDataElement
    {
        WaveStream CreateWaveStream(string path);
        bool IsSampleProvider { get; }
        bool CanResume { get; }
        bool CanSeek { get; }
    }
}
