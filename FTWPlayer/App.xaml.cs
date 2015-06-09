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
            // Error handling: TODO: create ErrorDialog
            //AppDomain.CurrentDomain.UnhandledException +=
            //    (s, args) =>
            //    {
            //        var em = new ErrorMessage((Exception)args.ExceptionObject);
            //        em.ShowDialog();

            //    };

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
                var f = exts.Where(s => !pm.SupportedFileTypes.Contains(s.TrimStart('.')));
                var t = pm.SupportedFileTypes.Where(s => !exts.Contains("." + s));
                foreach (var s in f) key.DeleteValue(s);
                foreach (var s in t) key.SetValue("." + s, prod + ".AudioFileGeneric", RegistryValueKind.String);
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
            Dispose();
            Settings.Default.Save();
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
