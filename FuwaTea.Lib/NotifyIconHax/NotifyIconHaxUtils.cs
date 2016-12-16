using System;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using ModularFramework;

namespace FuwaTea.Lib.NotifyIconHax
{
    // Based on https://hianz.wordpress.com/2013/09/03/new-windows-tray-notification-manager-is-here/
    public static class NotifyIconHaxUtils
    {
        // ReSharper disable once MissingAnnotation
        private static ITrayManager _trayManager;

        /// <summary>
        /// Set the NotifyIcon preference for a program.
        /// </summary>
        /// <param name="exeFullPath">The absolute path to the executable.</param>
        /// <param name="pref">The preference to be set.</param>
        public static bool SetPreference([NotNull] string exeFullPath, NOTIFYITEM_PREFERENCE pref)
        {
            var logger = LogManager.GetLogger(typeof(NotifyIconHaxUtils));
            logger.Debug("SetPreference called");
            if (_trayManager == null) _trayManager = ModuleFactory.GetElement<ITrayManager>();
            logger.Debug("Registering callback");
            return _trayManager.GetNotifyItems(notifyitem =>
            {
                var nexe = notifyitem.ExeName;
                var match = Regex.Match(notifyitem.ExeName, @"^\{[0-9a-fA-F-]{32,36}\}");
                if (match.Success)
                {
                    var guid = Guid.Parse(notifyitem.ExeName.Substring(0, match.Length));
                    string knownFolder;
                    NativeMethods.SHGetKnownFolderPath(guid, 0, IntPtr.Zero, out knownFolder);
                    if (!string.IsNullOrWhiteSpace(knownFolder))
                        nexe = Path.Combine(knownFolder, nexe.Substring(match.Length + 1));
                }
                logger.Debug("EXE path expanded: " + nexe);
                if (!string.Equals(nexe, exeFullPath, StringComparison.OrdinalIgnoreCase)) return;
                notifyitem.Preference = (uint)pref;
                _trayManager.SetPreference(notifyitem);
            });
        }

        /// <summary>
        /// Dispose the COM object that was used.
        /// </summary>
        public static void Dispose()
        {
            _trayManager?.Dispose(); // TODO: Dispose at end
        }
    }
}
