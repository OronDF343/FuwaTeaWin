using System.Windows;

namespace FuwaTea.Wpf.Helpers
{
    public static class ExtensionMethods
    {
        public static bool IsPropertySet(this DependencyObject d, DependencyProperty dp)
        {
            return d.ReadLocalValue(dp) != DependencyProperty.UnsetValue;
        }
    }
}
