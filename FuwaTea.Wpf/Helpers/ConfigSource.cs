using System.Configuration;
using System.Windows;

namespace FuwaTea.Wpf.Helpers
{
    public class ConfigSource
    {
        public static readonly DependencyProperty RepositoryProperty =
            DependencyProperty.RegisterAttached("Repository", typeof(ApplicationSettingsBase), typeof(ConfigSource));

        public static ApplicationSettingsBase GetRepository(DependencyObject obj)
        {
            return (ApplicationSettingsBase)obj.GetValue(RepositoryProperty);
        }

        public static void SetRepository(DependencyObject obj, ApplicationSettingsBase value)
        {
            obj.SetValue(RepositoryProperty, value);
        }
    }
}
