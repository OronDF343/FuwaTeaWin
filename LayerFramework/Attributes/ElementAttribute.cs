using System;
using FuwaTea.Annotations;
using LayerFramework.Interfaces;

namespace LayerFramework.Attributes
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(IBasicElement))]
    public abstract class ElementAttribute : Attribute
    {
        public string ElementName { get; set; }

        public ElementAttribute(string elemName)
        {
            ElementName = elemName;
        }
    }
}
