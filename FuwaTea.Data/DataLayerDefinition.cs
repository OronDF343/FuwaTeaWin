using LayerFramework.Attributes;
using LayerFramework.Interfaces;

namespace FuwaTea.Data
{
    [LayerDefinition("Data", typeof(DataElementAttribute), typeof(IDataElement))]
    public class DataLayerDefinition : ILayerDefinition { }
}
