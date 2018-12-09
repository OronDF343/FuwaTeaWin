using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using DryIoc;

namespace FuwaTea.Config
{
    public class ConfigManager : IConfigManager
    {
        private readonly Dictionary<string, Meta<IConfigPage, IConfigPageMetadata>> _pages;
        private readonly string _persistent;
        private readonly string _nonPersistent;

        public ConfigManager([Import] IEnumerable<Meta<IConfigPage, IConfigPageMetadata>> pages,
                             [Import(ContractName = ConfigConstants.PersistentConfigDirKey)] string p,
                             [Import(ContractName = ConfigConstants.NonPersistentConfigDirKey)] string np)
        {
            _pages = pages.ToDictionary(l => l.Metadata.Key, l => l);
            _persistent = p;
            _nonPersistent = np;
        }

        public void LoadAllConfigPages()
        {
            if (!Directory.Exists(_persistent)) Directory.CreateDirectory(_persistent);
            if (!Directory.Exists(_nonPersistent)) Directory.CreateDirectory(_nonPersistent);
            foreach (var item in _pages)
            {
                var ipath = Path.Combine(item.Value.Metadata.IsPersistent ? _persistent : _nonPersistent,
                                         item.Key + ConfigConstants.ConfigFileExtension);
                if (!File.Exists(ipath)) continue;
                item.Value.Value.Deserialize(File.ReadAllText(ipath));
            }
        }

        public void SaveAllConfigPages()
        {
            if (!Directory.Exists(_persistent)) Directory.CreateDirectory(_persistent);
            if (!Directory.Exists(_nonPersistent)) Directory.CreateDirectory(_nonPersistent);
            foreach (var item in _pages)
            {
                var ipath = Path.Combine(item.Value.Metadata.IsPersistent ? _persistent : _nonPersistent,
                                         item.Key + ConfigConstants.ConfigFileExtension);
                File.WriteAllText(ipath, item.Value.Value.Serialize());
            }
        }

        public IConfigPage GetPage(string key)
        {
            return _pages[key].Value;
        }

        public IConfigPageMetadata GetPageMetadata(string key)
        {
            return _pages[key].Metadata;
        }

        public IConfigPage this[string key] => GetPage(key);
    }
}
