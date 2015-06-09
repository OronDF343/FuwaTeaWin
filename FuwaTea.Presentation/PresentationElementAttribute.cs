using System;
using FuwaTea.Annotations;
using LayerFramework.Attributes;

namespace FuwaTea.Presentation
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(IPresentationElement))]
    [AttributeUsage(AttributeTargets.Class)]
    public class PresentationElementAttribute : ElementAttribute
    {
        public PresentationElementAttribute(string elemName)
            : base(elemName) { }
    }
}
