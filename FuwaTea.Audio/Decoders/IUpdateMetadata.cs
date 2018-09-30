using FuwaTea.Audio.Files;

namespace FuwaTea.Audio.Decoders
{
    public interface IUpdateMetadata
    {
        /// <summary>
        /// Update the metadata for the file.
        /// </summary>
        /// <remarks>
        /// Will update the <see cref="IFileLocationInfo.Metadata"/> property.
        /// This method should be as efficient as possible in terms of minimizing the amount of IO required to read the metadata.
        /// </remarks>
        /// <param name="file">The file.</param>
        /// <returns>True if any metadata was read.</returns>
        void UpdateMetadata(IFileHandle file);
    }
}