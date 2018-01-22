using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace FuwaTea.Lib.NotifyIcon
{
    /// <summary>
    /// Utillity class for safely handling pointers to structures in unmanaged memory.
    /// </summary>
    public static class InteropUtils
    {
        [NotNull]
        private static readonly Dictionary<IntPtr, int> AllocPtrs = new Dictionary<IntPtr, int>();

        /// <summary>
        /// Map a managed object to unmanaged memory.
        /// </summary>
        /// <param name="obj">A managed object (must be a struct or primitive type).</param>
        /// <returns>A pointer to unmanaged memory.</returns>
        public static IntPtr CreateStructPtr([NotNull] object obj)
        {
            var size = Marshal.SizeOf(obj);
            var ptr = Marshal.AllocHGlobal(size);
            AllocPtrs.Add(ptr, size);
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }

        /// <summary>
        /// Replace an object in unmanaged memory with another object.
        /// </summary>
        /// <remarks>
        /// The pointer must have been allocated by <see cref="CreateStructPtr"/>!
        /// Also, the operation will fail if not enough bytes of unmanaged memory are available at the location provided.
        /// </remarks>
        /// <param name="ptr">The pointer to unmanaged memory.</param>
        /// <param name="obj">A managed object (must be a struct or primitive type).</param>
        /// <returns><code>true</code> if the operation succeeded.</returns>
        public static bool UpdateStructPtr(IntPtr ptr, [NotNull] object obj)
        {
            if (!AllocPtrs.ContainsKey(ptr)) return false;
            var size = Marshal.SizeOf(obj);
            if (AllocPtrs[ptr] < size) return false;
            Marshal.StructureToPtr(obj, ptr, true);
            return true;
        }

        /// <summary>
        /// Frees unmanaged memory previously allocated using <see cref="CreateStructPtr"/>.
        /// </summary>
        /// <param name="ptr">The pointer to unmanaged memory.</param>
        /// <returns><code>true</code> if the operation succeeded.</returns>
        public static bool FreeStructPtr(IntPtr ptr)
        {
            if (!AllocPtrs.ContainsKey(ptr)) return false;
            Marshal.FreeHGlobal(ptr);
            AllocPtrs.Remove(ptr);
            return true;
        }

        /// <summary>
        /// Frees all of the unmanaged memory allocated using <see cref="CreateStructPtr"/>.
        /// DO NOT USE UNLESS ABSOLUTELY NECESSARY!
        /// </summary>
        public static void FreeAllPtrs()
        {
            foreach (var ptr in AllocPtrs.Keys)
                Marshal.FreeHGlobal(ptr);
            AllocPtrs.Clear();
        }
    }
}
