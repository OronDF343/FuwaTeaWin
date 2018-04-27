using System.ComponentModel.Composition;
using CSCore;

namespace FuwaTea.Audio.Effects
{
    [InheritedExport]
    public interface IEffect
    {
        bool CanProcess(WaveFormat wf);
        ISampleAggregator Begin(ISampleSource ss);
    }
}