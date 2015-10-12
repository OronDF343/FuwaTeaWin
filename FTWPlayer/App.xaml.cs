#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;
using FTWPlayer.Properties;
using FTWPlayer.Skins;
using FuwaTea.Lib;
using FuwaTea.Logic.Playlist;
using FuwaTea.Presentation.Playback;
using GalaSoft.MvvmLight.Threading;
using log4net;
using log4net.Config;
using Microsoft.Win32;
using ModularFramework;

namespace FTWPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : IDisposable
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        private Mutex _mutex;
        internal int Message;
        readonly ErrorCallback _ec = ex => LogManager.GetLogger(typeof(App)).Warn("AssemblyLoader reported an error:", ex);
        private const string SetupFileAssocArg = "--setup-file-associations";
        private const string CleanupFileAssocArg = "--clean-up-file-associations";
        private const string ConfigureFileAssocArg = "--configure-file-associations";
        private const string ShouldBeAdminArg = "--admin";

        protected override void OnStartup(StartupEventArgs e)
        {
            // TODO: create ErrorDialog
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => LogManager.GetLogger(GetType()).Fatal("An unhandled exception occured:", (Exception)args.ExceptionObject);

#if DEBUG
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Assembly.GetExecutingAssembly()
                                                                   .GetSpecificPath(false, "logconfig-debug.xml", false)));
#else
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Assembly.GetExecutingAssembly().GetExeFolder(),
                                                                        "logconfig.xml")));
