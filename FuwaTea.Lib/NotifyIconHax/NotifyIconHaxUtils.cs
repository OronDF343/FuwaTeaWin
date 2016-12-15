using System;
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
                logger.Debug(notifyitem.ExeName);
                if (!string.Equals(notifyitem.ExeName, exeFullPath, StringComparison.OrdinalIgnoreCase)) return;
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
