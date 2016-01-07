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
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Threading;
using System.Windows;
using FTWPlayer.Localization;
using FTWPlayer.Properties;
using FTWPlayer.Skins;
using FuwaTea.Lib;
using FuwaTea.Lib.FileAssociations;
using FuwaTea.Playback;
using FuwaTea.Playlist;
using FuwaTea.Wpf.Behaviors;
using FuwaTea.Wpf.Helpers;
using FuwaTea.Wpf.Keyboard;
using GalaSoft.MvvmLight.Threading;
using log4net;
using log4net.Config;
using ModularFramework;
using ModularFramework.Configuration;
using ModularFramework.Extensions;
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
        private List<string> _clArgs;
        private string _prod, _title;
        private bool _isinst;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(App));
        private readonly ErrorCallback _ec = ex => Logger.Warn("AssemblyLoader reported an error:", ex);
        private const string SetupFileAssocArg = "--setup-file-associations";
        private const string CleanupFileAssocArg = "--clean-up-file-associations";
        private const string ConfigureFileAssocArg = "--configure-file-associations";
        private const string ShouldBeAdminArg = "--admin";
        private const string SetLangArg = "--set-lang";

        internal List<IConfigurablePropertyInfo> DynSettings { get; private set; }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup([NotNull] StartupEventArgs e)
        {
            // TODO: create ErrorDialog
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Logger.Fatal("An unhandled exception occured:", (Exception)args.ExceptionObject);
                try
                {
                    MessageBox.Show(MainWindow,
                                    LocalizationProvider.GetLocalizedValue<string>("UnhandledExceptionMessage"),
                                    string.Format(LocalizationProvider.GetLocalizedValue<string>("AppCrash"), _prod), MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to show the unhandled exception dialog to the user!", ex);
                }
            };
            
            // Get ClArgs:
            _clArgs = Environment.GetCommandLineArgs().ToList();

            try
            {
#if DEBUG
                XmlConfigurator.ConfigureAndWatch(new FileInfo(Assembly.GetExecutingAssembly()
                                                                       .GetSpecificPath(false, "logconfig-debug.xml",
                                                                                        false)));
#else
                XmlConfigurator.ConfigureAndWatch(new FileInfo(Assembly.GetExecutingAssembly()
                                                                       .GetSpecificPath(false, "logconfig.xml",
                                                                                        false)));
#endif
            }
            catch (SystemException se) { Logger.Fatal("Failed to open log config file!", se); }

            Logger.Info("Exceptions are tracked, logging is configured, begin loading!");

            // Upgrade settings:
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(UpgradeSettings)))
                UpgradeSettings();

            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(ConfigureLocalization)))
                ConfigureLocalization();

            _isinst = Assembly.GetEntryAssembly().IsInstalledCopy();
            _prod = Assembly.GetEntryAssembly().GetProduct();
            _title = Assembly.GetEntryAssembly().GetAttribute<AssemblyTitleAttribute>().Title;

            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(ProcessAssocClArgs)))
                if (!ProcessAssocClArgs())
                {
                    Shutdown();
                    return;
                }

            Logger.Info("Normal startup will begin");
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(InitMutex)))
                if (!InitMutex())
                {
                    Shutdown();
                    return;
                }

            // Load modules:
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(LoadModules)))
                LoadModules();

            // Dynamic config init:
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(InitDynamicSettings)))
                InitDynamicSettings();

            // Load skins:
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(LoadSkins)))
                LoadSkins();

            // Load key bindings:
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(LoadKeyBindings)))
                LoadKeyBindings();

            // Set priority: 
            try { Process.GetCurrentProcess().PriorityClass = Settings.Default.ProcessPriority; }
            catch (Win32Exception w32)
            {
                Logger.Error("Failed to set the process priority!", w32);
            }

            Logger.Info("Ready to open the main window!");
            // Manually show main window (pervents loading it on shutdown)
            MainWindow = new MainWindow();
            MainWindow.Show();
            WindowPositioner.SetAutoPosition(MainWindow, Settings.Default.AutoWindowPosition);
            base.OnStartup(e);
        }

        /// <summary>
        /// Restart the application and request administrator rights
        /// </summary>
        /// <param name="args">The command line arguments to pass to the new instance of the application</param>
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
            try { process.Start(); }
            catch (Win32Exception w32)
            {
                Logger.Fatal("Failed to start the new process!", w32);
                // We should still exit anyway here
            }
            Shutdown();
        }

        private void UpgradeSettings()
        {
            var ver = Settings.Default.LastVersion;
            var cver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            if (string.Equals(ver, cver, StringComparison.OrdinalIgnoreCase)) return;
            Logger.Info("The settings are from an older version, upgrading now");
            try { Settings.Default.Upgrade(); }
            catch (ConfigurationErrorsException cee)
            {
                Logger.Error("Configuration errors when upgrading settings!", cee);
            }
            Settings.Default.LastVersion = cver;
            Settings.Default.Save();
        }

        private void ConfigureLocalization()
        {
            // Parse ClArgs:
            var li = _clArgs.IndexOf(SetLangArg) + 1;
            if (li > 0 && _clArgs.Count > li)
            {
                Settings.Default.OverrideDefaultLanguage = true;
                Settings.Default.SelectedLanguage = _clArgs[li];
            }

            // Set localization:
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            if (Settings.Default.OverrideDefaultLanguage) UpdateLanguage();
            // Update if changed:
            Settings.Default.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable once RedundantJumpStatement
                if (!Settings.Default.OverrideDefaultLanguage
                     || args.PropertyName != nameof(Settings.Default.SelectedLanguage)
                     && args.PropertyName != nameof(Settings.Default.OverrideDefaultLanguage)) return;
                //UpdateLanguage(); BUG: This causes an exception "Binding cannot be changed after it has been used" in WPFLocalizeExtension
            };
        }

        private void UpdateLanguage()
        {
            LocalizeDictionary.Instance.Culture = string.IsNullOrWhiteSpace(Settings.Default.SelectedLanguage)
                                                      ? CultureInfo.CurrentUICulture
                                                      : CultureInfo.CreateSpecificCulture(Settings.Default.SelectedLanguage);
            Settings.Default.PropertyValues[nameof(Settings.Default.SelectedLanguage)].PropertyValue = LocalizeDictionary.Instance.Culture.IetfLanguageTag;
        }

        private bool ProcessAssocClArgs()
        {
            if (_clArgs.Contains(SetupFileAssocArg) && _isinst)
            {
                Logger.Info("Detected argument: Setup File Associations");
                LoadModules();
                var l = Assembly.GetExecutingAssembly().Location;
                var pm = ModuleFactory.GetElement<IPlaybackManager>();
                var plm = ModuleFactory.GetElement<IPlaylistManager>();
                var supported = pm.GetExtensionsInfo().Union(StringUtils.GetExtensionsInfo(plm.ReadableFileTypes)).ToDictionary(p => p.Key, p => p.Value);
                try
                {
                    RegistryUtils.UpdateFuwaAssociations(supported, l, _prod,
                                                         string.Format(LocalizationProvider.GetLocalizedValue<string>("PlayWithFormatString"), _title),
                                                         string.Format(LocalizationProvider.GetLocalizedValue<string>("AddToPlaylistFormatString"), _title),
                                                         LocalizationProvider.GetLocalizedValue<string>("AppDescription"));
                }
                catch (Exception ex)
                {
                    Logger.Error("Registry error", ex);
                    RegistryError(SetupFileAssocArg);
                }
                return false;
            }
            if (_clArgs.Contains(CleanupFileAssocArg) && _isinst)
            {
                Logger.Info("Detected argument: Clean Up File Associations");
                try
                {
                    RegistryUtils.RemoveFuwaClasses(_prod);
                }
                catch (Exception ex)
                {
                    Logger.Error("Registry error", ex);
                    RegistryError(CleanupFileAssocArg);
                }
                Shutdown();
                return false;
            }
            if (_clArgs.Contains(ConfigureFileAssocArg) && _isinst)
            {
                Logger.Info("Detected argument: Configure File Associations");
                var assocUi = new ApplicationAssociationRegistrationUI();
                try
                {
                    assocUi.LaunchAdvancedAssociationUI(_prod);
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
                return false;
            }
            return true;
        }

        private void RegistryError(string args)
        {
            Logger.Error("Failed to create/open registry subkey!");
            if (_clArgs.Contains(ShouldBeAdminArg)) return;
            Logger.Info("Trying to restart with admin rights");
            RestartAsAdmin(args);
        }

        private bool InitMutex()
        {
            var mutexCreated = false;
            var mutexName = _prod + (_isinst ? "|Installed" : "|Portable");
            try {
                _mutex = new Mutex(true, mutexName, out mutexCreated);
            }
            catch (UnauthorizedAccessException uae) {
                Logger.Error("Failed to gain access to the Mutex!", uae);
            }
            catch (IOException ie) {
                Logger.Error("Win32 error while opening the Mutex!", ie);
            }
            catch (WaitHandleCannotBeOpenedException whcboe) {
                Logger.Error("Failed to create the Mutex!", whcboe);
            }
            Message = (int)NativeMethods.RegisterWindowMessage(mutexName);

            if (mutexCreated) return true;
            if (!_clArgs.Contains("--wait")) // TODO: better handling of _clArgs
            {
                Logger.Info("Another instance is already open - send our arguments to it if needed");
                _mutex = null;
                if (_clArgs.Count < 2) return false;
                SendClArgs();
                return false;
            }
            try
            {
                if (!_mutex.WaitOne(Settings.Default.InstanceCreationTimeout))
                {
                    Logger
                              .Fatal("Failed to create an instance: The operation has timed out.");
                    MessageBox.Show("Failed to create an instance: The operation has timed out.",
                                    "Single Instance Application", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (AbandonedMutexException ame)
            {
                Logger.Error("Mutex was abandoned!", ame);
            }
            if (Mutex.TryOpenExisting(mutexName, MutexRights.FullControl, out _mutex)) return true;
            _mutex = new Mutex(true, mutexName, out mutexCreated);
            if (mutexCreated) return true;
            Logger
                      .Fatal("Failed to create an instance: Cannot open or create the Mutex!");
            MessageBox.Show("Failed to create an instance: Cannot open or create the Mutex!",
                            "Single Instance Application", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        private void SendClArgs()
        {
            var p = Assembly.GetExecutingAssembly().GetSpecificPath(true, @"ClArgs.txt", false);
            try
            {
                File.WriteAllLines(p, _clArgs);
                // TODO: find better way of passing args
            }
            catch (DirectoryNotFoundException dnfe)
            {
                Logger.Error("Invalid path for saving ClArgs file", dnfe);
            }
            catch (IOException ie)
            {
                Logger.Error("I/O error while saving ClArgs file", ie);
            }
            catch (SecurityException se)
            {
                Logger.Error("Missing permissions to save ClArgs file", se);
            }
            catch (UnauthorizedAccessException uae)
            {
                Logger.Error("Access denied for saving ClArgs file", uae);
            }
            NativeMethods.SendMessage(NativeMethods.HWND_BROADCAST, Message, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Load all the modular components of the application
        /// </summary>
        /// <param name="loadExtensions">Specifies whether to load extensions as well</param>
        public void LoadModules(bool loadExtensions = true)
        {
            Func<string, bool> sel =
                f =>
                f.ToLowerInvariant().EndsWith(".dll")
                || (!f.ToLowerInvariant().EndsWith("unins000.exe") && f.ToLowerInvariant().EndsWith(".exe"));
            var ef = Assembly.GetEntryAssembly().GetExeFolder();
            if (ef == null)
            {
                Logger.Fatal("Failed to load modular components - I don't know where I am!");
                return; // TODO: return false and check
            }
            ModuleFactory.LoadFolder(ef, sel, _ec, true);
            if (!loadExtensions) return;
            string extDir;
            try { extDir = Assembly.GetEntryAssembly().GetSpecificPath(false, "extensions", true); }
            catch (IOException ie)
            {
                Logger.Error("I/O exception getting extensions directory!", ie);
                return;
            }
            catch (UnauthorizedAccessException uae)
            {
                Logger.Error("Access denied to extensions directory!", uae);
                return;
            }
            var extWhitelist = Settings.Default.ExtensionWhitelist ?? new ExtensionAttributeCollection();
            ExtensionUtils.ExtensionLoadCallback =
                exa =>
                {
                    if (extWhitelist.Items.Contains(exa)) return true;
                    var res = MessageBox.Show($"Are you sure you want to load the following extension?\n\n{exa.Name} version {exa.Version}\nby {exa.Author}\n{exa.Homepage}\n\nOnly load extensions that you trust!",
                                              "FTW Extension Loader", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes;
                    if (res) extWhitelist.Items.Add(exa);
                    return res;
                };
            ExtensionUtils.LoadExtensions(extDir, sel, _ec);
            IEnumerable<string> dirs;
            try { dirs = Directory.EnumerateDirectories(extDir); }
            catch (UnauthorizedAccessException uae)
            {
                Logger.Error("Access denied to the extensions directory!", uae);
                return;
            }
            catch (SecurityException se)
            {
                Logger.Error("Missing permissions to the extensions directory!", se);
                return;
            }
            catch (IOException ie)
            {
                Logger.Error("The extensions directory is a file?!", ie);
                return;
            }
            foreach (var dir in dirs)
                ExtensionUtils.LoadExtensions(dir, sel, _ec);
            Settings.Default.ExtensionWhitelist = extWhitelist;
        }

        private void InitDynamicSettings()
        {
            var provider = Settings.Default.Properties["LastVersion"]?.Provider;
            var dict = new SettingsAttributeDictionary
            {
                {
                    typeof(SettingsManageabilityAttribute),
                    new SettingsManageabilityAttribute(SettingsManageability.Roaming)
                },
                {
                    typeof(UserScopedSettingAttribute),
                    new UserScopedSettingAttribute()
                }
            };
            DynSettings = ModuleFactory.GetAllConfigurableProperties(ex => Logger.Error("Error getting configurable property:", ex)).ToList();
            foreach (var info in DynSettings)
            {
                try
                {
                    Settings.Default.Properties.Add(new SettingsProperty(info.Name, info.PropertyInfo.PropertyType,
                                                                         provider, false, info.DefaultValue,
                                                                         SettingsSerializeAs.String, dict, true, true));
                    info.Value = Settings.Default[info.Name];
                }
                catch (ConfigurationErrorsException cee)
                {
                    Logger.Error("The settings failed to instantiate!", cee);
                }
                catch (SettingsPropertyNotFoundException spnfe)
                {
                    Logger.Fatal("Unexpected settings error, Name=" + info.Name, spnfe);
                }
                catch (SettingsPropertyWrongTypeException spwte)
                {
                    Logger.Error("Invalid default value for setting, Name=" + info.Name + " DefaultValue=" + info.DefaultValue, spwte);
                }
                catch (SettingsPropertyIsReadOnlyException spiroe)
                {
                    Logger.Fatal("Unexpected settings error, Name=" + info.Name, spiroe);
                }
            }
            Logger.Debug("Dynamic properties: " + DynSettings.Count);
            // Handle changes
            Settings.Default.PropertyChanged += (sender, args) =>
            {
                var p = DynSettings.FirstOrDefault(i => i.Name == args.PropertyName);
                if (p == null) return;
                p.Value = Settings.Default[p.Name];
            };
        }

        private void LoadSkins()
        {
            var sm = ModuleFactory.GetElement<ISkinManager>();
            if (string.IsNullOrWhiteSpace(Settings.Default.Skin)) SkinLoadingBehavior.UpdateSkin(sm.LoadFallbackSkin().SkinParts);
            else
            {
                SkinPackage skin;
                try { skin = sm.LoadSkin(Settings.Default.Skin); }
                catch (InvalidOperationException ioe)
                {
                    Logger.Error("Detected cyclic dependency between skins! Loading fallback skin instead", ioe);
                    skin = sm.LoadFallbackSkin();
                }
                catch (SkinLoadException sle)
                {
                    Logger.Error("Failed to load the skin! Loading fallback skin instead", sle);
                    skin = sm.LoadFallbackSkin();
                }
                SkinLoadingBehavior.UpdateSkin(skin.SkinParts);
                Logger.Info($"Successfully loaded skin: Name=\"{skin.GetIdentifier()?.Name ?? "null"}\" Path=\"{skin.Path}\"");
            }
        }

        private void LoadKeyBindings()
        {
            Logger.Info("Loading key bindings engine...");
            KeyBindingsManager.Instance.Listener = new KeyboardListener();
            KeyBindingsManager.Instance.Listener.IsEnabled = Settings.Default.EnableKeyboardHook;
            Settings.Default.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(Settings.Default.EnableKeyboardHook))
                    KeyBindingsManager.Instance.Listener.IsEnabled = Settings.Default.EnableKeyboardHook;
            };
            if (Settings.Default.KeyBindings != null)
                foreach (var keyBinding in Settings.Default.KeyBindings.Items)
                    KeyBindingsManager.Instance.KeyBindings.Add(keyBinding);
            KeyBindingsManager.Instance.KeyBindings.CollectionChanged +=
                (s, args) =>
                Settings.Default.KeyBindings = new KeyBindingCollection(KeyBindingsManager.Instance.KeyBindings);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.</param>
        protected override void OnExit([NotNull] ExitEventArgs e)
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
