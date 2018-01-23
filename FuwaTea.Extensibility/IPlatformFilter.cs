namespace FuwaTea.Extensibility
{
    public interface IPlatformFilter
    {
        FilterAction Action { get; }
        OSKind OSKind { get; }
        //FilterRule Rule { get; }
        OSArch ProcessArchitecture { get; }
        //string Version { get; }
    }
}
