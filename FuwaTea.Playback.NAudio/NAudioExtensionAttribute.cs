using System;
using ModularFramework;
using ModularFramework.Attributes;

namespace FuwaTea.Playback.NAudio
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(INAudioExtension))]
    [AttributeUsage(AttributeTargets.Class)]
    public class NAudioExtensionAttribute : ElementAttribute
    {
        public NAudioExtensionAttribute(string elemName) : base(elemName) { }
    }
}
