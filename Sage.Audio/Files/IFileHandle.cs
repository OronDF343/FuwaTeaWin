using System;
using System.IO;

namespace Sage.Audio.Files
{
    public interface IFileHandle : IFileLocationInfo
    {
        /// <summary>
        /// Gets the stream of the file's data.
        /// </summary>
        /// <remarks>
        /// Must return a new stream wrapper on every call!
        /// Disposal should be handled by the caller.
        /// </remarks>
        Stream OpenStream(FileAccess fa, FileShare fs = FileShare.Read);
        /// <summary>
        /// Date when the file was last modified (if available).
        /// </summary>
        DateTime LastWrite { get; }
    }
}
