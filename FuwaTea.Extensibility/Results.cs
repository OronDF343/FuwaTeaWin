namespace FuwaTea.Extensibility
{
    public enum AssemblyLoadResult
    {
        NotLoaded,
        OK,
        BadImageFormat,
        FileNotFound,
        FileLoadFailed,
        NullResult
    }

    public enum ExtensionCheckResult
    {
        NotLoaded,
        OK,
        NotAnExtension,
        ApiVersionMismatch,
        ProcessArchMismatch,
        OSKindMismatch,
        OSVersionMismatch
    }
}
