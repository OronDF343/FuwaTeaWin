using System;
using System.IO;
using FuwaTea.Audio.Metadata;

namespace FuwaTea.Audio.Files.Impl
{
    public abstract class FileHandleBase : IFileHandle
    {
        private readonly IFileLocationInfo _fli;

        public FileHandleBase(IFileLocationInfo fli)
        {
            _fli = fli;
        }

        public string Protocol => _fli.Protocol;
        public string AbsolutePath => _fli.AbsolutePath;
        public string Extension => _fli.Extension;
        public string Fragment => _fli.Extension;
        public Uri Uri => _fli.Uri;
        public IMetadata Metadata => _fli.Metadata;
        public abstract Stream OpenStream(FileAccess fa);
        public abstract DateTime LastWrite { get; }
    }
}
