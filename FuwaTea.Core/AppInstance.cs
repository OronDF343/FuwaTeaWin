using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using DryIoc;
using FuwaTea.Extensibility;
using FuwaTea.Extensibility.Config;
using Newtonsoft.Json;
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
        private static readonly string MyDirPath =
            Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        public IReadOnlyList<string> Args { get; }
        public ExtensibilityContainer ExtensibilityContainer { get; private set; }
        internal ExtensionCache ExtensionCache { get; private set; }
        internal AppSettings AppSettings { get; }
        public IPlatformSupport PlatformSupport { get; private set; }
        
        public event UnhandledExceptionEventHandler UnhandledException;

        /// <summary>
        /// Create the instance, and auto-initialize only the bare-minimum requirements for running the application, such as logging.
        /// </summary>
        /// <param name="mainArgs">The supplied command-line arguments.</param>
        public AppInstance(string[] mainArgs)
        {
            // Track unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Log.Fatal("An unhandled exception occured: ", (Exception)args.ExceptionObject);
                OnUnhandledException(args);
            };

            // Create temporary console logger
            // TODO: Get logging switches from command line args
            Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Is(LogEventLevel.Error).CreateLogger();
            
            // Load appsettings.json
            try
            {
                var file = Path.Combine(MyDirPath, AppConstants.SettingsFileName);
                AppSettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(file));
            }
            catch (Exception e)
            {
                // TODO: Is using defaults "silently" really the best option?
                Log.Error(e, "Failed to load AppSettings, defaults will be used!");
                AppSettings = new AppSettings();
                // Don't overwrite the file
            }

            // Save command-line arguments
            Args = new ReadOnlyCollection<string>(mainArgs);

            // Configure the logger
            // TODO: Get logging overrides from command line args
            var logLevel = AppSettings.DefaultLogLevel;
            var logDir = MakeAppDataPath(AppConstants.LogsDirName, false);
            Log.Logger = new LoggerConfiguration().WriteTo.Console().MinimumLevel.Is(logLevel)
                                                  .WriteTo.RollingFileAlternate(logDir, retainedFileCountLimit: 20,
                                                                                fileSizeLimitBytes: 1048576).MinimumLevel.Is(logLevel)
                                                  .CreateLogger();
            // Now we are ready for Init()
            Log.Information("\n*************\nHello, world!\n*************");
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
            Log.Information("Main initialization has started");
            // Create container
            ExtensibilityContainer = new ExtensibilityContainer();
            // Initialize configuration
            InitConfig();
            // Resolve platform support
            InitPlatform();
            // Process command-line arguments
            if (!ProcessClArgs()) return false;
            // 

            return true;
        }

        private void InitConfig()
        {
            Log.Information("Configuration initialization has started");
            // Init config directories first
            var persistentConfigDir = MakeAppDataPath(AppConstants.ConfigDirName);
            Log.Debug("Persistent config dir: " + persistentConfigDir);
            var nonPersistentConfigDir = MakeAppDataPath(AppConstants.ConfigDirName, false);
            Log.Debug("Non-persistent config dir: " + nonPersistentConfigDir);
            ExtensibilityContainer.SetConfigDirs(persistentConfigDir, nonPersistentConfigDir);
            // Add required dynamic pages
            ExtensionCache = ExtensibilityContainer.RegisterConfigPage<ExtensionCache>(new ConfigPageMetadata(nameof(ExtensionCache), false, false));
            // Load required pages
            ExtensibilityContainer.LoadConfigPage(nameof(ExtensionCache));
            Log.Information("Configuration and extension cache have been initialized successfully");
        }

        private void InitPlatform()
        {
            Log.Information("Platform initialization has started");
            Log.Information($"I am running on {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}, {RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture}");
            // Load platform DLLs
            foreach (var f in Directory.EnumerateFiles(MyDirPath,
                                                       Assembly.GetExecutingAssembly().GetName().Name
                                                       + ".Platform.*.dll", SearchOption.TopDirectoryOnly))
            {
                Log.Debug($"Found platform DLL: {f}");
                var ext = ExtensionCache.CreateExtension(f);
                ext.Load();
                Log.Debug($"IsLoaded: {ext.IsLoaded}");
                if (ext.IsLoaded) ExtensibilityContainer.RegisterExtension(ext);
            }

            PlatformSupport = ExtensibilityContainer.OpenScope().Resolve<IPlatformSupport>(IfUnresolved.ReturnDefaultIfNotRegistered);
            if (PlatformSupport == null)
            {
                const string msg = "Platform support not available, or has failed to load!";
                Log.Error(msg);
                throw new PlatformNotSupportedException(msg);
            }
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
        
        /// <summary>
        /// Generates a path to an application data directory.
        /// </summary>
        /// <param name="dirName">The desired name of the directory.</param>
        /// <param name="isPersistent">Whether the directory should be in a persistent location.</param>
        /// <returns></returns>
        public string MakeAppDataPath(string dirName, bool isPersistent = true)
        {
            return AppSettings.IsInstalled
                       ? Path.Combine(Environment.GetFolderPath(isPersistent ? Environment.SpecialFolder.ApplicationData : Environment.SpecialFolder.LocalApplicationData),
                                      AppConstants.ProductName, dirName)
                       : MakeAppDirPath(dirName); // TODO: Is this the best idea? Persistent and non-persistent data will share the same directory in portable mode!
        }

        /// <summary>
        /// Generates a path to the application's home directory.
        /// </summary>
        /// <param name="dirName">The desired name of the directory.</param>
        /// <returns></returns>
        public string MakeAppDirPath(string dirName)
        {
            return Path.Combine(MyDirPath, dirName);
        }
    }
}
