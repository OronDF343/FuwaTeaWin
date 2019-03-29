namespace Sage.Extensibility.Attributes
{
    /// <summary>
    /// Enumeration of operating system kinds.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="System.Runtime.InteropServices.OSPlatform"/>, this enumeration can be used in attributes.
    /// </remarks>
    public enum OSKind
    {
        /// <summary>
        /// Microsoft Windows
        /// </summary>
        Windows,
        /// <summary>
        /// Linux
        /// </summary>
        Linux,
        /// <summary>
        /// Apple macOS
        /// </summary>
        MacOS
    }
}
