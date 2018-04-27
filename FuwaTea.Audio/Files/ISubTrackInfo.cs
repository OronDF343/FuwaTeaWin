using System;
using System.Collections.Generic;
using System.Text;

namespace FuwaTea.Audio.Files
{
    public interface ISubTrackInfo : IFileLocationInfo
    {
        /// <summary>
        /// Gets the "extension" / format identifier of the container.
        /// </summary>
        /// <remarks>Will be null or empty if none is specified (such is the case with certain streaming protocols).</remarks>
        string ContainerExtension { get; }
        /// <summary>
        /// Gets the subtrack ID.
        /// </summary>
        string SubTrackId { get; }
    }
}
