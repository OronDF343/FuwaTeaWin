using DryIoc;

namespace FuwaTea.Config
{
    public static class ConfigExtensions
    {
        public static T GetConfigPage<T>(this IContainer container, string key = null) where T : IConfigPage
        {
            return container.Resolve<T>(key);
        }
    }
}
