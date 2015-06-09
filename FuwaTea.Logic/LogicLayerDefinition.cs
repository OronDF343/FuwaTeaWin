using LayerFramework.Attributes;
using LayerFramework.Interfaces;

namespace FuwaTea.Logic
{
    [LayerDefinition("Logic", typeof(LogicElementAttribute), typeof(ILogicElement))]
    public class LogicLayerDefinition : ILayerDefinition { }
}
