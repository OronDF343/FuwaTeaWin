using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Baml2006;
using System.Windows.Markup;
using System.Xaml;
using FuwaTea.Lib;
using JetBrains.Annotations;
using log4net;
using Sage.Lib;
using XamlReader = System.Windows.Markup.XamlReader;

namespace FTWPlayer.Skins
{
    //[UIPart("Skin Manager")]
    public class SkinManager : ISkinManager
    {
        public ObservableCollection<SkinPackage> LoadedSkins { get; } = new ObservableCollection<SkinPackage>();

        private const string DefaultSkin = "pack://application:,,,/Skins/Default";
        
        public SkinPackage GetLoadedSkin(string source)
        {
            return LoadedSkins.FirstOrDefault(s => s.Path == source);
        }

        /// <exception cref="DirectoryNotFoundException">Skins directory is invalid, such as referring to an unmapped drive. </exception>
        /// <exception cref="IOException">Skins directory is a file name.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="SkinLoadException">An error occured while loading a skin</exception>
        public void LoadAllSkins(Action<Exception> ec)
        {
            // Built-in (explicit):
            LoadSkinFromPackUri(DefaultSkin);
            LoadSkinFromPackUri("pack://application:,,,/Skins/Glacier");
            // Installed dll skins:
            var skDir = Assembly.GetEntryAssembly().GetSpecificPath(false, "skins", true);
            var dllSkins =
                Directory.EnumerateFiles(skDir, "*.dll")
                         .Concat(Directory.EnumerateDirectories(skDir)
                                          .SelectMany(d => Directory.EnumerateFiles(d, "*.dll")));
            foreach (var skin in dllSkins)
                try { LoadSkinFromBaml(skin); }
                catch (Exception e) { ec(new Exception("Error loading BAML from file: " + skin, e)); }
            // Installed xaml skins:
            var xamlSkins = Directory.EnumerateDirectories(skDir);
            // User xaml skins:
            var uSkDir = Assembly.GetEntryAssembly().GetSpecificPath(true, "skins", true);
            var userXamlSkins = Directory.EnumerateDirectories(uSkDir);
            xamlSkins = xamlSkins.Concat(userXamlSkins);
            foreach (var dir in xamlSkins)
                try { LoadSkinFromXamlFiles(dir); }
                catch (Exception e) { ec(new Exception("Error loading XAML from directory: " + dir, e)); }
        }

        /// <summary>
        /// Loads a skin
        /// </summary>
        /// <remarks>Only this function takes shortened paths! It also adds missing ResourceDictionaries from the default skin</remarks>
        /// <param name="source">The location of the skin as a shortened path</param>
        /// <param name="children">(used internally in the recursion) all the children of the current skin</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> is <see langword="null" />.</exception>
        /// <exception cref="System.InvalidOperationException">There is a cyclic dependency between skins</exception>
        /// <exception cref="SkinLoadException">An error occured while loading the skin</exception>
        public SkinPackage LoadSkin(string source, HashSet<string> children = null)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(nameof(source), "Attempted to load skin from null source!");
            if (children?.Contains(source.ToLowerInvariant()) ?? false)
                throw new InvalidOperationException($"Detected skin cyclic dependency! Path=\"{source}\" is referenced more than once!");
            var f = GetLoadedSkin(source)
                    ?? (source.StartsWith(PackUriStart, StringComparison.OrdinalIgnoreCase)
                                                                              ? LoadSkinFromPackUri(source)
                                                                              : source.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                                                                                    ? LoadSkinFromBaml(ExpandPath(source))
                                                                                    : LoadSkinFromXamlFiles(ExpandPath(source)));
            if (source == DefaultSkin) return f;
            SkinPackage parent;
            if (!f.HasIdentifier() || string.IsNullOrWhiteSpace(f.GetIdentifier()?.Parent)) parent = LoadFallbackSkin();
            else
            {
                var ch = children ?? new HashSet<string>();
                ch.Add(source);
                // ReSharper disable once AssignNullToNotNullAttribute
                parent = LoadSkin(f.GetIdentifier()?.Parent, ch);
            }
            foreach (var part in parent.SkinParts.Where(part => !f.SkinParts.ContainsKey(part.Key)))
                f.SkinParts.Add(part.Key, part.Value);
            return f;
        }

