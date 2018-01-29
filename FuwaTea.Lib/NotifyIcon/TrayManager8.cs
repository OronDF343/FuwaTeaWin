using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DryIocAttributes;
using FuwaTea.Extensibility;

namespace FuwaTea.Lib.NotifyIcon
{
    //[LibComponent("Windows 8 Tray Manager")]
    [Reuse(ReuseType.Singleton)]
    [PlatformFilter(FilterAction.Whitelist, OSKind.Windows, ProcessArch.Any, FilterRule.Between, "6.2.0.0", OtherVersion = "6.3.0.0")]
    public class TrayManager8 : ITrayManager
    {
        private readonly ITrayNotify8 _trayNotify;
        private readonly IntPtr _itemPtr;

        public TrayManager8()
        {
            _trayNotify = new TrayNotify8();
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

    [CoClass(typeof(TrayNotify8))]
    [ComImport]
    [Guid("FB852B2C-6BAD-4605-9551-F15F87830935")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeLibImportClass(typeof(TrayNotify8))]
    //Virtual Functions for Windows 8
    public interface ITrayNotify8
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
    public sealed class TrayNotify8 : ITrayNotify8
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
