using FuwaTea.Audio.Files;

namespace FuwaTea.Audio.Decoders
{
    public interface IDecoderBase
    {
        /// <summary>
        /// Gets an array of file extensions supported by this decoder.
        /// </summary>
        /// <remarks>It is preferable that the extensions do not include a leading period.</remarks>
        string[] Extensions { get; }
        /// <summary>
        /// Check if given file is supported by this decoder.
        /// </summary>
        /// <remarks>
        /// This method is not intended to simply check the file path - it should assume the file path has already been checked against <see cref="Extensions"/>.
        /// Instead, this method allows the decoder to perform additional checks on the file to determine if decoding of it is supported and/or identify formats of data from a streaming protocol.
        /// Ideally, this method should verify the file type by means other than the extension, usually by checking the magic value of the header.
        /// For example, a given FLAC file should begin with the ASCII string "fLaC". This can be verified easily using <see cref="DecoderUtilExtensions.VerifyMagic"/>.
        /// </remarks>
        /// <param name="file">The file.</param>
        /// <returns>True if this decoder can be used to decode the given file.</returns>
        bool CanDecode(IFileHandle file);
        /// <summary>
        /// Update the metadata for the file.
        /// </summary>
        /// <remarks>
        /// Will update the <see cref="IFileLocationInfo.Metadata"/> property.
        /// This method should be as efficient as possible in terms of minimizing the amount of IO required to read the metadata.
        /// </remarks>
        /// <param name="file">The file.</param>
        /// <returns>True if any metadata was read.</returns>
        bool UpdateMetadata(IFileHandle file);
    }
}