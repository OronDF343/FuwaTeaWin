using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModularFramework
{
    public static class FolderUtils
    {
        public static IEnumerable<string> SelectFiles(string folder, Func<string, bool> fileSelector)
        {
            return Directory.EnumerateFiles(folder).Where(fileSelector);
        }
    }
}
