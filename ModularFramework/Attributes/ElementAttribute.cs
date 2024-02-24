using System;

namespace ModularFramework.Attributes
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public abstract class ElementAttribute : Attribute
    {
        public string ElementName { get; set; }

        public ElementAttribute(string elemName)
        {
            ElementName = elemName;
        }
    }
}
