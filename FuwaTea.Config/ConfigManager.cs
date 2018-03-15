using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Jil;

namespace FuwaTea.Config
{
    public class ConfigManager : IConfigManager
    {
        private Dictionary<string, Lazy<IConfigPage, IConfigPageMetadata>> _pages;
        
        public ConfigManager([ImportMany] IEnumerable<Lazy<IConfigPage, IConfigPageMetadata>> pages)
        {
            _pages = pages.ToDictionary(l => l.Metadata.Key, l => l);
        }

        public void LoadAllConfigPages()
        {
            var ppath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ConfigConstants.ConfigDirName);
            var nppath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigConstants.ConfigDirName);
            foreach (var item in _pages)
            {
                var ipath = Path.Combine(item.Value.Metadata.IsPersistent ? ppath : nppath,
                                         item.Key + ConfigConstants.ConfigFileExtension);
                DeserializePage(item.Value.Value, File.ReadAllText(ipath));
            }
        }

        public void SaveAllConfigPages()
        {
            var ppath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ConfigConstants.ConfigDirName);
            var nppath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigConstants.ConfigDirName);
            foreach (var item in _pages)
            {
                var ipath = Path.Combine(item.Value.Metadata.IsPersistent ? ppath : nppath,
                                         item.Key + ConfigConstants.ConfigFileExtension);
                File.WriteAllText(ipath, SerializePage(item.Value.Value));
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

        public string SerializePage<T>(T page) where T : IConfigPage
        {
            return JSON.SerializeDynamic(page);
        }

        public T DeserializePage<T>(T page, string data) where T : IConfigPage
        {
            return JSON.Deserialize<T>(data);
        }

        public string SerializePage(IConfigPage page)
        {
            return JSON.SerializeDynamic(page);
        }

        public IConfigPage DeserializePage(IConfigPage page, string data)
        {
            return (IConfigPage)JSON.DeserializeDynamic(data);
        }
    }
}
