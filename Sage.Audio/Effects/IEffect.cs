using System.ComponentModel.Composition;
using CSCore;

namespace Sage.Audio.Effects
{
    [InheritedExport]
    public interface IEffect : ISampleAggregator
    {
        new ISampleSource BaseSource { get; set; }
    }
}