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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Windows;
using FTWPlayer.Properties;
using FuwaTea.Lib;
using FuwaTea.Presentation.Playback;
using GalaSoft.MvvmLight.Threading;
using LayerFramework;
using log4net;
using log4net.Config;
using Microsoft.Win32;

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

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Assembly.GetExecutingAssembly().GetExeFolder(),
                                                                        "logconfig-debug.xml")));
#else
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Assembly.GetExecutingAssembly().GetExeFolder(),
                                                                        "logconfig.xml")));
#endif
            // TODO: create ErrorDialog
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => LogManager.GetLogger(GetType()).Fatal("An unhandled exception occured:", (Exception)args.ExceptionObject);

            // Get ClArgs:
            var clArgs = Environment.GetCommandLineArgs().ToList();
            var isinst = Assembly.GetEntryAssembly().IsInstalledCopy();
            var prod = Assembly.GetEntryAssembly().GetProduct();
            if (clArgs.Contains("--setup-file-associations") && isinst)
            {
                LayerFactory.LoadFolder(Assembly.GetEntryAssembly().GetExeFolder(), Console.WriteLine, true);
                var pm = LayerFactory.GetElement<IPlaybackManager>();
                var key = Registry.LocalMachine.CreateSubKey(string.Format(@"Software\Clients\Media\{0}\Capabilities\FileAssociations", prod));
                var exts = key.GetValueNames();
                var f = exts.Where(s => !pm.SupportedFileTypes.Contains(s));
                var t = pm.SupportedFileTypes.Where(s => !exts.Contains(s));
                foreach (var s in f) key.DeleteValue(s);
                foreach (var s in t) key.SetValue(s, prod + ".AudioFileGeneric", RegistryValueKind.String);
                Shutdown();
                return;
            }
            if (clArgs.Contains("--clean-up-file-associations") && isinst)
            {
                LayerFactory.LoadFolder(Assembly.GetEntryAssembly().GetExeFolder(), Console.WriteLine, true);
                var pm = LayerFactory.GetElement<IPlaybackManager>();
                var key = Registry.LocalMachine.CreateSubKey(string.Format(@"Software\Clients\Media\{0}\Capabilities\FileAssociations", prod));
                foreach (var s in key.GetValueNames()) key.DeleteValue(s);
                Shutdown();
                return;
            }
            if (clArgs.Contains("--configure-file-associations") && isinst)
            {
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

            bool mutexCreated;
            var mutexName = prod + (isinst ? "|Installed" : "|Portable");
            _mutex = new Mutex(true, mutexName, out mutexCreated);
            Message = (int)NativeMethods.RegisterWindowMessage(mutexName);


            if (!mutexCreated)
            {
                if (!clArgs.Contains("--wait")) // TODO: better handling of clArgs
                {
                    _mutex = null;
                    if (clArgs.Count > 1)
                    {
                        File.WriteAllLines(Path.Combine(Assembly.GetEntryAssembly().GetExeFolder(), @"ClArgs.txt"), clArgs); // TODO: find better way of passing args
                        NativeMethods.SendMessage(NativeMethods.HWND_BROADCAST, Message, IntPtr.Zero, IntPtr.Zero);
                    }
                    Shutdown();
                    return;
                }
                if (!_mutex.WaitOne(Settings.Default.InstanceCreationTimeout))
                {
                    MessageBox.Show("Failed to create an instance: The operation has timed out.", "Single Instance Application", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                    return;
                }
                if (!Mutex.TryOpenExisting(mutexName, MutexRights.FullControl, out _mutex))
                {
                    _mutex = new Mutex(true, mutexName, out mutexCreated);
                    if (!mutexCreated)
                    {
                        MessageBox.Show("Failed to create an instance: Cannot open or create the Mutex!", "Single Instance Application", MessageBoxButton.OK, MessageBoxImage.Error);
                        Shutdown();
                        return;
                    }
                }
            }

            // Set Priority: 
            Process.GetCurrentProcess().PriorityClass = Settings.Default.ProcessPriority;

            // Manually show main window (pervents loading it on shutdown)
            MainWindow = new MainWindow();
            MainWindow.Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            Dispose();
            base.OnExit(e);
        }

        private void Dispose(Boolean disposing)
        {
            if (disposing && (_mutex != null))
            {
                _mutex.ReleaseMutex();
                _mutex.Close();
                _mutex = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
