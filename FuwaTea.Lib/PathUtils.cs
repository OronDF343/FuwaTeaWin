using System;
using System.Collections.Generic;
using System.IO;

namespace FuwaTea.Lib
{
    public static class PathUtils
    {
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");
            if (toPath.StartsWith("file:")) return toPath.Remove(0, 5); // changed

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        public static IEnumerable<FileInfo> EnumerateFilesEx(this DirectoryInfo path)
        {
            var queue = new Queue<DirectoryInfo>();
            queue.Enqueue(path);
            IEnumerable<FileSystemInfo> tmp;
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    tmp = path.GetFiles();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    continue;
                }

                foreach (var t in tmp)
                    yield return t as FileInfo;

                try
                {
                    tmp = path.GetDirectories();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    continue;
                }

                foreach (var subDir in tmp)
                    queue.Enqueue(subDir as DirectoryInfo);
            }
        }
    }
}
