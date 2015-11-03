using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ModularFramework.Attributes;

namespace FuwaTea.Lib.NotifyIconHax
{
    [LibComponent("Windows 8.1/10 Tray Manager")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "6.3.0.0")]
    public class TrayManager81 : ITrayManager
    {
        private readonly ITrayNotify81 _trayNotify;
        private readonly IntPtr _ptr;
        public TrayManager81()
        {
            _trayNotify = new TrayNotify81();
            _ptr = InteropUtils.CreateStructPtr(new NOTIFYITEM());
        }

        public void RegisterCallback(CallBack ncb)
        {
            uint i = 0;
            _trayNotify.RegisterCallback(new NotificationCallBackWrapper(ncb), ref i);
        }

        public void SetPreference(NOTIFYITEM notifyItem)
        {
            if (InteropUtils.UpdateStructPtr(_ptr, notifyItem))
                _trayNotify.SetPreference(_ptr);
        }

        public void Dispose()
        {
            InteropUtils.FreeStructPtr(_ptr);
            Marshal.ReleaseComObject(_trayNotify);
        }
    }

    [CoClass(typeof(TrayNotify81))]
    [ComImport]
    [Guid("D133CE13-3537-48BA-93A7-AFCD5D2053B4")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeLibImportClass(typeof(TrayNotify81))]
    //Virtual Functions for Windows 8
    public interface ITrayNotify81
    {
        void RegisterCallback(INotificationCB ncb, ref uint ci);

        void UnregisterCallback(ref uint ci);

        void SetPreference(IntPtr notifyItem);

        void EnableAutoTray([MarshalAs(UnmanagedType.Bool)] bool b);

        //Look at virtual function table for correct signature
        void DoAction([MarshalAs(UnmanagedType.Bool)] bool b);
    }

    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    [Guid("25DEAD04-1EAC-4911-9E3A-AD0A4AB560FD")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    public sealed class TrayNotify81 : ITrayNotify81
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RegisterCallback(INotificationCB ncb, ref uint ci);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void UnregisterCallback(ref uint ci);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetPreference(IntPtr notifyItem);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void EnableAutoTray(bool b);

        [MethodImpl(MethodImplOptions.InternalCall)]
        //Look at virtual function table for correct signature
        public extern void DoAction(bool b);
    }
}
