using CSCore;

namespace FuwaTea.Audio.Effects
{
    public abstract class EffectBase : SampleAggregatorBase, IEffect
    {
        public EffectBase(ISampleSource baseSource)
            : base(baseSource) { }
    }
}
