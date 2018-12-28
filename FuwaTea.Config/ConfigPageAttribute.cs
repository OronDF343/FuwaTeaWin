using System.ComponentModel.Composition;
using DryIocAttributes;
using JetBrains.Annotations;

namespace FuwaTea.Config
{
    [BaseTypeRequired(typeof(IConfigPage))]
    [MetadataAttribute]
    public class ConfigPageAttribute : ExportManyAttribute, IConfigPageMetadata
    {
        public ConfigPageAttribute(string key)
        {
            Key = key;
            ContractKey = Key;
        }

        public string Key { get; }
        public bool IsPersistent { get; set; } = true;
        public bool IsUserEditable { get; set; } = true;
    }
}
