namespace FuwaTea.Extensibility.Config
{
    /// <summary>
    /// Standalone metadata for a configuration page.
    /// </summary>
    public class ConfigPageMetadata : IConfigPageMetadata
    {
        /// <summary>
        /// Create metadata.
        /// </summary>
        /// <param name="key">The unique identifier of the page.</param>
        /// <param name="isPersistent">Which location the page should be stored in.</param>
        /// <param name="isUserEditable">Whether it is a user-facing page</param>
        public ConfigPageMetadata(string key, bool isPersistent = true, bool isUserEditable = true)
        {
            Key = key;
            IsPersistent = isPersistent;
            IsUserEditable = isUserEditable;
        }

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public bool IsPersistent { get; }

        /// <inheritdoc />
        public bool IsUserEditable { get; }
    }
}
