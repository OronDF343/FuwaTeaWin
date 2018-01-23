using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;
using DryIocAttributes;
using JetBrains.Annotations;
using log4net;

namespace FuwaTea.Lib.NotifyIcon
{
    // Based on https://hianz.wordpress.com/2013/09/03/new-windows-tray-notification-manager-is-here/
    [Export]
    [Reuse(ReuseType.Singleton)]
    public class NotifyIconManager : IDisposable
    {
        private readonly ITrayManager _trayManager;

        [ImportingConstructor]
        public NotifyIconManager([Import] ITrayManager trayManager)
        {
            _trayManager = trayManager;
        }

        /// <summary>
        /// Set the NotifyIcon preference for a program.
        /// </summary>
        /// <param name="exeFullPath">The absolute path to the executable.</param>
        /// <param name="pref">The preference to be set.</param>
        public bool SetPreference([NotNull] string exeFullPath, NOTIFYITEM_PREFERENCE pref)
        {
            var logger = LogManager.GetLogger(typeof(NotifyIconManager));
            logger.Debug("Registering callback");
            return _trayManager.GetNotifyItems(notifyitem =>
            {
                var nexe = notifyitem.ExeName;
                var match = Regex.Match(notifyitem.ExeName, @"^\{[0-9a-fA-F-]{32,36}\}");
                if (match.Success)
                {
                    var guid = Guid.Parse(notifyitem.ExeName.Substring(0, match.Length));
                    NativeMethods.SHGetKnownFolderPath(guid, 0, IntPtr.Zero, out var knownFolder);
                    if (!string.IsNullOrWhiteSpace(knownFolder))
                        nexe = Path.Combine(knownFolder, nexe.Substring(match.Length + 1));
                }
                logger.Debug("EXE path expanded: " + nexe);
                if (!string.Equals(nexe, exeFullPath, StringComparison.OrdinalIgnoreCase)) return;
                notifyitem.Preference = (uint)pref;
                _trayManager.SetPreference(notifyitem);
            });
        }

        /// <inheritdoc />
        /// <summary>
        /// Dispose the COM object that was used.
        /// </summary>
        public void Dispose()
        {
            _trayManager?.Dispose();
        }
    }
}
