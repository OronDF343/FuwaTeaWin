using System.ComponentModel.Composition;

namespace FuwaTea.Audio.Effects
{
    [MetadataAttribute]
    public class EffectAttribute : ExportAttribute, IEffectMetadata
    {
        public EffectAttribute() 
            : base(typeof(IEffect))
        {

        }
    }
}
