using Avalonia;
using Avalonia.Logging.Serilog;
using FuwaTea.Core;
using FuwaTea.Extensibility;
using FuwaTea.Extensibility.Attributes;
using Sage.ViewModels;
using Sage.Views;
using Serilog;

[assembly: Extension("UI", ExtensibilityConstants.CurrentApiVersion)]

namespace Sage
{
    public static class Program
    {
        public static AppInstance AppInstance { get; set; }

        private static void Main(string[] args)
        {
            AppInstance = new AppInstance(args);
            if (!AppInstance.Init()) return;
            Log.Information("Building Avalonia app with main window");
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
