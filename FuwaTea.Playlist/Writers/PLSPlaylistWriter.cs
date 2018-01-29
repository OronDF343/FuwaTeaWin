using System.Collections.Generic;
using System.IO;
using System.Text;
using DryIocAttributes;
using FuwaTea.Lib;

namespace FuwaTea.Playlist.Writers
{
    //[PlaylistHandler("PLS Playlist Writer")]
    [Reuse(ReuseType.Singleton)]
    public class PLSPlaylistWriter : IPlaylistWriter
    {
        public IEnumerable<string> SupportedFileTypes => new[] { "pls|PLS Playlist" };

        public void WritePlaylist(string path, IPlaylist playlist, bool relativePaths)
        {
            using (var file = new StreamWriter(path, false, Encoding.UTF8))
            {
                file.WriteLine("[playlist]");
                for (var i = 0; i < playlist.Count; ++i)
                {
                    file.WriteLine($"File{i + 1}=" + (relativePaths ? PathUtils.MakeRelativePath(path, playlist[i].FilePath) : playlist[i].FilePath));
                    // Check if it is a stream
                    if (!playlist[i].FilePath.StartsWith("http://") && !playlist[i].FilePath.StartsWith("https://"))
                    {
                        if (!string.IsNullOrWhiteSpace(playlist[i].Tag?.Title))
                            file.WriteLine($"Title{i + 1}={playlist[i].Tag.Title}");
                        file.WriteLine($"Length{i + 1}={playlist[i].Duration.TotalSeconds.ToString("#")}");
                    }
                    else
                    {
                        // Currently the stream title is stored as the first performer. TODO: change this when needed
                        if (!string.IsNullOrWhiteSpace(playlist[i].Tag?.FirstPerformer))
                            file.WriteLine($"Title{i + 1}={playlist[i].Tag.FirstPerformer}");
                        // Indefinite length
                        file.WriteLine($"Length{i + 1}=-1");
                    }
                }
                file.WriteLine($"NumberOfEntries={playlist.Count}");
                file.WriteLine("Version=2");
                file.WriteLine();
            }
        }
    }
}
