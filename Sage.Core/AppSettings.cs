using Serilog.Events;

namespace Sage.Core
{
    public class AppSettings
    {
        public AppSettings()
        {
            IsInstalled = false;
            FileLogEnabled = true;
#if DEBUG
            FileLogLevel = LogEventLevel.Debug;
            ConsoleLogLevel = LogEventLevel.Debug;
#else
            FileLogLevel = LogEventLevel.Information;
            ConsoleLogLevel = LogEventLevel.Warning;
#endif
        }
        public bool IsInstalled { get; set; }
        public LogEventLevel FileLogLevel { get; set; }
        public LogEventLevel ConsoleLogLevel { get; set; }
        public int InstanceCreationTimeout { get; set; }
        public bool FileLogEnabled { get; set; }
    }
}
