using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace FTWPlayer.ViewModels.Utils
{
    public class SettingsTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var elem = container as FrameworkElement;
            var spv = item as SettingsPropertyValue;
            if (elem == null || spv == null) return base.SelectTemplate(item, container);
            if (spv.PropertyValue is bool)
            {
                // TODO: This code is a temporary workaround. Do something about this later.
                // Only the checkboxes seem to have a problem where the first checkbox is toggled by all of them.
                // This is fixed by creating a new binding each time
                // This is caused by the normal template allowing editing
                // "Name" doesn't tell the CfgDynBinding that it has changed in this case
                var chk = new FrameworkElementFactory(typeof(CheckBox));
                chk.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                chk.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
                // TODO IMPORTANT: Fix this!!!
                //chk.SetBinding(ToggleButton.IsCheckedProperty, new CfgDynBinding(new Binding("Name") { Mode = BindingMode.OneWay }) { Repository = Settings.Default });
                return new DataTemplate(typeof(bool)) { VisualTree = chk };
            }
            if (spv.PropertyValue is string) return elem.TryFindResource("StringDataTemplate") as DataTemplate;
            return elem.TryFindResource("DefaultTemplate") as DataTemplate ?? base.SelectTemplate(item, container);
        }
    }
}
