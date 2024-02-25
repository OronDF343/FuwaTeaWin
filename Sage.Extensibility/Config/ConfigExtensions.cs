using DryIoc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sage.Extensibility.Config
{
    public static class ConfigExtensions
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
        };
        private static readonly JsonPopulator _jsonPopulator = new JsonPopulator();

        public static T GetConfigPage<T>(this IContainer container, string key = null) where T : IConfigPage
        {
            return container.Resolve<T>(key);
        }

        public static string Serialize<T>(this T page) where T : IConfigPage
        {
            return JsonSerializer.Serialize(page, _jsonOptions);
        }

        public static T Deserialize<T>(this T page, string data) where T : IConfigPage
        {
            _jsonPopulator.PopulateObject(page, data, _jsonOptions);
            return page;
        }

        public static string Serialize(this IConfigPage page)
        {
            return JsonSerializer.Serialize(page, _jsonOptions);
        }

        public static IConfigPage Deserialize(this IConfigPage page, string data)
        {
            _jsonPopulator.PopulateObject(page, data, _jsonOptions);
            return page;
        }
    }
}
