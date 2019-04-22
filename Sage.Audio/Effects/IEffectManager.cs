using System.ComponentModel.Composition;
using Sage.Lib.Models;

namespace Sage.Audio.Effects
{
    [InheritedExport]
    public interface IEffectManager : IImplementationMultiSelector<IEffect>
    {
    }
}
