namespace FuwaTea.Extensibility.Config
{
    public interface IConfigPageMetadata
    {
        string Key { get; }
        // Whether this is an important page or just some cache data, etc.
        bool IsPersistent { get; }
        // Whether this page shoould be editable in the UI
        bool IsUserEditable { get; }
    }
}
