using System.Collections.Generic;
using System.IO;

namespace FuwaTea.Audio.Files.Impl
{
    public class FileProtocolHandler : IProtocolHandler
    {
        private const string FileProtocolString = "file";

        public IEnumerable<string> SupportedFormats => new[] { FileProtocolString };
        public bool CanHandle(IFileLocationInfo ti)
        {
            return File.Exists(ti.LocalPath);
        }

        public IFileHandle Handle(IFileLocationInfo ti) => new StandardFileHandle(ti);
    }
}
