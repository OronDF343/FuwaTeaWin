﻿using System.Collections.Generic;
using DryIocAttributes;

namespace FuwaTea.Metadata.Tags
{
    //[MetadataLoader("TagLib# Tag Provider")]
    [Reuse(ReuseType.Singleton)]
    public class TagLibTagProvider : ITagProvider
    {
        public IEnumerable<string> SupportedFileTypes => new[]
        {
            "aac",
            "m4a",
            "aiff",
            "ape",
            "asf",
            "aud",
            "flac",
            "ogg",
            "mp3",
            "wav",
            "wv",
            "adts",
            "wma",
            "mka"
        };

        public Tag Create(string path)
        {
            return new TagLibTag(path);
        }
    }
}
