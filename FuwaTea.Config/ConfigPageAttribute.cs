using System.ComponentModel.Composition;
using JetBrains.Annotations;

namespace FuwaTea.Config
{
    [BaseTypeRequired(typeof(IConfigPage))]
    [MetadataAttribute]
    public class ConfigPageAttribute : ExportAttribute, IConfigPageMetadata
    {
        public ConfigPageAttribute(string key)
            : base(key, typeof(IConfigPage))
        {
            Key = key;
        }

        public string Key { get; }
        public bool IsPersistent { get; set; } = true;
        public bool IsUserEditable { get; set; } = true;
    }
}
