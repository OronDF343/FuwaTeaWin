using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace FuwaTea.Lib.FileAssociations
{
    public class AppCapabilities
    {
        protected const string AppNameValueName = "ApplicationName";
        protected const string AppDescriptionValueName = "ApplicationDescription";
        protected const string FileAssociationsKeyName = "FileAssociations";
        protected const string UrlAssociationsKeyName = "UrlAssociations";
        protected const string MimeAssociationsKeyName = "MimeAssociations";

        public RegistryKey RegistryBaseKey { get; }
        public string AppName { get; set; }
        public string AppDescription { get; set; }
        public List<AssociationEntry> FileAssociationEntries { get; } = new List<AssociationEntry>();
        public List<AssociationEntry> UrlAssociationEntries { get; } = new List<AssociationEntry>();
        public List<AssociationEntry> MimeAssociationEntries { get; } = new List<AssociationEntry>();

        public AppCapabilities(string clientType, string keyName)
        {
            RegistryBaseKey = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\Clients\{clientType}\{keyName}\Capabilities");
            ReadValues();
        }

        public AppCapabilities(string keyPath)
        {
            RegistryBaseKey = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\{keyPath}\Capabilities");
            ReadValues();
        }

        public void ReadValues()
        {
            AppName = (string)RegistryBaseKey.GetValue(AppNameValueName);
            AppDescription = (string)RegistryBaseKey.GetValue(AppDescriptionValueName);
            FileAssociationEntries.AddRange(GetAllAssociations(FileAssociationsKeyName));
            UrlAssociationEntries.AddRange(GetAllAssociations(UrlAssociationsKeyName));
            MimeAssociationEntries.AddRange(GetAllAssociations(MimeAssociationsKeyName));
        }

        protected IEnumerable<AssociationEntry> GetAllAssociations(string subKey)
        {
            var assoc = RegistryBaseKey.CreateSubKey(subKey);
            if (assoc == null) throw new NullReferenceException("Failed to open key!"); // TODO: RegistryException
            var exts = assoc.GetValueNames();
            return exts.Select(ext => new AssociationEntry { Type = ext, ClassName = (string)assoc.GetValue(ext) });
        }

        public void WriteValues()
        {
            RegistryBaseKey.SetValue(AppNameValueName, AppName, RegistryValueKind.String);
            RegistryBaseKey.SetValue(AppDescriptionValueName, AppDescription, RegistryValueKind.String);
            WriteAllAssociations(FileAssociationsKeyName, FileAssociationEntries);
            WriteAllAssociations(UrlAssociationsKeyName, UrlAssociationEntries);
            WriteAllAssociations(MimeAssociationsKeyName, MimeAssociationEntries);
        }

        protected void WriteAllAssociations(string subKey, ICollection<AssociationEntry> assocs)
        {
            var key = RegistryBaseKey.CreateSubKey(subKey);
            if (key == null) throw new NullReferenceException("Failed to open key!"); // TODO: RegistryException
            var names = key.GetValueNames();
            var redundant = names.Except(assocs.Select(s => s.Type), StringComparer.OrdinalIgnoreCase);
            var toAdd = assocs.Where(s => !names.Any(n => string.Equals(n, s.Type, StringComparison.OrdinalIgnoreCase)));
            foreach (var k in redundant) key.DeleteValue(k);
            foreach (var k in toAdd) key.SetValue(k.Type, k.ClassName); 
        }

        public class AssociationEntry
        {
            public string Type;
            public string ClassName;
        }
    }
}
