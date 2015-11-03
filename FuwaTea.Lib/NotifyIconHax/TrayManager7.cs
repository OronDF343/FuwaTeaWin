using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ModularFramework.Attributes;

namespace FuwaTea.Lib.NotifyIconHax
{
    [LibComponent("Windows XP/Vista/7 Tray Manager")]
    [OSFilter(FilterActions.Whitelist, PlatformID.Win32NT, FilterRules.LessThan, "6.2.0.0")]
    public class TrayManager7 : ITrayManager
    {
        private readonly ITrayNotify7 _trayNotify;
        private readonly IntPtr _ptr;
        public TrayManager7()
        {
            _trayNotify = new TrayNotify7();
            _ptr = InteropUtils.CreateStructPtr(new NOTIFYITEM());
        }

        public void RegisterCallback(CallBack ncb)
        {
            _trayNotify.RegisterCallback(new NotificationCallBackWrapper(ncb));
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

    [CoClass(typeof(TrayNotify7))]
    [ComImport]
    [Guid("FB852B2C-6BAD-4605-9551-F15F87830935")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeLibImportClass(typeof(TrayNotify7))]
    public interface ITrayNotify7
    {
        void RegisterCallback(INotificationCB ncb);

        void SetPreference(IntPtr notifyItem);

        void EnableAutoTray([MarshalAs(UnmanagedType.Bool)] bool b);
    }

    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    [Guid("25DEAD04-1EAC-4911-9E3A-AD0A4AB560FD")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    public sealed class TrayNotify7 : ITrayNotify7
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void RegisterCallback(INotificationCB ncb);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void SetPreference(IntPtr notifyItem);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void EnableAutoTray(bool b);
    }
}
