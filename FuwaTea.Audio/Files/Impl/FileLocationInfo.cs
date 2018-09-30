﻿using System;
using System.Collections.Generic;
using FuwaTea.Audio.Metadata;
using FuwaTea.Lib;

namespace FuwaTea.Audio.Files.Impl
{
    public class FileLocationInfo : IFileLocationInfo
    {
        public FileLocationInfo(Uri uri)
        {
            Uri = uri;
        }

        public string Protocol => Uri.Scheme;
        public string AbsolutePath => Uri.AbsolutePath;
        public string Extension => Uri.AbsolutePath.GetExtension();
        public string Fragment => Uri.Fragment;
        public Uri Uri { get; }
        public IDictionary<MetadataSource, IMetadata> Metadata { get; } = new Dictionary<MetadataSource, IMetadata>();
    }
}
