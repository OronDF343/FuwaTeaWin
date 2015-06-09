using LayerFramework.Attributes;
using LayerFramework.Interfaces;

namespace FuwaTea.Presentation
{
    [LayerDefinition("Presentation", typeof(PresentationElementAttribute), typeof(IPresentationElement))]
    public class PresentationLayerDefinition : ILayerDefinition { }
}
