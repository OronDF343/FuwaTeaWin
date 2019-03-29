using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using DryIoc;
using Newtonsoft.Json;
using Sage.Extensibility;
using Sage.Extensibility.Config;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.RollingFileAlternate;

namespace Sage.Core
{
    /// <summary>
    /// The application instance context.
    /// </summary>
    public sealed class AppInstance : IDisposable
    {
        private static readonly string MyDirPath =
            Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        private Mutex _mutex;
        private string _mutexName;

        public IReadOnlyList<string> Args { get; }
        public ExtensibilityContainer ExtensibilityContainer { get; private set; }
        internal ExtensionCache ExtensionCache { get; private set; }
        internal AppSettings AppSettings { get; }
        public IPlatformSupport PlatformSupport { get; private set; }

        public NamedPipeServerStream IpcServer { get; private set; }
        
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
            // Initialize the mutex (single-instance application)
            // If we successfully took ownership of the mutex, initialize the IPC server (named pipe)
            if (InitMutex()) InitIpcServer();
            else return false;
            // Load core DLLs (if not loaded yet)
            LoadCore();
            // Load extensions
            LoadExtensions();
            // Load remaining configs
            LoadConfigs();
            
            Log.Information("Main initialization has finished all tasks");
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
            EnumerateDlls(MyDirPath, Assembly.GetExecutingAssembly().GetName().Name
                                     + ".Platform.*.dll", SearchOption.TopDirectoryOnly);

            PlatformSupport = ExtensibilityContainer.OpenScope().Resolve<IPlatformSupport>(IfUnresolved.ReturnDefaultIfNotRegistered);
            if (PlatformSupport == null)
            {
                throw new PlatformNotSupportedException("Platform support not available, or has failed to load!");
            }
        }

        private bool ProcessClArgs()
        {
            // TODO: Implement
            if (Args.Contains(AppConstants.Arguments.UpdateFileAssociations))
                return false;
            if (Args.Contains(AppConstants.Arguments.DeleteFileAssociations))
                return false;
            if (Args.Contains(AppConstants.Arguments.FileAssociationsUi))
                return false;
            return true;
        }

        private bool InitMutex()
        {
            var mutexCreated = false;
            _mutexName = AppConstants.ProductName + "|" + (AppSettings.IsInstalled ? "Installed" : "Portable");
            try {
                _mutex = new Mutex(true, _mutexName, out mutexCreated);
            }
            catch (UnauthorizedAccessException uae) {
                Log.Error("Failed to gain access to the Mutex!", uae);
            }
            catch (IOException ie) {
                Log.Error("Win32 error while opening the Mutex!", ie);
            }
            catch (WaitHandleCannotBeOpenedException whe) {
                Log.Error("Failed to create the Mutex!", whe);
            }
            //Message = (int)NativeMethods.RegisterWindowMessage(mutexName);

            if (mutexCreated) return true;
            if (!Args.Contains(AppConstants.Arguments.Wait))
            {
                Log.Information("Another instance is already open - send our arguments to it if needed");
                _mutex = null;
                if (Args.Count > 1) SendArgsIpc();
                return false;
            }
            try
            {
                if (!_mutex.WaitOne(AppSettings.InstanceCreationTimeout))
                    throw new TimeoutException("Failed to create an instance: The operation has timed out.");
            }
            catch (AbandonedMutexException ame)
            {
                Log.Error("Mutex was abandoned!", ame);
            }
            if (Mutex.TryOpenExisting(_mutexName, out _mutex)) return true;
            _mutex = new Mutex(true, _mutexName, out mutexCreated);
            if (mutexCreated) return true;
            throw new UnauthorizedAccessException("Failed to create an instance: Cannot open or create the Mutex!");
        }

        private void SendArgsIpc()
        {
            var ipcClient = new NamedPipeClientStream(_mutexName, _mutexName, PipeDirection.Out);
            // TODO: This is temporary
            var data = Encoding.UTF8.GetBytes(string.Join("\n", Args));
            ipcClient.Write(data, 0, data.Length);
            ipcClient.Dispose();
        }

        private void InitIpcServer()
        {
            IpcServer = new NamedPipeServerStream(_mutexName, PipeDirection.In, 1, PipeTransmissionMode.Message);
        }

        private void LoadCore()
        {
            Log.Information("Loading core DLLs");
            EnumerateDlls(MyDirPath, "*.dll", SearchOption.TopDirectoryOnly);
        }

        private void LoadExtensions()
        {
            Log.Information("Loading extensions");
            EnumerateDlls(MakeAppDirPath(AppConstants.ExtensionsDirName), "*.dll", SearchOption.AllDirectories);
        }

        private void LoadConfigs()
        {
            Log.Information("Loading all configuration pages");
            ExtensibilityContainer.LoadAllConfigPages();
        }

        /// <summary>
        /// Generates a path to an application data directory.
        /// </summary>
        /// <param name="dirName">The desired name of the directory.</param>
        /// <param name="isPersistent">Whether the directory should be in a persistent location.</param>
        /// <returns></returns>
        public string MakeAppDataPath(string dirName, bool isPersistent = true)
        {
            var r = AppSettings.IsInstalled
                       ? Path.Combine(Environment.GetFolderPath(isPersistent ? Environment.SpecialFolder.ApplicationData : Environment.SpecialFolder.LocalApplicationData),
                                      AppConstants.ProductName, dirName)
                       : MakeAppDirPath(dirName); // TODO: Is this the best idea? Persistent and non-persistent data will share the same directory in portable mode!
            Directory.CreateDirectory(r);
            return r;
        }

        /// <summary>
        /// Generates a path to the application's home directory.
        /// </summary>
        /// <param name="dirName">The desired name of the directory.</param>
        /// <returns></returns>
        public string MakeAppDirPath(string dirName)
        {
            var r = Path.Combine(MyDirPath, dirName);
            Directory.CreateDirectory(r);
            return r;
        }

        private void EnumerateDlls(string baseDir, string searchPattern, SearchOption searchOption)
        {
            // Load core DLLs
            foreach (var f in Directory.EnumerateFiles(baseDir, searchPattern, searchOption))
            {
                Log.Debug($"Found DLL: {f}");
                // Find in cache or create new
                var ext = ExtensionCache.CreateExtension(f);
                // Don't load extension if already loaded
                if (!string.IsNullOrEmpty(ext.Key) && ExtensibilityContainer.LoadedExtensions.ContainsKey(ext.Key))
                {
                    Log.Debug($"Already loaded key, skipping: {ext.Key}");
                    continue;
                }
                ext.Load();
                Log.Debug($"IsLoaded: {ext.IsLoaded}");
                if (ext.IsLoaded) ExtensibilityContainer.RegisterExtension(ext);
            }
        }

        public void Dispose()
        {
            IpcServer?.Dispose();
            _mutex?.Dispose();
            ExtensibilityContainer?.Dispose();
        }
    }
}
