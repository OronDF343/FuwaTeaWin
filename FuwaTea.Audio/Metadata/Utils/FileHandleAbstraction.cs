﻿using System.IO;
using FuwaTea.Audio.Files;
using File = TagLib.File;

namespace FuwaTea.Audio.Metadata.Utils
{
    public class FileHandleAbstraction : File.IFileAbstraction
    {
        private readonly IFileHandle _handle;

        public FileHandleAbstraction(IFileHandle handle) => _handle = handle;

        public string Name => _handle.AbsolutePath;
        
        public Stream ReadStream => _handle.OpenStream(FileAccess.Read);

        public Stream WriteStream => _handle.OpenStream(FileAccess.ReadWrite);

        public void CloseStream(Stream stream) => stream?.Close();
    }
}