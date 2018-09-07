using System;
using System.Collections.Generic;
using DryIocAttributes;

namespace FuwaTea.Audio.Files.Impl
{
    [Reuse(ReuseType.Singleton)]
    public class FileProtocolHandler : IProtocolHandler
    {
        private const string FileProtocolString = "file";

        public IEnumerable<string> SupportedFormats => new[] { FileProtocolString };
        public bool CanHandle(IFileLocationInfo ti)
        {
            return string.Equals(ti.Protocol, FileProtocolString, StringComparison.OrdinalIgnoreCase);
        }

        public IFileHandle Handle(IFileLocationInfo ti) => new StandardFileHandle(ti);
    }
}
