using System;
using ModularFramework;
using ModularFramework.Attributes;

namespace FuwaTea.Playback.NAudio.Codecs
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(IWaveStreamProvider))]
    [AttributeUsage(AttributeTargets.Class)]
    public class CodecAttribute : ElementAttribute
    {
        public CodecAttribute(string elemName) : base(elemName) { }
    }
}
