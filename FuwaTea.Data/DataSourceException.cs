using System;
using LayerFramework.Exceptions;

namespace FuwaTea.Data
{
    public class DataSourceException : LayerException
    {
        public DataSourceException(string path) { SourcePath = path; }
        public DataSourceException(string path, string message)
            : base(message) { SourcePath = path; }

        public DataSourceException(string path, Exception innerException)
            : base(innerException) { SourcePath = path; }

        public DataSourceException(string path, string message, Exception innerException)
            : base(message, innerException) { SourcePath = path; }

        public string SourcePath { get; set; }
        public override string LayerName { get { return "Data"; } }
    }
}
