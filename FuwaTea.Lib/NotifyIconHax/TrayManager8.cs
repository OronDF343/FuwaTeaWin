using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ModularFramework.Attributes;

namespace FuwaTea.Lib.NotifyIconHax
{
    [LibComponent("Windows 8 Tray Manager")]
    [OSFilter(FilterActions.Blacklist, PlatformID.Win32NT, FilterRules.GreaterThan, "6.3.0.0")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.GreaterThan, "6.2.0.0")]
    public class TrayManager8 : ITrayManager
    {
        private readonly ITrayNotify8 _trayNotify;
        private readonly IntPtr _ptr;
        public TrayManager8()
        {
            _trayNotify = new TrayNotify8();
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

    [CoClass(typeof(TrayNotify8))]
    [ComImport]
    [Guid("FB852B2C-6BAD-4605-9551-F15F87830935")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeLibImportClass(typeof(TrayNotify8))]
    //Virtual Functions for Windows 8
    public interface ITrayNotify8
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
    public sealed class TrayNotify8 : ITrayNotify8
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
