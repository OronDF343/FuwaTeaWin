using System;
using FuwaTea.Annotations;
using LayerFramework.Attributes;

namespace FuwaTea.Data
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(IDataElement))]
    [AttributeUsage(AttributeTargets.Class)]
    public class DataElementAttribute : ElementAttribute
    {
        public DataElementAttribute(string elemName)
            : base(elemName) { }
    }
}
