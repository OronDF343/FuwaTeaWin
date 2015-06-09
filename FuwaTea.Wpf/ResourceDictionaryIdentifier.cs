namespace FuwaTea.Wpf
{
    public class ResourceDictionaryIdentifier
    {
        /// <summary>
        /// Unique ID for this skin. Please use the format "author:name" to avoid duplicate IDs. THIS VALUE IS REQUIRED!
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// What is this skin called?
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// What is this skin?
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Version number.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Who made this?
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Where can this skin be found online?
        /// </summary>
        public string Homepage { get; set; }
        /// <summary>
        /// The parent skin must be loaded before this one! This is the unique ID of it.
        /// Don't set this value if you don't need it!
        /// </summary>
        public string Parent { get; set; }
        /// <summary>
        /// How can we use this skin? THIS VALUE IS REQUIRED!
        /// </summary>
        public ResourceDictionaryType SkinType { get; set; }
    }

    public enum ResourceDictionaryType
    {
        /// <summary>
        /// This skin should only ever be loaded by itself.
        /// </summary>
        Standalone = 0,
        /// <summary>
        /// This skin is a "base" skin which defines it's own styles, and is compatible with "addon" skins designed specifically for it. (and not for it's parent, if it has any)
        /// </summary>
        Base = 1,
        /// <summary>
        /// This skin is an "addon" skin which changes parts of it's parent's styles, but remains compatible with "color" skins designed for it's parent.
        /// </summary>
        Addon = 2,
        /// <summary>
        /// This skin is a "color" skin which changes it's parent's colors/brushes/etc, so no skin should be loaded after this one.
        /// </summary>
        Color = 3
    }
}
