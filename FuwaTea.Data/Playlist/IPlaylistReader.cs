using System.Collections.Generic;

namespace FuwaTea.Data.Playlist
{
    public interface IPlaylistReader : IDataElement
    {
        /// <summary>
        /// Reads file entries from a playlist file.
        /// </summary>
        /// <param name="path">The path to the playlist file.</param>
        /// <returns>A <see cref="T:IEnumerable`1{string}"/> which will contain the entries from the playlist file.</returns>
        IEnumerable<string> GetPlaylistFiles(string path);
    }
}
