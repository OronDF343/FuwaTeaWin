using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FuwaTea.Lib.NotifyIconHax
{
    public static class InteropUtils
    {
        private static readonly List<IntPtr> AllocPtrs = new List<IntPtr>();

        public static IntPtr CreateStructPtr(object obj)
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
            AllocPtrs.Add(ptr);
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }

        public static bool UpdateStructPtr(IntPtr ptr, object obj)
        {
            if (!AllocPtrs.Contains(ptr)) return false;
            Marshal.StructureToPtr(obj, ptr, true);
            return true;
        }

        public static bool FreeStructPtr(IntPtr ptr)
        {
            if (!AllocPtrs.Contains(ptr)) return false;
            Marshal.FreeHGlobal(ptr);
            AllocPtrs.Remove(ptr);
            return true;
        }

        public static void FreeAllPtrs()
        {
            foreach (var ptr in AllocPtrs)
                Marshal.FreeHGlobal(ptr);
            AllocPtrs.Clear();
        }
    }
}
