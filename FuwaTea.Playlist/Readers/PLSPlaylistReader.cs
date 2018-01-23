using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using DryIocAttributes;
using FuwaTea.Lib;
using log4net;

namespace FuwaTea.Playlist.Readers
{
    //[PlaylistHandler("PLS Playlist Reader")]
    [Export(typeof(IPlaylistReader))]
    [Reuse(ReuseType.Singleton)]
    public class PLSPlaylistReader : IPlaylistReader
    {
        public IEnumerable<string> SupportedFileTypes => new[] { "pls|PLS Playlist" };

        public void LoadPlaylistFiles(string path, IPlaylist playlist)
        {
            // NOTE: All strings are case sensitive! See: http://schworak.com/blog/e41/extended-pls-plsv2/
            // Optimized search for header
            var linesEnumerator = File.ReadLines(path).GetEnumerator();
            do {
                if (!string.IsNullOrWhiteSpace(linesEnumerator.Current)) break;
            } while (linesEnumerator.MoveNext());
            // We should have found it now
            if (linesEnumerator.Current != "[playlist]")
                throw new InvalidDataException("Invalid file format!");
            // Find the version and number of entries, while adding the rest of the lines to a list
            int numEntries = -1, version = 1;
            var entries = new List<string>();
            while (linesEnumerator.MoveNext())
            {
                if (linesEnumerator.Current.StartsWith("Version"))
                    version = int.Parse(linesEnumerator.Current.Split("=".ToCharArray(), 2, StringSplitOptions.None)[1]);
                else if (linesEnumerator.Current.StartsWith("NumberOfEntries"))
                    numEntries = int.Parse(linesEnumerator.Current.Split("=".ToCharArray(), 2, StringSplitOptions.None)[1]);
                else entries.Add(linesEnumerator.Current);
            }
            // Check the version
            if (version != 2) LogManager.GetLogger(GetType()).Warn($"Expected PLSv2, got version {version}.");
            // Check if number of entries was specified
            if (numEntries == -1) LogManager.GetLogger(GetType()).Warn("NumberOfEntries not specified!");
            // Prepare a list of files to add
            // NOTE: currently we will ignore everything except the files
            var dir = Path.GetDirectoryName(path);
            var list = from entry in entries
                       where entry.StartsWith("File")
                       let parts = entry.Split("=".ToCharArray(), 2, StringSplitOptions.None)
                       orderby int.Parse(parts[0].Substring(4)) ascending
                       select PathUtils.ExpandRelativePath(dir, parts[1]);
            // NOTE: We haven't checked that the numbers of the entries were correct, nor that the number of entries was correct, who cares.
            playlist.Init(path, list);
        }
    }
}
