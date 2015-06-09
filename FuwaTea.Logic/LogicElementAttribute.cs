using System;
using FuwaTea.Annotations;
using LayerFramework.Attributes;

namespace FuwaTea.Logic
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(ILogicElement))]
    [AttributeUsage(AttributeTargets.Class)]
    public class LogicElementAttribute : ElementAttribute
    {
        public LogicElementAttribute(string elemName)
            : base(elemName) { }
    }
}
