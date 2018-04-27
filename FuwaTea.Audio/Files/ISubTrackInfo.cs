namespace FuwaTea.Audio.Files
{
    public interface ISubTrackInfo : IFileLocationInfo
    {
        /// <summary>
        /// Gets the location info of the container file.
        /// </summary>
        IFileLocationInfo Container { get; }
        /// <summary>
        /// Gets the subtrack ID.
        /// </summary>
        string SubTrackId { get; }
    }
}
