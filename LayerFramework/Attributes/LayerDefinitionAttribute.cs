using System;
using FuwaTea.Annotations;
using LayerFramework.Interfaces;

namespace LayerFramework.Attributes
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(ILayerDefinition))]
    public class LayerDefinitionAttribute : Attribute
    {
        public string LayerName { get; set; }

        public Type AttributeType { get; set; }
        public Type InterfaceType { get; set; }

        public LayerDefinitionAttribute(string lName, Type attrType, Type iType)
        {
            LayerName = lName;
            AttributeType = attrType;
            InterfaceType = iType;
        }
    }
}
