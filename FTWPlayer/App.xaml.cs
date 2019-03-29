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
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Interop;
using DryIoc;
using FTWPlayer.Localization;
using FTWPlayer.Skins;
using FuwaTea.Lib;
using FuwaTea.Lib.FileAssociations;
using FuwaTea.Wpf.Behaviors;
using FuwaTea.Wpf.Helpers;
using FuwaTea.Wpf.Keyboard;
using GalaSoft.MvvmLight.Threading;
using JetBrains.Annotations;
using log4net;
using log4net.Config;
using Sage.Audio.Decoders;
using Sage.Extensibility;
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
        private const string SetupFileAssocArg = "--setup-file-associations";
        private const string CleanupFileAssocArg = "--clean-up-file-associations";
        private const string ConfigureFileAssocArg = "--configure-file-associations";
        private const string ShouldBeAdminArg = "--admin";
        private const string SetLangArg = "--set-lang";

        private IResolverContext AppScope { get; set; }

        private UISettings Settings { get; set; }
        
        public ExtensibilityContainer MainContainer { get; private set; }

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
            
            // Load settings:
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(LoadSettings)))
                LoadSettings();

            // Upgrade settings:
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(UpgradeSettings)))
                UpgradeSettings();

            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(ConfigureLocalization)))
                ConfigureLocalization();

            // Load skins:
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(LoadSkins)))
                LoadSkins();

            // Load key bindings:
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(LoadKeyBindings)))
                LoadKeyBindings();

            // Set priority: 
            try { Process.GetCurrentProcess().PriorityClass = Settings.ProcessPriority; }
            catch (Win32Exception w32)
            {
                Logger.Error("Failed to set the process priority!", w32);
            }

            Logger.Info("Ready to open the main window!");
            // Manually show main window (pervents loading it on shutdown)
            MainWindow = new MainWindow();
            MainWindow.SourceInitialized += MainWindow_OnSourceInitialized;
            MainWindow.Show();
            WindowPositioner.SetAutoPosition(MainWindow, Settings.AutoWindowPosition);
            base.OnStartup(e);
        }
        
        #region Single Instance Application

        private void MainWindow_OnSourceInitialized(object sender, EventArgs e)
        {
            // Single Instance Application hook
            var source = (HwndSource)PresentationSource.FromVisual(MainWindow);
            source?.AddHook(HwndSourceHook); // should never be null
            // Needs to happen here:
            FlowDirectionUpdater.UpdateFlowDirection(LocalizeDictionary.Instance.Culture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight);
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == Message)
            {
                MainWindow.Show();
                var clArgs = File.ReadAllLines(Assembly.GetExecutingAssembly().GetSpecificPath(true, @"ClArgs.txt", false)).ToList(); // TODO: find better way of passing args
                // TODO: better handling of clArgs
                // TODO: finish this
                MiscUtils.ParseClArgs(clArgs);
            }
            return IntPtr.Zero;
        }

        #endregion

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
            // TODO: Is this necessary?
            //var ver = Settings.LastVersion;
            //var cver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //if (string.Equals(ver, cver, StringComparison.OrdinalIgnoreCase)) return;
            //Logger.Info("The settings are from an older version, upgrading now");
            //try { Settings.Upgrade(); }
            //catch (ConfigurationErrorsException cee)
            //{
            //    Logger.Error("Configuration errors when upgrading settings!", cee);
            //}
            //Settings.LastVersion = cver;
            //Settings.Save();
        }

        private void ConfigureLocalization()
        {
            // Parse ClArgs:
            var li = _clArgs.IndexOf(SetLangArg) + 1;
            if (li > 0 && _clArgs.Count > li)
            {
                Settings.OverrideDefaultLanguage = true;
                Settings.SelectedLanguage = _clArgs[li];
            }

            // Set localization:
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            if (Settings.OverrideDefaultLanguage) UpdateLanguage();
            // Update if changed:
            Settings.PropertyChanged += (sender, args) =>
            {
                // ReSharper disable once RedundantJumpStatement
                if (!Settings.OverrideDefaultLanguage
                     || args.PropertyName != nameof(Settings.SelectedLanguage)
                     && args.PropertyName != nameof(Settings.OverrideDefaultLanguage)) return;
                //UpdateLanguage(); BUG: This causes an exception "Binding cannot be changed after it has been used" in WPFLocalizeExtension
            };
        }

        private void UpdateLanguage()
        {
            LocalizeDictionary.Instance.Culture = string.IsNullOrWhiteSpace(Settings.SelectedLanguage)
                                                      ? CultureInfo.CurrentUICulture
                                                      : CultureInfo.CreateSpecificCulture(Settings.SelectedLanguage);
            Settings.SelectedLanguage = LocalizeDictionary.Instance.Culture.IetfLanguageTag;
        }

        private bool ProcessAssocClArgs()
        {
            if (_clArgs.Contains(SetupFileAssocArg) && _isinst)
            {
                Logger.Info("Detected argument: Setup File Associations");
                LoadModules();
                var l = Assembly.GetExecutingAssembly().Location;
                var dm = AppScope.Resolve<IDecoderManager>();
                var stem = AppScope.Resolve<ISubTrackEnumerationManager>();
                //var supported = pm.GetExtensionsInfo().Union(StringUtils.GetExtensionsInfo(plm.ReadableFileTypes)).ToDictionary(p => p.Key, p => p.Value);
                // TODO: Localize descriptions
                var supported = dm.SupportedFormats.ToDictionary(s => s, s => s.ToUpperInvariant() + " audio file");
                foreach (var format in stem.SupportedFormats)
                {
                    supported.Add(format, format.ToUpperInvariant() + " container file");
                }
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
                if (!_mutex.WaitOne(Settings.InstanceCreationTimeout))
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
            bool Sel(string f) => f.ToLowerInvariant().EndsWith(".dll") || (!f.ToLowerInvariant().EndsWith("unins000.exe") && f.ToLowerInvariant().EndsWith(".exe"));
            var ef = Assembly.GetEntryAssembly().GetExeFolder();
            if (ef == null)
            {
                Logger.Fatal("Failed to load modular components - I don't know where I am!");
                return; // TODO: return false and check
            }

            MainContainer = new ExtensibilityContainer();

            // TODO: Refactor this
            foreach (var file in Directory.EnumerateFiles(ef))
            {
                if (!Sel(file)) continue;
                try
                {
                    var ext = MainContainer.LoadExtension(AssemblyName.GetAssemblyName(file));
                    Logger.Info($"Loaded: {ext}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load component {file}", ex);
                }
            }

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

            // TODO: Extension whitelist
            //var extWhitelist = Settings.ExtensionWhitelist ?? new ExtensionAttributeCollection();

            foreach (var file in Directory.EnumerateFiles(extDir))
            {
                if (!Sel(file)) continue;
                try
                {
                    var ext = MainContainer.LoadExtension(AssemblyName.GetAssemblyName(file));
                    if (ext != null) Logger.Info($"Loaded: {ext}");
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to load extension {file}", ex);
                    throw ex;
                }
                //if (extWhitelist.Items.Contains(exa)) return true;
                //var res = MessageBox.Show($"Are you sure you want to load the following extension?\n\n{exa.Name} version {exa.Version}\nby {exa.Author}\n{exa.Homepage}\n\nOnly load extensions that you trust!",
                //                          "FTW Extension Loader", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes;
                //if (res) extWhitelist.Items.Add(exa);
                //return res;
            }
            
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
            {
                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    if (!Sel(file)) continue;
                    try
                    {
                        var ext = MainContainer.LoadExtension(AssemblyName.GetAssemblyName(file));
                        Logger.Info($"Loaded: {ext}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to load extension {file}", ex);
                    }
                    //if (extWhitelist.Items.Contains(exa)) return true;
                    //var res = MessageBox.Show($"Are you sure you want to load the following extension?\n\n{exa.Name} version {exa.Version}\nby {exa.Author}\n{exa.Homepage}\n\nOnly load extensions that you trust!",
                    //                          "FTW Extension Loader", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes;
                    //if (res) extWhitelist.Items.Add(exa);
                    //return res;
                }
            }
            //Settings.ExtensionWhitelist = extWhitelist;
            AppScope = MainContainer.OpenScope(nameof(App));
        }

        private void LoadSettings()
        {
            SettingsManager = AppScope.Resolve<IConfigManager>();
            SettingsManager.LoadAllConfigPages();
            Settings = AppScope.Resolve<UISettings>();

            // TODO: Remove below here when ready!
            //var provider = Settings.Properties["LastVersion"]?.Provider;
            //var dict = new SettingsAttributeDictionary
            //{
            //    {
            //        typeof(SettingsManageabilityAttribute),
            //        new SettingsManageabilityAttribute(SettingsManageability.Roaming)
            //    },
            //    {
            //        typeof(UserScopedSettingAttribute),
            //        new UserScopedSettingAttribute()
            //    }
            //};
            //DynSettings = ConfigurationUtils.GetAllConfigurableProperties(AppScope, ex => Logger.Error("Error getting configurable property:", ex)).ToList();
            //foreach (var info in DynSettings)
            //{
            //    try
            //    {
            //        Settings.Properties.Add(new SettingsProperty(info.Name, info.PropertyInfo.PropertyType,
            //                                                             provider, false, info.DefaultValue,
            //                                                             SettingsSerializeAs.String, dict, true, true));
            //        info.Value = Settings[info.Name];
            //    }
            //    catch (ConfigurationErrorsException cee)
            //    {
            //        Logger.Error("The settings failed to instantiate!", cee);
            //    }
            //    catch (SettingsPropertyNotFoundException spnfe)
            //    {
            //        Logger.Fatal("Unexpected settings error, Name=" + info.Name, spnfe);
            //    }
            //    catch (SettingsPropertyWrongTypeException spwte)
            //    {
            //        Logger.Error("Invalid default value for setting, Name=" + info.Name + " DefaultValue=" + info.DefaultValue, spwte);
            //    }
            //    catch (SettingsPropertyIsReadOnlyException spiroe)
            //    {
            //        Logger.Fatal("Unexpected settings error, Name=" + info.Name, spiroe);
            //    }
            //}
            //Logger.Debug("Dynamic properties: " + DynSettings.Count);
            //// Handle changes
            //Settings.PropertyChanged += (sender, args) =>
            //{
            //    var p = DynSettings.FirstOrDefault(i => i.Name == args.PropertyName);
            //    if (p == null) return;
            //    p.Value = Settings[p.Name];
            //};
        }

        private IConfigManager SettingsManager { get; set; }

        private void LoadSkins()
        {
            var sm = AppScope.Resolve<ISkinManager>();
            if (string.IsNullOrWhiteSpace(Settings.Skin)) SkinLoadingBehavior.UpdateSkin(sm.LoadFallbackSkin().SkinParts);
            else
            {
                SkinPackage skin;
                try { skin = sm.LoadSkin(Settings.Skin); }
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
            KeyBindingsManager.Instance.Listener.IsEnabled = Settings.EnableKeyboardHook;
            Settings.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(Settings.EnableKeyboardHook))
                    KeyBindingsManager.Instance.Listener.IsEnabled = Settings.EnableKeyboardHook;
            };
            if (Settings.KeyBindings != null)
                foreach (var keyBinding in Settings.KeyBindings)
                    KeyBindingsManager.Instance.KeyBindings.Add(keyBinding);
            KeyBindingsManager.Instance.KeyBindings.CollectionChanged +=
                (s, args) =>
                Settings.KeyBindings = new ObservableCollection<KeyBinding>(KeyBindingsManager.Instance.KeyBindings);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.</param>
        protected override void OnExit([NotNull] ExitEventArgs e)
        {
            using (LogicalThreadContext.Stacks["NDC"].Push(nameof(OnExit)))
            {
                Logger.Info("Saving settings...");
                SettingsManager.SaveAllConfigPages();
                Dispose();
            }
            base.OnExit(e);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || _mutex == null) return;
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
