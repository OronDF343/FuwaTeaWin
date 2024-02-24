using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ModularFramework.Extensions
{
    public delegate bool ExtensionLoadCallback(ExtensionAttribute arg1);

    public static class ExtensionUtils
    {
        public static List<ExtensionAttribute> Whitelist { get; } = new List<ExtensionAttribute>();
        public static Dictionary<Assembly, ExtensionAttribute> LoadedExtensions { get; } = new Dictionary<Assembly, ExtensionAttribute>();  

        public static ExtensionLoadCallback ExtensionLoadCallback { get; set; }

        public static void LoadExtensions(string folder, Func<string, bool> fileSelector, ErrorCallback ec)
        {
            var exp = FolderUtils.SelectFiles(folder, fileSelector)
                                 .Select(f => AssemblyLoader.LoadAssembly(f, ec))
                                 .Where(a => a.IsDefined(typeof(ExtensionAttribute), false));
                                 
            foreach (var e in exp)
            {
                var attr = e.GetAttribute<ExtensionAttribute>();
                if (!Whitelist.Contains(attr))
                {
                    if (ExtensionLoadCallback == null || !ExtensionLoadCallback(attr)) continue;
                    Whitelist.Add(attr);
                }
                ModuleFactory.LoadAssembly(e, ec, true);
                LoadedExtensions.Add(e, attr);
            }
        }

        public static ExtensionAttribute GetSourceExtensionInfo([NotNull] Type t)
        {
            ExtensionAttribute a;
            return LoadedExtensions.TryGetValue(t.Assembly, out a) ? a : null;
        }
    }
}
