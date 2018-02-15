namespace FuwaTea.Config
{
    public interface IConfigPageMetadata
    {
        // TODO: Might not be necessary
        string Key { get; }
        // Key of IConfigFile to store it
        string ConfigFileKey { get; }
        // Whether this is a user-configurable page or just some cache data, etc.
        bool ShouldDisplayInUI { get; }
    }
}
