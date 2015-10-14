﻿#region License
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;
using FTWPlayer.Localization;
using FTWPlayer.Properties;
using FTWPlayer.Skins;
using FuwaTea.Lib;
using FuwaTea.Playback;
using FuwaTea.Playlist;
using GalaSoft.MvvmLight.Threading;
using log4net;
using log4net.Config;
using Microsoft.Win32;
using ModularFramework;
using WPFLocalizeExtension.Engine;

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
        private const string SetLangArg = "--set-lang";

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

            // Get ClArgs:
            var clArgs = Environment.GetCommandLineArgs().ToList();
            var li = clArgs.IndexOf(SetLangArg) + 1;
            if (li > 0 && clArgs.Count > li)
            {
                Settings.Default.SelectedLanguage = clArgs[li];
            }

            // Set localization
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            LocalizeDictionary.Instance.Culture = string.IsNullOrWhiteSpace(Settings.Default.SelectedLanguage) ? CultureInfo.CurrentUICulture : CultureInfo.CreateSpecificCulture(Settings.Default.SelectedLanguage);
            Settings.Default.SelectedLanguage = LocalizeDictionary.Instance.Culture.IetfLanguageTag;

            var isinst = Assembly.GetEntryAssembly().IsInstalledCopy();
            var prod = Assembly.GetEntryAssembly().GetProduct();
            var title = Assembly.GetEntryAssembly().GetAttribute<AssemblyTitleAttribute>().Title;
            if (clArgs.Contains(SetupFileAssocArg) && isinst)
            {
                LogManager.GetLogger(GetType()).Info("Detected argument: Setup File Associations");
                LoadModules();
                var l = Assembly.GetExecutingAssembly().Location;
                var pm = ModuleFactory.GetElement<IPlaybackManager>();
                var plm = ModuleFactory.GetElement<IPlaylistManager>();
                var supported = pm.GetExtensionsInfo().Union(StringUtils.GetExtensionsInfo(plm.ReadableFileTypes)).ToDictionary(p => p.Key, p => p.Value);
                #region Registry edits

                try
                {
                    var cls = Registry.LocalMachine.CreateSubKey(@"Software\Classes");
                    // Set directory actions:
                    var shell = cls.CreateSubKey(@"Directory\shell");
                    // Play option
                    var play = shell.CreateSubKey($"{prod}.Play");
                    play.SetValue("", string.Format(LocalizationProvider.GetLocalizedValue<string>("PlayWithFormatString"), title));
                    var command1 = play.CreateSubKey("command");
                    command1.SetValue("", $"\"{l}\" \"%1\"");
                    // Add option
                    var add = shell.CreateSubKey($"{prod}.AddToPlaylist");
                    add.SetValue("", string.Format(LocalizationProvider.GetLocalizedValue<string>("AddToPlaylistFormatString"), title));
                    var command2 = add.CreateSubKey("command");
                    command2.SetValue("", $"\"{l}\" \"%1\" --add");
                    // Get classes:
                    var classes = cls.GetSubKeyNames();
                    var installedClasses = classes.Where(c => c.StartsWith(prod + ".")).ToList();
                    var redundantClasses = installedClasses.Where(c => !supported.ContainsKey(c.Substring(c.IndexOf('.') + 1)));
                    var missingClasses = supported.Where(x => !installedClasses.Contains(prod + "." + x.Key));
                    // Delete redundant classes:
                    foreach (var c in redundantClasses) cls.DeleteSubKeyTree(c);
                    // Add missing classes:
                    foreach (var x in missingClasses)
                    {
                        var k = cls.CreateSubKey(prod + "." + x.Key);
                        // if (k == null) { RegistryError(); return; }
                        k.SetValue("", x.Value);
                        var di = k.CreateSubKey("DefaultIcon");
                        di.SetValue("", $"\"{l}\",0");
                        shell = k.CreateSubKey("shell");
                        shell.SetValue("", "Play");
                        // Play option
                        play = shell.CreateSubKey("Play");
                        play.SetValue("", string.Format(LocalizationProvider.GetLocalizedValue<string>("PlayWithFormatString"), title));
                        command1 = play.CreateSubKey("command");
                        command1.SetValue("", $"\"{l}\" \"%1\"");
                        // Add option
                        add = shell.CreateSubKey("AddToPlaylist");
                        add.SetValue("", string.Format(LocalizationProvider.GetLocalizedValue<string>("AddToPlaylistFormatString"), title));
                        command2 = add.CreateSubKey("command");
                        command2.SetValue("", $"\"{l}\" \"%1\" --add");
                    }
                    // Set app description:
                    var cap = Registry.LocalMachine.CreateSubKey($@"Software\Clients\Media\{prod}\Capabilities");
                    cap.SetValue("ApplicationDescription", LocalizationProvider.GetLocalizedValue<string>("AppDescription"));
                    // Get associations:
                    var assoc = cap.CreateSubKey("FileAssociations");
                    var exts = assoc.GetValueNames();
                    var redundantAssociations = exts.Where(s => !supported.ContainsKey(s.Substring(1)));
                    var missingAssociations = supported.Keys.Where(s => !exts.Contains("." + s));
                    // Delete redundant values in associations
                    foreach (var s in redundantAssociations) assoc.DeleteValue(s);
                    // Add missing associations
                    foreach (var s in missingAssociations) assoc.SetValue("." + s, prod + "." + s, RegistryValueKind.String);
                }
                catch (Exception)
                {
                    RegistryError(SetupFileAssocArg);
                }
                #endregion
                Shutdown();
                return;
            }
            if (clArgs.Contains(CleanupFileAssocArg) && isinst)
            {
                LogManager.GetLogger(GetType()).Info("Detected argument: Clean Up File Associations");
                #region Registry edits

                try
                {
                    var cls = Registry.LocalMachine.CreateSubKey(@"Software\Classes");
                    var shell = cls.CreateSubKey(@"Directory\shell");
                    shell.DeleteSubKeyTree($"{prod}.Play");
                    shell.DeleteSubKeyTree($"{prod}.AddToPlaylist");
                    foreach (var s in cls.GetSubKeyNames().Where(c => c.StartsWith(prod + "."))) cls.DeleteSubKeyTree(s);
                    // Not needed since uninstaller takes care of this
                    //var key = Registry.LocalMachine.CreateSubKey($@"Software\Clients\Media\{prod}\Capabilities\FileAssociations");
                    //foreach (var s in key.GetValueNames()) key.DeleteValue(s);
                }
                catch (Exception)
                {
                    RegistryError(CleanupFileAssocArg);
                }
                #endregion
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

            // Load modules:
            LoadModules();
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

        private void RegistryError(string args)
        {
            LogManager.GetLogger(GetType()).Error("Failed to create/open registry subkey!");
            if (!Environment.GetCommandLineArgs().Contains(ShouldBeAdminArg))
            {
                LogManager.GetLogger(GetType()).Info("Trying to restart with admin rights");
                RestartAsAdmin(args);
            }
        }

        public void LoadModules(bool loadExtensions = true)
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
