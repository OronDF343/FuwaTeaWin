using System;
using System.Runtime.InteropServices;

namespace FuwaTea.Lib.NotifyIconHax
{
    public delegate void CallBack(NOTIFYITEM notifyitem);
    
    public interface ITrayManager : IDisposable, ILibComponent
    {
        void RegisterCallback(CallBack ncb);
        void SetPreference(NOTIFYITEM notifyItem);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NOTIFYITEM
    {
        public string pszExeName { get; set; }
        public string pszTip { get; set; }
        public IntPtr hIcon { get; set; }
        public IntPtr hWnd { get; set; }
        public uint dwPreference { get; set; }
        public uint uID { get; set; }
        public Guid GuidItem { get; set; }
    }
}
