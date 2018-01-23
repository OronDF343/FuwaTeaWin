using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FuwaTea.Extensibility;

namespace FuwaTea.Lib.NotifyIcon
{
    //[LibComponent("Windows 8.1+ Tray Manager")]
    [Export(typeof(ITrayManager))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PlatformFilter(FilterAction.Whitelist, OSKind.Windows, OSArch.Any, FilterRule.GreaterThan, "6.3.0.0")]
    public class TrayManager81 : ITrayManager
    {
        private readonly ITrayNotify81 _trayNotify;
        private readonly IntPtr _itemPtr;

        public TrayManager81()
        {
            _trayNotify = new TrayNotify81();
            _itemPtr = InteropUtils.CreateStructPtr(new NOTIFYITEM());
        }

        public bool GetNotifyItems(NotifyItemCallback ncb)
        {
            var i = 0;
            var hr = _trayNotify.RegisterCallback(new NotificationCallbackWrapper(ncb), ref i);
            _trayNotify.UnregisterCallback(ref i);
            return hr >= 0;
        }

        public bool SetPreference(NOTIFYITEM notifyItem)
        {
            if (!InteropUtils.UpdateStructPtr(_itemPtr, notifyItem)) return false;
            return _trayNotify.SetPreference(_itemPtr) >= 0;
        }

        public void Dispose()
        {
            InteropUtils.FreeStructPtr(_itemPtr);
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
        int RegisterCallback(INotificationCB ncb, ref int ci);

        int UnregisterCallback(ref int ci);

        int SetPreference(IntPtr notifyItem);

        int EnableAutoTray([MarshalAs(UnmanagedType.Bool)] bool b);

        //Look at virtual function table for correct signature
        int DoAction([MarshalAs(UnmanagedType.Bool)] bool b);
    }

    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    [Guid("25DEAD04-1EAC-4911-9E3A-AD0A4AB560FD")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    public sealed class TrayNotify81 : ITrayNotify81
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int RegisterCallback(INotificationCB ncb, ref int ci);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int UnregisterCallback(ref int ci);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int SetPreference(IntPtr notifyItem);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int EnableAutoTray(bool b);

        [MethodImpl(MethodImplOptions.InternalCall)]
        //Look at virtual function table for correct signature
        public extern int DoAction(bool b);
    }
}
