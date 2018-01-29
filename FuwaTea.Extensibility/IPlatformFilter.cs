namespace FuwaTea.Extensibility
{
    public interface IPlatformFilter
    {
        FilterAction Action { get; }
        OSKind OSKind { get; }
        FilterRule Rule { get; }
        ProcessArch ProcessArchitecture { get; }
        string Version { get; }
        string OtherVersion { get; }
    }
}
