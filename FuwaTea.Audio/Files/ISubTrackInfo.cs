namespace FuwaTea.Audio.Files
{
    public interface ISubTrackInfo : IFileLocationInfo
    {
        /// <summary>
        /// Gets the handle of the container file.
        /// </summary>
        IFileHandle Container { get; }
        /// <summary>
        /// Gets the subtrack ID.
        /// </summary>
        string SubTrackId { get; }
    }
}
