using FuwaTea.Audio.Metadata;

namespace FuwaTea.Audio.Files
{
    public interface IFileLocationInfo
    {
        /// <summary>
        /// Gets the full path string of the file.
        /// </summary>
        /// <remarks>This is not necessarily a standard path - it may include additional info such as the sub-track ID.</remarks>
        string Path { get; }
        /// <summary>
        /// Gets the actual location of the file.
        /// </summary>
        /// <remarks>Either a local or network path, or a URL.</remarks>
        string Location { get; }
        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <remarks>Will be null or empty if none is specified (such is the case with certain streaming protocols).</remarks>
        string Extension { get; }
        /// <summary>
        /// Gets the protocol of the file path.
        /// </summary>
        /// <remarks>For example: "file", "http", ...</remarks>
        string Protocol { get; }
        /// <summary>
        /// Gets the metadata provided by one or more of: The decoder, the subtrack container / enumerator, the playlist and the library cache.
        /// </summary>
        IAudioMetadata Metadata { get; }
    }
}
