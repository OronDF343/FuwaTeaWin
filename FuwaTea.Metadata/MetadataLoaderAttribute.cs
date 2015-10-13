using System;
using ModularFramework;
using ModularFramework.Attributes;

namespace FuwaTea.Metadata
{
    [BaseTypeRequired(typeof(IMetadataLoader))]
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class)]
    public class MetadataLoaderAttribute : ElementAttribute
    {
        public MetadataLoaderAttribute(string elemName) : base(elemName) { }
    }
}
