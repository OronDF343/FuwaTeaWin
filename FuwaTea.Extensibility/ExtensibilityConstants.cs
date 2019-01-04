namespace FuwaTea.Extensibility
{
    /// <summary>
    /// Constants used for extensibility.
    /// </summary>
    public static class ExtensibilityConstants
    {
        /// <summary>
        /// API versions to allow loading automatically.
        /// </summary>
        public static readonly int[] ApiVersionWhitelist = { 1 };
        /// <summary>
        /// The current API version. The highest version to allow loading, unless it is in <see cref="ApiVersionWhitelist"/>.
        /// </summary>
        public const int CurrentApiVersion = 1;
        /// <summary>
        /// API versions to never allow loading.
        /// </summary>
        public static readonly int[] ApiVersionBlacklist = { 0 };
        /// <summary>
        /// Key for exporting auto-initialized services.
        /// </summary>
        public const string AutoInitKey = "autoinit";
    }
}
