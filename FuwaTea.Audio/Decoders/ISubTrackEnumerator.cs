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
        IEnumerable<ISubTrackInfo> EnumerateSubTracks(IFileHandle file);
        /// <summary>
        /// Open a handle to a specific subtrack.
        /// </summary>
        /// <param name="container">A handle to the container file.</param>
        /// <param name="subTrackInfo">The subtrack info.</param>
        /// <returns>The subtrack handle.</returns>
        ISubTrackHandle OpenSubTrack(IFileHandle container, ISubTrackInfo subTrackInfo);
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
