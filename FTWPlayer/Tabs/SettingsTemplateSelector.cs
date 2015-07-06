using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace FTWPlayer.Tabs
{
    public class SettingsTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var elem = container as FrameworkElement;
            var spv = item as SettingsPropertyValue;
            if (elem == null || spv == null) return base.SelectTemplate(item, container);
            if (spv.PropertyValue is bool) return elem.TryFindResource("BooleanDataTemplate") as DataTemplate;
            if (spv.PropertyValue is string) return elem.TryFindResource("StringDataTemplate") as DataTemplate;
            return elem.TryFindResource("DefaultTemplate") as DataTemplate ?? base.SelectTemplate(item, container);
        }
    }
}
