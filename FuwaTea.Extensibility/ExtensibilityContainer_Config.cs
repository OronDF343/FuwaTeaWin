using System;
using System.Collections.Generic;
using System.IO;
using DryIoc;
using FuwaTea.Extensibility.Config;

namespace FuwaTea.Extensibility
{
    // ReSharper disable once MismatchedFileName
    public partial class ExtensibilityContainer
    {
        /// <summary>
        /// Directory for persistent configuration (AppData\Roaming).
        /// </summary>
        public string PersistentConfigDir { get; private set; }

        /// <summary>
        /// Directory for non-persistent configuration (AppData\Local).
        /// </summary>
        public string NonPersistentConfigDir { get; private set; }

        /// <summary>
        /// Gets whether configuration can currently be loaded/saved.
        /// </summary>
        public bool IsConfigEnabled { get; private set; }

        private readonly HashSet<string> _loadedPages;

        /// <summary>
        /// Initialize the extensibility container.
        /// </summary>
        public ExtensibilityContainer()
        {
            IsConfigEnabled = false;
            _loadedPages = new HashSet<string>();
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
        /// Add a config page registration.
        /// This will register the page as <see cref="IConfigPage"/>, and as itself, as a singleton with the key and with the metadata.
        /// </summary>
        /// <remarks>Should only be used where absolutely required.</remarks>
        /// <param name="metadata">The required metadata.</param>
        /// <typeparam name="TConfig">The implementation type.</typeparam>
        public TConfig RegisterConfigPage<TConfig>(IConfigPageMetadata metadata) where TConfig : IConfigPage
        {
            Register<IConfigPage, TConfig>(metadata.Key, Reuse.Singleton, metadata);
            Register<TConfig>(metadata.Key, Reuse.Singleton, metadata);
            return IocContainer.Resolve<TConfig>();
        }

        /// <summary>
        /// Load a single config page.
        /// </summary>
        /// <param name="key">The key of the page.</param>
        /// <exception cref="InvalidOperationException">When calling this method before setting the config directories.</exception>
        /// <returns>The page instance.</returns>
        public IConfigPage LoadConfigPage(string key)
        {
            if (!IsConfigEnabled) throw new InvalidOperationException("Config not enabled!");
            var page = IocContainer.Resolve<Meta<IConfigPage, IConfigPageMetadata>>(key);
            var iPath = Path.Combine(page.Metadata.IsPersistent ? PersistentConfigDir : NonPersistentConfigDir,
                                     page.Metadata.Key + ConfigConstants.ConfigFileExtension);
            if (File.Exists(iPath))
                page.Value.Deserialize(File.ReadAllText(iPath));
            _loadedPages.Add(key);
            return page.Value;
        }
        
        /// <summary>
        /// Save a single config page.
        /// </summary>
        /// <param name="key">The key of the page.</param>
        /// <exception cref="InvalidOperationException">When calling this method before setting the config directories.</exception>
        /// <returns>The page instance.</returns>
        public IConfigPage SaveConfigPage(string key)
        {
            if (!IsConfigEnabled) throw new InvalidOperationException("Config not enabled!");
            var page = IocContainer.Resolve<Meta<IConfigPage, IConfigPageMetadata>>(key);
            var iPath = Path.Combine(page.Metadata.IsPersistent ? PersistentConfigDir : NonPersistentConfigDir,
                                     page.Metadata.Key + ConfigConstants.ConfigFileExtension);
            File.WriteAllText(iPath, page.Value.Serialize());
            return page.Value;
        }
        
        /// <summary>
        /// Load all configuration pages.
        /// </summary>
        /// <param name="enableReload">Whether to enable reloading pages that have already been loaded.</param>
        /// <exception cref="InvalidOperationException">When calling this method before setting the config directories.</exception>
        public void LoadAllConfigPages(bool enableReload = false)
        {
            if (!IsConfigEnabled) throw new InvalidOperationException("Config not enabled!");
            // TODO: Handle exceptions
            foreach (var item in IocContainer.ResolveMany<Meta<IConfigPage, IConfigPageMetadata>>())
            {
                if (!enableReload && _loadedPages.Contains(item.Metadata.Key)) continue;
                var iPath = Path.Combine(item.Metadata.IsPersistent ? PersistentConfigDir : NonPersistentConfigDir,
                                         item.Metadata.Key + ConfigConstants.ConfigFileExtension);
                if (!File.Exists(iPath)) continue;
                item.Value.Deserialize(File.ReadAllText(iPath));
                _loadedPages.Add(item.Metadata.Key);
            }
        }

        /// <summary>
        /// Save all configuration pages.
        /// </summary>
        /// <exception cref="InvalidOperationException">When calling this method before setting the config directories.</exception>
        public void SaveAllConfigPages()
        {
            if (!IsConfigEnabled) throw new InvalidOperationException("Config not enabled!");
            // TODO: Handle exceptions
            foreach (var item in IocContainer.ResolveMany<Meta<IConfigPage, IConfigPageMetadata>>())
            {
                var iPath = Path.Combine(item.Metadata.IsPersistent ? PersistentConfigDir : NonPersistentConfigDir,
                                         item.Metadata.Key + ConfigConstants.ConfigFileExtension);
                File.WriteAllText(iPath, item.Value.Serialize());
            }
        }
    }
}
