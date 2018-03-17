using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace FuwaTea.Lib.NotifyIcon
{
    /// <summary>
    /// Callback when info about a NotifyIcon is recieved.
    /// </summary>
    /// <param name="notifyitem">The information that was recieved.</param>
    public delegate void NotifyItemCallback(NOTIFYITEM notifyitem);
    
    /// <summary>
    /// A wrapper for the ITrayManager COM interface, which is different between versions of Windows.
    /// </summary>
    public interface ITrayManager : IDisposable
    {
        /// <summary>
        /// Get information about NotifyIcons.
        /// </summary>
        /// <param name="ncb">Callback when info about a NotifyIcon is recieved.</param>
        /// <returns><code>true</code> if the callback registration succeeded.</returns>
        bool GetNotifyItems([NotNull] NotifyItemCallback ncb);

        /// <summary>
        /// Update the preference information for a specific NotifyIcon.
        /// </summary>
        /// <param name="notifyItem">The updated preference information.</param>
        /// <returns><code>true</code> if the operation succeeded.</returns>
        bool SetPreference(NOTIFYITEM notifyItem);
    }

    /// <summary>
    /// The known NotifyIcon preference values.
    /// </summary>
    public enum NOTIFYITEM_PREFERENCE : uint
    {
        /// <summary>
        /// In Windows UI: "Only show notifications."
        /// </summary>
        WhenActive = 0,
        /// <summary>
        /// In Windows UI: "Hide icon and notifications."
        /// </summary>
        Never = 1,
        /// <summary>
        /// In Windows UI: "Show icon and notifications."
        /// </summary>
        Always = 2
    };

    /// <summary>
    /// Information about a NotifyIcon.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NOTIFYITEM
    {
        /// <summary>
        /// The absolute path to the executable.
        /// </summary>
        public string ExeName { get; set; }
        /// <summary>
        /// The last tooltip that was displayed.
        /// </summary>
        public string ToolTip { get; set; }
        /// <summary>
        /// The last icon that was displayed.
        /// </summary>
        public IntPtr Icon { get; set; }
        /// <summary>
        /// The HWND of the application (if it is running).
        /// </summary>
        public IntPtr Hwnd { get; set; }
        /// <summary>
        /// The preference value (see <see cref="NOTIFYITEM_PREFERENCE"/>).
        /// </summary>
        public uint Preference { get; set; }
        /// <summary>
        /// The unique identifier of the NotifyIcon.
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// The <see cref="Guid"/> of the NotifyIcon.
        /// </summary>
        public Guid Guid { get; set; }
    }
}
