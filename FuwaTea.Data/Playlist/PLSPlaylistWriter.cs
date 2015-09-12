﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using FuwaTea.Common.Models;
using FuwaTea.Lib;

namespace FuwaTea.Data.Playlist
{
    [DataElement("PLS Playlist Writer")]
    public class PLSPlaylistWriter : IPlaylistWriter
    {
        public IEnumerable<string> SupportedFileTypes => new[] { ".pls" };

        public void WritePlaylist(string path, IPlaylist playlist, bool relativePaths)
        {
            var dir = Path.GetDirectoryName(path);
            using (var file = new StreamWriter(path, false, Encoding.UTF8))
            {
                file.WriteLine("[playlist]");
                for (var i = 0; i < playlist.Count; ++i)
                {
                    file.WriteLine($"File{i}=" + (relativePaths ? PathUtils.MakeRelativePath(dir, playlist[i].FilePath) : playlist[i].FilePath));
                    // Check if it is a stream
                    if (!playlist[i].FilePath.StartsWith("http://") && !playlist[i].FilePath.StartsWith("https://"))
                    {
                        if (!string.IsNullOrWhiteSpace(playlist[i].Tag?.Title))
                            file.WriteLine($"Title{i}={playlist[i].Tag.Title}");
                        file.WriteLine($"Length{i}={playlist[i].Duration.TotalSeconds.ToString("#")}");
                    }
                    else
                    {
                        // Currently the stream title is stored as the first performer. TODO: change this when needed
                        if (!string.IsNullOrWhiteSpace(playlist[i].Tag?.FirstPerformer))
                            file.WriteLine($"Title{i}={playlist[i].Tag.FirstPerformer}");
                        // Indefinite length
                        file.WriteLine($"Length{i}=-1");
                    }
                }
                file.WriteLine($"NumberOfEntries={playlist.Count}");
                file.WriteLine("Version=2");
                file.WriteLine();
            }
        }
    }
}
