using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FuwaTea.Lib
{
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    [Guid("1968106d-f3b5-44cf-890e-116fcb9ecef1")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    public sealed class ApplicationAssociationRegistrationUI : IApplicationAssociationRegistrationUI
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void LaunchAdvancedAssociationUI(string appRegistryName);
    }

    [CoClass(typeof(ApplicationAssociationRegistrationUI))]
    [ComImport]
    [Guid("1f76a169-f994-40ac-8fc8-0959e8874710")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeLibImportClass(typeof(ApplicationAssociationRegistrationUI))]
    public interface IApplicationAssociationRegistrationUI
    {
        void LaunchAdvancedAssociationUI([MarshalAs(UnmanagedType.LPWStr)] string appRegistryName);
    }
}
