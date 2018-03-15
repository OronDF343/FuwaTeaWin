using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DryIocAttributes;
using FuwaTea.Extensibility;
using JetBrains.Annotations;

namespace FuwaTea.Lib.NotifyIcon
{
    // [LibComponent("Windows XP/Vista/7 Tray Manager")]
    public class TrayManager7 : ITrayManager
    {
        [NotNull]
        private readonly ITrayNotify7 _trayNotify;
        private readonly IntPtr _itemPtr;

        public TrayManager7()
        {
            _trayNotify = new TrayNotify7();
            _itemPtr = InteropUtils.CreateStructPtr(new NOTIFYITEM());
        }

        public bool GetNotifyItems(NotifyItemCallback ncb)
        {
            var hr = _trayNotify.RegisterCallback(new NotificationCallbackWrapper(ncb));
            _trayNotify.RegisterCallback(null);
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

    [CoClass(typeof(TrayNotify7))]
    [ComImport]
    [Guid("FB852B2C-6BAD-4605-9551-F15F87830935")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeLibImportClass(typeof(TrayNotify7))]
    public interface ITrayNotify7
    {
        int RegisterCallback(INotificationCB ncb);

        int SetPreference(IntPtr notifyItem);

        int EnableAutoTray([MarshalAs(UnmanagedType.Bool)] bool b);
    }

    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    [Guid("25DEAD04-1EAC-4911-9E3A-AD0A4AB560FD")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    public sealed class TrayNotify7 : ITrayNotify7
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int RegisterCallback(INotificationCB ncb);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int SetPreference(IntPtr notifyItem);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int EnableAutoTray(bool b);
    }
}
