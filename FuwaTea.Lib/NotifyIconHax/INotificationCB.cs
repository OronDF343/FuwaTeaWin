using System;
using System.Runtime.InteropServices;

namespace FuwaTea.Lib.NotifyIconHax
{
    [ComImport]
    [Guid("D782CCBA-AFB0-43F1-94DB-FDA3779EACCB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface INotificationCB
    {
        void Notify(uint e, IntPtr notifyItem);
    }

    public class NotificationCallBackWrapper : INotificationCB
    {
        public void Notify(uint e, IntPtr notifyItem)
        {
            AddedCallBack?.Invoke(Marshal.PtrToStructure<NOTIFYITEM>(notifyItem));
        }

        private CallBack AddedCallBack { get; }

        public NotificationCallBackWrapper(CallBack cb)
        {
            AddedCallBack = cb;
        }
    }
}
