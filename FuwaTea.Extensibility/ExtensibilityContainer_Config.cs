using System.IO;
using DryIoc;
using FuwaTea.Extensibility.Config;

namespace FuwaTea.Extensibility
{
    // ReSharper disable once MismatchedFileName
    public partial class ExtensibilityContainer
    {
        public string PersistentConfigDir { get; private set; }
        public string NonPersistentConfigDir { get; private set; }

        /// <summary>
        /// Gets whether configuration can currently be loaded/saved.
        /// </summary>
        public bool IsConfigEnabled { get; private set; }

        /// <summary>
        /// Initialize the extensibility container.
        /// </summary>
        public ExtensibilityContainer()
        {
            IsConfigEnabled = false;
        }
        
        /// <summary>
        /// Set the config directories.
        /// </summary>
        /// <param name="persistentConfigDir">Directory for persistent configuration (AppData\Roaming).</param>
        /// <param name="nonPersistentConfigDir">Directory for non-persistent configuration (AppData\Local).</param>
        public void SetConfigDirs(string persistentConfigDir, string nonPersistentConfigDir)
        {
            IsConfigEnabled = false;
            PersistentConfigDir = persistentConfigDir;
            NonPersistentConfigDir = nonPersistentConfigDir;
            if (!Directory.Exists(PersistentConfigDir)) Directory.CreateDirectory(PersistentConfigDir);
            if (!Directory.Exists(NonPersistentConfigDir)) Directory.CreateDirectory(NonPersistentConfigDir);
            IsConfigEnabled = true;
        }

        /// <summary>
        /// Load a single config page.
        /// </summary>
        /// <param name="key">The key of the page.</param>
        /// <returns>The page instance.</returns>
        public IConfigPage LoadConfigPage(string key)
        {
            var page = IocContainer.Resolve<Meta<IConfigPage, IConfigPageMetadata>>(key);
            var iPath = Path.Combine(page.Metadata.IsPersistent ? PersistentConfigDir : NonPersistentConfigDir,
                                     page.Metadata.Key + ConfigConstants.ConfigFileExtension);
            if (File.Exists(iPath))
                page.Value.Deserialize(File.ReadAllText(iPath));
            return page.Value;
        }
        
        /// <summary>
        /// Save a single config page.
        /// </summary>
        /// <param name="key">The key of the page.</param>
        /// <returns>The page instance.</returns>
        public IConfigPage SaveConfigPage(string key)
        {
            var page = IocContainer.Resolve<Meta<IConfigPage, IConfigPageMetadata>>(key);
            var iPath = Path.Combine(page.Metadata.IsPersistent ? PersistentConfigDir : NonPersistentConfigDir,
                                     page.Metadata.Key + ConfigConstants.ConfigFileExtension);
            File.WriteAllText(iPath, page.Value.Serialize());
            return page.Value;
        }

        /// <summary>
        /// Load all configuration pages.
        /// </summary>
        public void LoadAllConfigPages()
        {
            foreach (var item in IocContainer.ResolveMany<Meta<IConfigPage, IConfigPageMetadata>>())
            {
                var iPath = Path.Combine(item.Metadata.IsPersistent ? PersistentConfigDir : NonPersistentConfigDir,
                                         item.Metadata.Key + ConfigConstants.ConfigFileExtension);
                if (!File.Exists(iPath)) continue;
                item.Value.Deserialize(File.ReadAllText(iPath));
            }
        }

        /// <summary>
        /// Save all configuration pages.
        /// </summary>
        public void SaveAllConfigPages()
        {
            foreach (var item in IocContainer.ResolveMany<Meta<IConfigPage, IConfigPageMetadata>>())
            {
                var iPath = Path.Combine(item.Metadata.IsPersistent ? PersistentConfigDir : NonPersistentConfigDir,
                                         item.Metadata.Key + ConfigConstants.ConfigFileExtension);
                File.WriteAllText(iPath, item.Value.Serialize());
            }
        }
    }
}
