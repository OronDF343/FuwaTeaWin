using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FuwaTea.Wpf.Helpers
{
    public static class ExtensionMethods
    {
        public static bool IsPropertySet(this DependencyObject d, DependencyProperty dp)
        {
            return d.ReadLocalValue(dp) != DependencyProperty.UnsetValue;
        }

        public static void UpdateBindingSources(this DependencyObject obj, params DependencyProperty[] properties)
        {
            foreach (var be in properties.Select(depProperty => BindingOperations.GetBindingExpression(obj, depProperty)))
            {
                be?.UpdateSource();
            }

            var count = VisualTreeHelper.GetChildrenCount(obj);
            for (var i = 0; i < count; i++)
            {
                //process child items recursively
                var childObject = VisualTreeHelper.GetChild(obj, i);
                UpdateBindingSources(childObject, properties);
            }
        }

        public static void UpdateBindingTargets(this DependencyObject obj, params DependencyProperty[] properties)
        {
            foreach (var be in properties.Select(depProperty => BindingOperations.GetBindingExpression(obj, depProperty)))
            {
                be?.UpdateTarget();
            }

            var count = VisualTreeHelper.GetChildrenCount(obj);
            for (var i = 0; i < count; i++)
            {
                //process child items recursively
                var childObject = VisualTreeHelper.GetChild(obj, i);
                UpdateBindingTargets(childObject, properties);
            }
        }
    }
}
