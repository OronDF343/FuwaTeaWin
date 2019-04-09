using System.ComponentModel.Composition;
using DryIocAttributes;
using JetBrains.Annotations;

namespace Sage.Extensibility.Config
{
    /// <summary>
    /// Export the attributed class as a configuration page.
    /// </summary>
    /// <remarks>
    /// For importing to work, the following conditions must be met:
    /// * This attribute must be present
    /// * The class must implement <see cref="IConfigPage"/>
    /// * The [Export] attribute should be present
    /// </remarks>
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
