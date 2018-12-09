using System;
using System.IO;

namespace FuwaTea.Audio.Files.Impl
{
    public class StandardFileHandle : FileHandleBase
    {
        public StandardFileHandle(IFileLocationInfo fli)
            : base(fli) { }

        public override Stream OpenStream(FileAccess fa)
        {
            return File.Open(LocalPath, FileMode.Open, fa);
        }

        public override DateTime LastWrite => File.GetLastWriteTime(LocalPath);
    }
}
