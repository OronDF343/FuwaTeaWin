using System;
using System.Collections.Generic;
using System.IO;
using Sage.Audio.Metadata;

namespace Sage.Audio.Files.Impl
{
    public abstract class FileHandleBase : IFileHandle
    {
        private readonly IFileLocationInfo _fli;

        public FileHandleBase(IFileLocationInfo fli)
        {
            _fli = fli;
        }

        public string Protocol => _fli.Protocol;
        public string LocalPath => _fli.LocalPath;
        public string Extension => _fli.Extension;
        public string Fragment => _fli.Extension;
        public Uri Uri => _fli.Uri;
        public IDictionary<MetadataSource, IMetadata> Metadata => _fli.Metadata;
        public abstract Stream OpenStream(FileAccess fa, FileShare fs = FileShare.Read);
        public abstract DateTime LastWrite { get; }
    }
}
