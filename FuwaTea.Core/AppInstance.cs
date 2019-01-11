using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FuwaTea.Extensibility;
using FuwaTea.Extensibility.Config;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.RollingFileAlternate;

namespace FuwaTea.Core
{
    /// <summary>
    /// The application instance context.
    /// </summary>
    public sealed class AppInstance
    {
        public IReadOnlyList<string> Args { get; }
        public bool IsInstalled { get; private set; }
        public ExtensibilityContainer ExtensibilityContainer { get; private set; }
        internal ExtensionCache ExtensionCache { get; private set; }
        public IPlatformSupport PlatformSupport { get; private set; }

        public event UnhandledExceptionEventHandler UnhandledException;

        /// <summary>
        /// Create the instance, and auto-initialize only the bare-minimum requirements for running the application, such as logging.
        /// </summary>
        /// <param name="mainArgs">The supplied command-line arguments.</param>
        public AppInstance(string[] mainArgs)
        {
            // Save command-line arguments
            Args = new ReadOnlyCollection<string>(mainArgs);

            // Configure the logger
            // TODO: Get logging switches from args
            var logLevel = LogEventLevel.Debug;
            var logDir = AppConstants.MakeAppPath(Environment.SpecialFolder.LocalApplicationData, AppConstants.LogsDirName);
            Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Is(logLevel)
                                                  .WriteTo.RollingFileAlternate(logDir, retainedFileCountLimit: 20,
                                                                                fileSizeLimitBytes: 1048576).MinimumLevel.Is(logLevel)
                                                  .CreateLogger();
            
            // Track unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Log.Fatal("An unhandled exception occured: ", (Exception)args.ExceptionObject);
                OnUnhandledException(args);
            };
            // Now we are ready for Init()
            Log.Information("AppInstance has started. Hello, world!");
        }

        // If we remove sealed: protected virtual
        private void OnUnhandledException(UnhandledExceptionEventArgs e)
        {
            UnhandledException?.Invoke(this, e);
        }

        /// <summary>
        /// Fully initialize the application to the desired state (in accordance with the command-line arguments).
        /// </summary>
        /// <returns>A boolean value indicating whether GUI initialization should follow, or if the process should terminate.</returns>
        public bool Init()
        {
            Log.Debug("Main initialization started");
            // Create container
            ExtensibilityContainer = new ExtensibilityContainer();
            // Resolve platform support
            InitPlatform();
            // Process command-line arguments
            if (!ProcessClArgs()) return false;

            return true;
        }

        private bool ProcessClArgs()
        {
            if (Args.Contains(AppConstants.Arguments.UpdateFileAssociations))
                return false;
            if (Args.Contains(AppConstants.Arguments.DeleteFileAssociations))
                return false;
            if (Args.Contains(AppConstants.Arguments.FileAssociationsUi))
                return false;
            return true;
        }

        private void InitPlatform()
        {
            // Load 
        }

        private void InitConfig()
        {
            // Init config directories first
            var persistentConfigDir = AppConstants.MakeAppPath(Environment.SpecialFolder.ApplicationData, AppConstants.ConfigDirName);
            var nonPersistentConfigDir = AppConstants.MakeAppPath(Environment.SpecialFolder.LocalApplicationData, AppConstants.ConfigDirName);
            ExtensibilityContainer.SetConfigDirs(persistentConfigDir, nonPersistentConfigDir);
            // Add required dynamic pages
            ExtensionCache = ExtensibilityContainer.RegisterConfigPage<ExtensionCache>(new ConfigPageMetadata(nameof(ExtensionCache), false, false));
        }
    }
}
