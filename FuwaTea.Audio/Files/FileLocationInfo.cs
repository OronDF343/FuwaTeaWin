﻿using System;
using System.Collections.Generic;
using FuwaTea.Audio.Metadata;
using FuwaTea.Lib;

namespace FuwaTea.Audio.Files
{
    public class FileLocationInfo : IFileLocationInfo
    {
        public FileLocationInfo(Uri uri)
        {
            Uri = uri;
        }

        public string Protocol => Uri.Scheme;
        public string LocalPath => Uri.LocalPath;
        public string Extension => Uri.LocalPath.GetExtension();
        public string Fragment => Uri.Fragment;
        public Uri Uri { get; }
        public IDictionary<MetadataSource, IMetadata> Metadata { get; } = new Dictionary<MetadataSource, IMetadata>();
    }
}