        /// <exception cref="SkinLoadException">An error occured while loading the skin</exception>
        public SkinPackage LoadFallbackSkin()
        {
            return LoadSkin(DefaultSkin);
        }

        /// <exception cref="SkinLoadException">An error occured while loading the skin.</exception>
        public SkinPackage LoadSkinFromXamlFiles(string dir)
        {
            var shortDir = ShortenPath(dir);
            var f = GetLoadedSkin(shortDir);
            if (f != null) return f;

            var found = false;
            var allDicts = new Dictionary<string, ResourceDictionary>();
            try
            {
                foreach (var path in Directory.EnumerateFiles(dir, "*.xaml"))
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var obj = XamlReader.Load(fs, new ParserContext { BaseUri = new Uri(path) });
                        if (!(obj is ResourceDictionary))
                        {
                            LogManager.GetLogger(GetType()).Warn("XAML file is not a skin: " + path);
                            continue;
                        }
                        if (path != null) allDicts.Add(Path.GetFileNameWithoutExtension(path).ToLowerInvariant(), (ResourceDictionary)obj);
                        found = true;
                    }
                }
            }
            catch (Exception e)
            {
                throw new SkinLoadException("An error occured opening the speified directory or one of the files in it", e);
            }
            if (!found)
            {
                throw new SkinLoadException("The speified directory contains no valid XAML skin files: " + dir);
            }
            if (!allDicts.HasIdentifier()) LogManager.GetLogger(GetType()).Warn("XAML files are missing Skin Identifier: " + dir);
            var pkg = new SkinPackage(shortDir, allDicts);
            LoadedSkins.Add(pkg);
            return pkg;
        }

        private const string PackUriStart = "pack://application:,,,/";

        /// <exception cref="SkinLoadException">URI format is incorrect -or- the referenced assembly could not be loaded</exception>
        public SkinPackage LoadSkinFromPackUri(string uri)
        {
            if (!uri.StartsWith(PackUriStart)) throw new SkinLoadException("Missing a valid application pack URI prefix!");
            var f = GetLoadedSkin(uri);
            if (f != null) return f;

            var shortUri = uri.Substring(PackUriStart.Length);
            Assembly a;
            if (uri.Contains(";"))
            {
                try
                {
                    var ai = uri.Substring(0, uri.IndexOf("/", StringComparison.Ordinal)).Split(';');
                    if (ai.Length < 2 || !ai[ai.Length - 1].Equals("component", StringComparison.OrdinalIgnoreCase))
                        throw new FormatException("Missing \"component\" in referenced assembly definition!");
                    var an = new AssemblyName { Name = ai[0] };
                    var hasVersion = ai[1].Contains(".");
                    if (hasVersion) an.Version = Version.Parse(ai[1]);
                    if (hasVersion && ai.Length > 3) an.SetPublicKey(StringUtils.UnHexify(ai[2]));
                    else if (!hasVersion && ai.Length > 2) an.SetPublicKey(StringUtils.UnHexify(ai[1]));
                    a = Assembly.Load(an);
                }
                catch (Exception e)
                {
                    throw new SkinLoadException("An error occured while loading the referenced assembly from the pack URI!", e);
                }
            }
            else a = Assembly.GetCallingAssembly();
            var allDicts = GetResourcesFromAssembly(a, s => s.StartsWith(shortUri, StringComparison.OrdinalIgnoreCase));

            if (!allDicts.HasIdentifier()) LogManager.GetLogger(GetType()).Warn("Assembly \"" + a.GetName().Name + "\" is missing Skin Identifier in path: " + shortUri);
            var pkg = new SkinPackage(uri, allDicts);
            LoadedSkins.Add(pkg);
            return pkg;
        }

        /// <exception cref="SkinLoadException">An error occured while loading the resources from the <paramref name="dll"/> file</exception>
        public SkinPackage LoadSkinFromBaml(string dll)
        {
            var shortDll = ShortenPath(dll);
            var f = GetLoadedSkin(shortDll);
            if (f != null) return f;
            Dictionary<string, ResourceDictionary> allDicts;

            try { allDicts = GetResourcesFromAssembly(Assembly.LoadFrom(dll), s => true); }
            catch (Exception e)
            {
                throw new SkinLoadException("An error occured while loading the resources from the DLL file", e);
            }

            if (!allDicts.HasIdentifier()) LogManager.GetLogger(GetType()).Warn("DLL file is missing Skin Identifier: " + dll);
            var pkg = new SkinPackage(shortDll, allDicts);
            LoadedSkins.Add(pkg);
            return pkg;
        }
        
        private Dictionary<string, ResourceDictionary> GetResourcesFromAssembly(Assembly a, Func<string, bool> pathFilter)
        {
            var found = false;
            var allDicts = new Dictionary<string, ResourceDictionary>();
            var stream = a.GetManifestResourceStream(a.GetName().Name + ".g.resources");
            if (stream == null)
            {
                throw new InvalidOperationException("Assembly contains no visible resources: " + a.GetName().Name);
            }
            using (var rr = new ResourceReader(stream))
            {
                foreach (var m in rr.Cast<DictionaryEntry>()
                                    .Where(e => ((string)e.Key).EndsWith(".baml", StringComparison.OrdinalIgnoreCase))
                                    .Where(e => pathFilter((string)e.Key)))
                {
                    using (var r = (Stream)m.Value)
                    {
                        if (r == null) continue;
                        var reader = new Baml2006Reader(r, new XamlReaderSettings { LocalAssembly = a, BaseUri = new Uri(PackUriStart + (string)m.Key) });
                        var writer = new XamlObjectWriter(reader.SchemaContext);
                        while (reader.Read()) { writer.WriteNode(reader); }
                        var obj = writer.Result;
                        if (!(obj is ResourceDictionary)) continue;
                        var k = ((string)m.Key).Substring(((string)m.Key).LastIndexOf('/') + 1);
                        allDicts.Add(k.Substring(0, k.Length - 5).ToLowerInvariant(), (ResourceDictionary)obj);
                        found = true;
                    }
                }
            }
            if (!found) throw new SkinLoadException("Assembly contains no valid skin files: " + a.GetName().Name);
            return allDicts;
        }
        
        public string ExpandPath(string path)
        {
            return path.Replace("%app%", Assembly.GetEntryAssembly().GetSpecificPath(false, "skins", false))
                       .Replace("%user%", Assembly.GetEntryAssembly().GetSpecificPath(true, "skins", false));
        }
        
        public string ShortenPath(string path)
        {
            return path.Replace(Assembly.GetEntryAssembly().GetSpecificPath(false, "skins", false), "%app%")
                       .Replace(Assembly.GetEntryAssembly().GetSpecificPath(true, "skins", false), "%user%");
        }
    }

    public static class ExtensionMethods
    {
        [CanBeNull]
        public static ResourceDictionaryIdentifier GetIdentifier(this SkinPackage rd)
        {
            return rd.HasIdentifier() ? rd.SkinParts["commonstyle"]["SkinIdentifier"] as ResourceDictionaryIdentifier : null;
        }

        [CanBeNull]
        public static ResourceDictionaryIdentifier GetIdentifier(this Dictionary<string, ResourceDictionary> rd)
        {
            return rd.HasIdentifier() ? rd["commonstyle"]["SkinIdentifier"] as ResourceDictionaryIdentifier : null;
        }

        public static bool HasIdentifier(this SkinPackage rd)
        {
            return rd.SkinParts.HasIdentifier();
        }

        public static bool HasIdentifier(this Dictionary<string, ResourceDictionary> rd)
        {
            return rd.ContainsKey("commonstyle") && rd["commonstyle"].Contains("SkinIdentifier");
        }
    }
}
