namespace FuwaTea.Extensibility
{
    public interface IPlatformFilter
    {
        FilterAction Action { get; }
        OSKind OSKind { get; }
        //FilterRule Rule { get; }
        Architecture ProcessArchitecture { get; }
        //string Version { get; }
    }
}
