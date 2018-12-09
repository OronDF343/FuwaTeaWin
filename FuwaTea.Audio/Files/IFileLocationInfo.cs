using System;
using System.Collections.Generic;
using FuwaTea.Audio.Metadata;

namespace FuwaTea.Audio.Files
{
    public interface IFileLocationInfo
    {
        /// <summary>
        /// Gets the protocol of the file path.
        /// </summary>
        /// <remarks>
        /// For example: "file", "http", ...
        /// </remarks>
        string Protocol { get; }
        /// <summary>
        /// Gets the absolute path of the file.
        /// </summary>
        string LocalPath { get; }
        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <remarks>Will be null or empty if none is specified (such is the case with certain streaming protocols).</remarks>
        string Extension { get; }
        /// <summary>
        /// Gets the fragment in the file URI.
        /// </summary>
        /// <remarks>The URI fragment can be used to store additional info such as the subtrack ID.</remarks>
        string Fragment { get; }
        /// <summary>
        /// Gets the full URI (Uniform Resource Identifier) of the file.
        /// </summary>
        /// <remarks>Many other string properties are derived from this one.</remarks>
        Uri Uri { get; }
        /// <summary>
        /// Gets the metadata provided by one of: The decoder, the subtrack container / enumerator, the playlist and the library cache.
        /// </summary>
        IDictionary<MetadataSource, IMetadata> Metadata { get; }
    }
}
