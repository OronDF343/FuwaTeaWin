using System.Collections.Generic;
using System.ComponentModel.Composition;
using FuwaTea.Audio.Files;
using FuwaTea.Audio.Metadata;

namespace FuwaTea.Audio.Decoders
{
    [InheritedExport]
    public interface ISubTrackEnumerator : IDecoderBase
    {
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
    }
}
