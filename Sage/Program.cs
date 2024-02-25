using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Sage.Core;
using Sage.Extensibility;
using Sage.Extensibility.Attributes;
using Serilog;

[assembly: Extension("UI", ExtensibilityConstants.CurrentApiVersion)]

namespace Sage
{
    public static class Program
    {
        public static AppInstance AppInstance { get; set; }

        [STAThread]
        private static void Main(string[] args)
        {
            AppInstance = new AppInstance(args);
            if (!AppInstance.Init()) return;
            Log.Information("Building Avalonia app with main window");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            Log.Information("Main window exited; Shutting down");
            AppInstance.Dispose();
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToTrace();
    }
}
