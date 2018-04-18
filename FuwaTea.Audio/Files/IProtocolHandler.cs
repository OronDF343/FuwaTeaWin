using System.ComponentModel.Composition;

namespace FuwaTea.Audio.Files
{
    [InheritedExport]
    public interface IProtocolHandler
    {
        /// <summary>
        /// Gets an array of URI protocols supported by this decoder.
        /// </summary>
        /// <remarks>
        /// For example, the value "foo" corresponds to paths beginning with "foo://"
        /// Local paths are governed by the "file" protocol.
        /// </remarks>
        string[] Protocols { get; }

        /// <summary>
        /// Open a stream of the file data.
        /// </summary>
        /// <remarks>
        /// A protocol handler may add or modify the file's location information. In fact, adding an extension is encouraged when none is specified in the path. The more complete the information, the better.
        /// </remarks>
        /// <param name="file">The file's location information.</param>
        /// <returns>A stream.</returns>
        IFileHandle OpenFile(IFileLocationInfo file);
    }
}