#endif

            LogManager.GetLogger(GetType()).Info("Exceptions are tracked, logging is configured, begin loading!");

            // Get ClArgs:
            var clArgs = Environment.GetCommandLineArgs().ToList();
            var isinst = Assembly.GetEntryAssembly().IsInstalledCopy();
            var prod = Assembly.GetEntryAssembly().GetProduct();
            if (clArgs.Contains(SetupFileAssocArg) && isinst)
            {
                LogManager.GetLogger(GetType()).Info("Detected argument: Setup File Associations");
                LoadLayers();
                var pm = ModuleFactory.GetElement<IPlaybackManager>();
                var plm = ModuleFactory.GetElement<IPlaylistManager>();
                var supported = pm.SupportedFileTypes.Union(plm.ReadableFileTypes);
                var key = Registry.LocalMachine.CreateSubKey($@"Software\Clients\Media\{prod}\Capabilities\FileAssociations");
                if (key == null)
                {
                    LogManager.GetLogger(GetType()).Error("Failed to create/open registry subkey!");
                    if (!clArgs.Contains(ShouldBeAdminArg))
                    {
                        LogManager.GetLogger(GetType()).Info("Trying to restart with admin rights");
                        RestartAsAdmin(SetupFileAssocArg);
                    }
                    Shutdown();
                    return;
                }
                var exts = key.GetValueNames();
                var f = exts.Where(s => !supported.Contains(s));
                var t = supported.Where(s => !exts.Contains(s));
                foreach (var s in f) key.DeleteValue(s);
                foreach (var s in t) key.SetValue(s, prod + ".AudioFileGeneric", RegistryValueKind.String);
                Shutdown();
                return;
            }
            if (clArgs.Contains(CleanupFileAssocArg) && isinst)
            {
                LogManager.GetLogger(GetType()).Info("Detected argument: Clean Up File Associations");
                var key = Registry.LocalMachine.CreateSubKey($@"Software\Clients\Media\{prod}\Capabilities\FileAssociations");
                if (key != null) foreach (var s in key.GetValueNames()) key.DeleteValue(s);
                else
                {
                    LogManager.GetLogger(GetType()).Error("Failed to create/open registry subkey!");
                    if (!clArgs.Contains(ShouldBeAdminArg))
                    {
                        LogManager.GetLogger(GetType()).Info("Trying to restart with admin rights");
                        RestartAsAdmin(CleanupFileAssocArg);
                    }
                }
                Shutdown();
                return;
            }
            if (clArgs.Contains(ConfigureFileAssocArg) && isinst)
            {
                LogManager.GetLogger(GetType()).Info("Detected argument: Configure File Associations");
                var assocUi = new ApplicationAssociationRegistrationUI();
                try
                {
                    assocUi.LaunchAdvancedAssociationUI(prod);
                }
                catch
                {
                    MessageBox.Show("Could not display the file association manager!\nPlease right-click a file -> 'Open with' -> 'Choose default program' and choose FTW Player.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                finally
                {
                    Marshal.ReleaseComObject(assocUi);
                }
                Shutdown();
                return;
            }

            LogManager.GetLogger(GetType()).Info("Normal startup will begin");
            bool mutexCreated;
            var mutexName = prod + (isinst ? "|Installed" : "|Portable");
            _mutex = new Mutex(true, mutexName, out mutexCreated);
            Message = (int)NativeMethods.RegisterWindowMessage(mutexName);

            if (!mutexCreated)
            {
                if (!clArgs.Contains("--wait")) // TODO: better handling of clArgs
                {
                    LogManager.GetLogger(GetType()).Info("Another instance is already open - send our arguments to it");
                    _mutex = null;
                    if (clArgs.Count > 1)
                    {
                        File.WriteAllLines(Assembly.GetExecutingAssembly().GetSpecificPath(true, @"ClArgs.txt", false), clArgs); // TODO: find better way of passing args
                        NativeMethods.SendMessage(NativeMethods.HWND_BROADCAST, Message, IntPtr.Zero, IntPtr.Zero);
                    }
                    Shutdown();
                    return;
                }
                if (!_mutex.WaitOne(Settings.Default.InstanceCreationTimeout))
                {
                    LogManager.GetLogger(GetType()).Fatal("Failed to create an instance: The operation has timed out.");
                    MessageBox.Show("Failed to create an instance: The operation has timed out.", "Single Instance Application", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }
                if (!Mutex.TryOpenExisting(mutexName, MutexRights.FullControl, out _mutex))
                {
                    _mutex = new Mutex(true, mutexName, out mutexCreated);
                    if (!mutexCreated)
                    {
                        LogManager.GetLogger(GetType()).Fatal("Failed to create an instance: Cannot open or create the Mutex!");
                        MessageBox.Show("Failed to create an instance: Cannot open or create the Mutex!", "Single Instance Application", MessageBoxButton.OK, MessageBoxImage.Error);
                        Shutdown();
                        return;
                    }
                }
            }

            // Upgrade settings:
            var ver = Settings.Default.LastVersion;
            var cver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (!string.Equals(ver, cver, StringComparison.OrdinalIgnoreCase))
            {
                LogManager.GetLogger(GetType()).Info("The settings are from an older version, upgrading now");
                Settings.Default.Upgrade();
                Settings.Default.LastVersion = cver;
                Settings.Default.Save();
            }

            // Load layers:
            LoadLayers();
            // Load skins:
            try
            {
                foreach(var rd in ModuleFactory.GetElement<ISkinManager>().LoadSkinChain(Settings.Default.SkinChain.Cast<string>()))
                    Resources.MergedDictionaries.Add(rd);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(GetType()).Error("Failed to load skin chain! Loading default", ex);
                foreach (var rd in ModuleFactory.GetElement<ISkinManager>().LoadSkinChain(((StringCollection)new XmlSerializer(typeof(StringCollection)).Deserialize(new StringReader((string)Settings.Default.Properties["SkinChain"]?.DefaultValue))).Cast<string>()))
                    Resources.MergedDictionaries.Add(rd);
            }

            // Set priority: 
            Process.GetCurrentProcess().PriorityClass = Settings.Default.ProcessPriority;
            
            LogManager.GetLogger(GetType()).Info("Ready to open the main window!");
            // Manually show main window (pervents loading it on shutdown)
            MainWindow = new MainWindow();
            MainWindow.Show();
            base.OnStartup(e);
        }

        public void RestartAsAdmin(string args)
        {
            var info = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, args + " " + ShouldBeAdminArg)
            {
                Verb = "runas",
                UseShellExecute = true
                // indicates to elevate privileges
            };
            var process = new Process
            {
                EnableRaisingEvents = true, // enable WaitForExit()
                StartInfo = info
            };
            process.Start();
            Shutdown();
        }

        public void LoadLayers(bool loadExtensions = true)
        {
            Func<string, bool> sel =
                f =>
                f.ToLowerInvariant().EndsWith(".dll")
                || (!f.ToLowerInvariant().EndsWith("unins000.exe") && f.ToLowerInvariant().EndsWith(".exe"));
            ModuleFactory.LoadFolder(Assembly.GetEntryAssembly().GetExeFolder(), sel, _ec, true);
            if (!loadExtensions) return;
            var extDir = Assembly.GetEntryAssembly().GetSpecificPath(false, "extensions", true);
            ModuleFactory.LoadFolder(extDir, sel, _ec, true);
            foreach (var dir in Directory.EnumerateDirectories(extDir))
                ModuleFactory.LoadFolder(dir, sel, _ec, true);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            Dispose();
            base.OnExit(e);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || (_mutex == null)) return;
            _mutex.ReleaseMutex();
            _mutex.Close();
            _mutex = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
