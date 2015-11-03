using System;
using ModularFramework;

namespace FuwaTea.Lib.NotifyIconHax
{
    // Based on https://hianz.wordpress.com/2013/09/03/new-windows-tray-notification-manager-is-here/
    public static class NotifyIconHaxUtils
    {
        private static ITrayManager _trayManager;

        public static void SetPreference(string exeFullPath, uint pref)
        {
            if (_trayManager == null) _trayManager = ModuleFactory.GetElement<ITrayManager>();
            _trayManager.RegisterCallback(notifyitem =>
            {
                if (!string.Equals(notifyitem.pszExeName, exeFullPath, StringComparison.OrdinalIgnoreCase)) return;
                notifyitem.dwPreference = pref;
                _trayManager.SetPreference(notifyitem);
            });
        }

        public static void Dispose()
        {
            _trayManager?.Dispose(); // TODO: Dispose at end
        }
    }
}
