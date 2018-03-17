using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace FuwaTea.Lib.NotifyIcon
{
    /// <summary>
    /// The COM interface which defines a NotifyIcon callback.
    /// </summary>
    [ComImport]
    [Guid("D782CCBA-AFB0-43F1-94DB-FDA3779EACCB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INotificationCB
    {
        /// <summary>
        /// The callback method.
        /// </summary>
        /// <param name="e">The event code.</param>
        /// <param name="notifyItem">A pointer to the NotifyIcon info (of type <see cref="NOTIFYITEM"/>).</param>
        void Notify(uint e, IntPtr notifyItem);
    }

    /// <summary>
    /// A wrapper for <see cref="INotificationCB"/> which adds support for C# delegates.
    /// </summary>
    public class NotificationCallbackWrapper : INotificationCB
    {
        /// <inheritdoc/>
        public void Notify(uint e, IntPtr notifyItem)
        {
            AddedCallBack.Invoke(Marshal.PtrToStructure<NOTIFYITEM>(notifyItem));
        }
        
        [NotNull]
        private NotifyItemCallback AddedCallBack { get; }

        /// <summary>
        /// Creates a new wrapper for a <see cref="NotifyItemCallback"/>.
        /// </summary>
        /// <param name="cb">A <see cref="NotifyItemCallback"/> delegate.</param>
        public NotificationCallbackWrapper([NotNull] NotifyItemCallback cb)
        {
            AddedCallBack = cb;
        }
    }
}
