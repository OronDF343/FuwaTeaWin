using Serilog.Events;

namespace FuwaTea.Core
{
    public class AppSettings
    {
        public AppSettings()
        {
            IsInstalled = false;
#if DEBUG
            DefaultLogLevel = LogEventLevel.Debug;
#else
            DefaultLogLevel = LogEventLevel.Information;
#endif
        }
        public bool IsInstalled { get; set; }
        public LogEventLevel DefaultLogLevel { get; set; }
        public int InstanceCreationTimeout { get; set; }
    }
}
