using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Logging.Serilog;
using FuwaTea.Core;
using FuwaTea.Extensibility;
using Sage.ViewModels;
using Sage.Views;

[assembly: Extension("UI", ExtensibilityConstants.CurrentApiVersion)]

namespace Sage
{
    public static class Program
    {
        public static AppInstance AppInstance { get; set; }

        private static void Main(string[] args)
        {
            AppInstance = new AppInstance(args);
            if (!AppInstance.Init()) return; // 
            BuildAvaloniaApp().Start<MainWindow>(() => new MainWindowViewModel());
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug();
    }
}
