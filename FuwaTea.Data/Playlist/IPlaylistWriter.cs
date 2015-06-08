using System.Collections.Generic;

namespace FuwaTea.Data.Playlist
{
    public interface IPlaylistWriter : IDataElement
    {
        /// <summary>
        /// Writes a list of entries to a playlist file.
        /// </summary>
        /// <param name="path">The path to the playlist file.</param>
        /// <param name="entries">The entries which will be written to the playlist file.</param>
        /// <param name="relativePaths"></param>
        void WritePlaylist(string path, IEnumerable<string> entries, bool relativePaths);
    }
}
