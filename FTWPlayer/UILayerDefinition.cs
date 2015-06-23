using LayerFramework.Attributes;
using LayerFramework.Interfaces;

namespace FTWPlayer
{
    [LayerDefinition("WPF_GUI_Parts", typeof(UIPartAttribute), typeof(IUIPart))]
    public class UILayerDefinition : ILayerDefinition
    {
    }
}
