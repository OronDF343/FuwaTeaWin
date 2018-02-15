using System.ComponentModel.Composition;
using JetBrains.Annotations;

namespace FuwaTea.Config
{
    [BaseTypeRequired(typeof(IConfigPage))]
    [MetadataAttribute]
    public class ConfigPageAttribute : ExportAttribute, IConfigPageMetadata
    {
        public ConfigPageAttribute(string key, string configFileKey)
            : base(key, typeof(IConfigPage))
        {
            Key = key;
            ConfigFileKey = configFileKey;
        }

        public string Key { get; }
        public string ConfigFileKey { get; }
        public bool ShouldDisplayInUI { get; set; } = true;
    }
}
