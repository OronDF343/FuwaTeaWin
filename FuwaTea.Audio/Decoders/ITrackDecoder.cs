using System.ComponentModel.Composition;
using CSCore;
using FuwaTea.Audio.Files;

namespace FuwaTea.Audio.Decoders
{
    [InheritedExport]
    public interface ITrackDecoder : IDecoderBase
    {
        /// <summary>
        /// Begin decoding a file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>An <see cref="ISampleSource"/> which provides the decoded audio samples.</returns>
        ISampleSource Begin(IFileHandle file);
    }
}
