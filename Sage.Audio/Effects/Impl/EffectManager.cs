using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Sage.Extensibility.Config;
using Sage.Lib.Models;

namespace Sage.Audio.Effects.Impl
{
    [ConfigPage(nameof(EffectManager)), Export]
    public class EffectManager : ImplementationMultiSelectorBase<IEffect>, IEffectManager
    {
        public EffectManager([ImportMany] IList<IEffect> implementations)
            : base(implementations.Distinct().ToList()) { }

        public override bool InsertImplementation(int index, IEffect ti)
        {
            return !SelectedImplementationsList.Contains(ti) && base.InsertImplementation(index, ti);
        }

        public override bool AppendImplementation(IEffect ti)
        {
            return !SelectedImplementationsList.Contains(ti) && base.AppendImplementation(ti);
        }
    }
}
