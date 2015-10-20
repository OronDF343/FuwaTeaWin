using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace FuwaTea.Lib.FileAssociations
{
    public class RegistryClass
    {
        public const string DefaultIconKeyName = "DefaultIcon";
        public const string ShellKeyName = "shell";
        public const string CommandKeyName = "command";

        public RegistryKey RegistryBaseKey { get; }
        public string Description { get; set; }
        public string DefaultIconPath { get; set; }
        public string DefaultShellKeyName { get; set; }
        public List<ShellEntry> ShellEntries { get; } = new List<ShellEntry>();

        public string ClassName { get; }
        public RegistryClass(string className)
        {
            ClassName = className;
            RegistryBaseKey = Registry.ClassesRoot.CreateSubKey(className);
            ReadValues();
        }

        public ShellEntry FindOrCreateEntry(string key)
        {
            var entry = ShellEntries.FirstOrDefault(en => string.Equals(en.KeyName, key, StringComparison.OrdinalIgnoreCase));
            if (entry == default(ShellEntry))
            {
                entry = new ShellEntry(key);
                ShellEntries.Add(entry);
            }
            return entry;
        }

        public void Delete()
        {
            Registry.ClassesRoot.DeleteSubKeyTree(ClassName);
        }

        public void ReadValues()
        {
            Description = (string)RegistryBaseKey.GetValue("");
            DefaultIconPath = (string)RegistryBaseKey.CreateSubKey(DefaultIconKeyName)?.GetValue("");
            var shell = RegistryBaseKey.CreateSubKey(ShellKeyName);
            DefaultShellKeyName = (string)shell.GetValue("");
            var sh = from k in shell.GetSubKeyNames()
                     let sk = shell.CreateSubKey(k)
                     select new ShellEntry(k)
                            {
                                Description = (string)sk.GetValue(""),
                                Command = (string)sk.CreateSubKey(CommandKeyName)?.GetValue("")
                            };
            ShellEntries.AddRange(sh);
        }

        public void WriteValues()
        {
            RegistryBaseKey.SetValue("", Description, RegistryValueKind.String);
            RegistryBaseKey.CreateSubKey(DefaultIconKeyName).SetValue("", DefaultIconPath, RegistryValueKind.String);
            var shell = RegistryBaseKey.CreateSubKey(ShellKeyName);
            shell.SetValue("", DefaultShellKeyName, RegistryValueKind.String);
            var skNames = shell.GetSubKeyNames();
            var redundant = skNames.Where(s => !ShellEntries.Any(e => string.Equals(e.KeyName, s, StringComparison.OrdinalIgnoreCase)));
            var toAdd = ShellEntries.Where(e => !skNames.Any(s => string.Equals(s, e.KeyName, StringComparison.OrdinalIgnoreCase)));
            foreach (var s in redundant) shell.DeleteSubKeyTree(s);
            foreach (var entry in toAdd)
            {
                var k = shell.CreateSubKey(entry.KeyName);
                k.SetValue("", entry.Description, RegistryValueKind.String);
                k.CreateSubKey(CommandKeyName).SetValue("", entry.Command, RegistryValueKind.String);
            }
        }

        public class ShellEntry
        {
            public ShellEntry(string keyName) { KeyName = keyName; }
            public string KeyName { get; }
            public string Description { get; set; }
            public string Command { get; set; }
        }
    }
}
