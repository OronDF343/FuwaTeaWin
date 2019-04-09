using System;
using System.IO;

namespace Sage.Audio.Files.Impl
{
    public class StandardFileHandle : FileHandleBase
    {
        public StandardFileHandle(IFileLocationInfo fli)
            : base(fli) { }

        public override Stream OpenStream(FileAccess fa, FileShare fs = FileShare.Read)
        {
            return File.Open(LocalPath, FileMode.Open, fa, fs);
        }

        public override DateTime LastWrite => File.GetLastWriteTime(LocalPath);
    }
}
