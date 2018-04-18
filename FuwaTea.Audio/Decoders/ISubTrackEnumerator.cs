using System.Collections.Generic;
using System.ComponentModel.Composition;
using FuwaTea.Audio.Files;
using FuwaTea.Audio.Metadata;

namespace FuwaTea.Audio.Decoders
{
    [InheritedExport]
    public interface ISubTrackEnumerator
    {
        /// <summary>
        /// Gets an array of file extensions supported by this decoder.
        /// </summary>
        /// <remarks>It is preferable that the extensions do not include a leading period.</remarks>
        string[] Extensions { get; }
        /// <summary>
        /// See <see cref="ITrackDecoder.CanDecode"/>.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>True if the file is supported.</returns>
        bool CanDecode(IFileHandle file);
        /// <summary>
        /// Enumerate the subtracks in the container file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The contained subtracks.</returns>
        IEnumerable<IFileHandle> EnumerateSubTracks(IFileHandle file);
        /// <summary>
        /// Gets a specific subtrack by ID.
        /// </summary>
        /// <param name="file">The container file.</param>
        /// <param name="subTrackId">The subtrack ID.</param>
        /// <returns>The subtrack info.</returns>
        IFileHandle GetSubTrack(IFileHandle file, string subTrackId);
        /// <summary>
        /// Get the general metadata for the container file.
        /// </summary>
        /// <remarks>
        /// This method should be as efficient as possible in terms of minimizing the amount of IO required to read the metadata.
        /// </remarks>
        /// <param name="file">The file.</param>
        /// <returns>The metadata.</returns>
        IAudioMetadata GetMetadata(IFileHandle file);
    }
}
