using DryIoc;
using Newtonsoft.Json;

namespace Sage.Extensibility.Config
{
    public static class ConfigExtensions
    {
        public static T GetConfigPage<T>(this IContainer container, string key = null) where T : IConfigPage
        {
            return container.Resolve<T>(key);
        }

        public static string Serialize<T>(this T page) where T : IConfigPage
        {
            return JsonConvert.SerializeObject(page, Formatting.Indented);
        }

        public static T Deserialize<T>(this T page, string data) where T : IConfigPage
        {
            return JsonConvert.DeserializeObject<T>(data,
                                                    new JsonSerializerSettings
                                                    {
                                                        Converters =
                                                            { new ReuseCreationConverter<IConfigPage>(page) },
                                                        ObjectCreationHandling =
                                                            ObjectCreationHandling.Replace,
                                                        Formatting = Formatting.Indented
                                                    });
        }

        public static string Serialize(this IConfigPage page)
        {
            return JsonConvert.SerializeObject(page, Formatting.Indented);
        }

        public static IConfigPage Deserialize(this IConfigPage page, string data)
        {
            return JsonConvert.DeserializeObject<IConfigPage>(data,
                                                              new JsonSerializerSettings
                                                              {
                                                                  Converters =
                                                                      { new ReuseCreationConverter<IConfigPage>(page) },
                                                                  ObjectCreationHandling =
                                                                      ObjectCreationHandling.Replace,
                                                                  Formatting = Formatting.Indented
                                                              });
        }
    }
}
