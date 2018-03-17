using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using DryIoc;
using FuwaTea.Core;

namespace FuwaTea.Config
{
    public class ConfigManager : IConfigManager
    {
        private readonly Dictionary<string, Meta<IConfigPage, IConfigPageMetadata>> _pages;
        
        public ConfigManager([Import] IEnumerable<Meta<IConfigPage, IConfigPageMetadata>> pages)
        {
            _pages = pages.ToDictionary(l => l.Metadata.Key, l => l);
        }

        public void LoadAllConfigPages()
        {
            var ppath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppConstants.SanitizedName, ConfigConstants.ConfigDirName);
            var nppath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConstants.SanitizedName, ConfigConstants.ConfigDirName);
            if (!Directory.Exists(ppath)) Directory.CreateDirectory(ppath);
            if (!Directory.Exists(nppath)) Directory.CreateDirectory(nppath);
            foreach (var item in _pages)
            {
                var ipath = Path.Combine(item.Value.Metadata.IsPersistent ? ppath : nppath,
                                         item.Key + ConfigConstants.ConfigFileExtension);
                if (!File.Exists(ipath)) continue;
                item.Value.Value.Deserialize(File.ReadAllText(ipath));
            }
        }

        public void SaveAllConfigPages()
        {
            var ppath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppConstants.SanitizedName, ConfigConstants.ConfigDirName);
            var nppath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppConstants.SanitizedName, ConfigConstants.ConfigDirName);
            if (!Directory.Exists(ppath)) Directory.CreateDirectory(ppath);
            if (!Directory.Exists(nppath)) Directory.CreateDirectory(nppath);
            foreach (var item in _pages)
            {
                var ipath = Path.Combine(item.Value.Metadata.IsPersistent ? ppath : nppath,
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
