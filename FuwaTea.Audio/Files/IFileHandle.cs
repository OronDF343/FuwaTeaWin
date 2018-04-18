using System;
using System.IO;

namespace FuwaTea.Audio.Files
{
    public interface IFileHandle : IFileLocationInfo, IDisposable
    {
        /// <summary>
        /// Gets the stream of the file's data.
        /// </summary>
        /// <remarks>
        /// Must return a new stream wrapper on every get!
        /// Sincee a new stream wrapper will be opened when this property is accessed. Therefore, avoid accessing it when not necessary.
        /// Disposal should be handled by the caller, even though not strictly required.
        /// </remarks>
        Stream Stream { get; }
    }
}
