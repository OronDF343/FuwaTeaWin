using System.ComponentModel.Composition;
using DryIocAttributes;
using JetBrains.Annotations;

namespace FuwaTea.Config
{
    /// <summary>
    /// Export the attributed class as a configuration page.
    /// </summary>
    /// <remarks>This is the only attribute that you need for the exporting, as long as the class implements <see cref="IConfigPage"/>, as it inherits from <see cref="T:DryIocAttributes.ExportManyAttribute" />.</remarks>
    [BaseTypeRequired(typeof(IConfigPage))]
    [MetadataAttribute]
    public class ConfigPageAttribute : ExportManyAttribute, IConfigPageMetadata
    {
        /// <summary>
        /// Export a configuration page.
        /// </summary>
        /// <param name="key">The unique identifier of the page.</param>
        public ConfigPageAttribute(string key)
        {
            Key = key;
            ContractKey = Key;
        }

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public bool IsPersistent { get; set; } = true;

        /// <inheritdoc />
        public bool IsUserEditable { get; set; } = true;
    }
}
