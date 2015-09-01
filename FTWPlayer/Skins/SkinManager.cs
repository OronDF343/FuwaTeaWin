using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Baml2006;
using System.Xaml;
using FuwaTea.Lib;
using FuwaTea.Lib.Exceptions;
using log4net;
using XamlReader = System.Windows.Markup.XamlReader;

namespace FTWPlayer.Skins
{
    [UIPart("Skin Manager")]
    public class SkinManager : ISkinManager
    {
        // TODO: Explain all this class even more. Possibly overcomplicated
        public ObservableCollection<ResourceDictionary> LoadedSkins { get; } =
            new ObservableCollection<ResourceDictionary> {DefaultSkin};

        public Dictionary<string, string> IdsToFileNames { get; } = new Dictionary<string, string>
        {
            {"Default", "Default"}
        };

        private static readonly ResourceDictionary DefaultSkin
            = new ResourceDictionary {Source = new Uri("pack://application:,,,/Skins/MainSkin.xaml")};

        public void LoadAllSkins(ErrorCallback ec)
        { 
            // Installed dll skins:
            var skDir = Assembly.GetEntryAssembly().GetSpecificPath(false, "skins", true);
            var dllSkins =
                Directory.EnumerateFiles(skDir, "*.dll")
                         .Concat(Directory.EnumerateDirectories(skDir)
                                          .SelectMany(d => Directory.EnumerateFiles(d, "*.dll")));
            foreach (var skin in dllSkins.Where(d => !IdsToFileNames.ContainsValue(d)))
                try { LoadSkinsFromBaml(skin); }
                catch (Exception e) { ec(new Exception("Error loading BAML from file: " + skin, e)); }
            // Installed xaml skins:
            var xamlSkins =
                Directory.EnumerateFiles(skDir, "*.xaml")
                         .Concat(Directory.EnumerateDirectories(skDir)
                                          .SelectMany(d => Directory.EnumerateFiles(d, "*.xaml")));
            // User xaml skins:
            var uSkDir = Assembly.GetEntryAssembly().GetSpecificPath(true, "skins", true);
            var userXamlSkins =
                Directory.EnumerateFiles(uSkDir, "*.xaml")
                         .Concat(Directory.EnumerateDirectories(uSkDir)
                                          .SelectMany(d => Directory.EnumerateFiles(d, "*.xaml")));
            xamlSkins = xamlSkins.Concat(userXamlSkins);
            foreach (var skin in xamlSkins.Where(s => !IdsToFileNames.ContainsValue(s)))
                try { if (!IdsToFileNames.ContainsValue(ShortenPath(skin))) LoadSkinFromXamlFile(skin); }
                catch (Exception e) { ec(new Exception("Error loading XAML from file: " + skin, e)); }
        }

        public IEnumerable<ResourceDictionary> LoadSkinChain(IEnumerable<string> files)
        {
            return files.Select(file =>
                                IdsToFileNames.ContainsValue(file)
                                    ? GetSkin(IdsToFileNames.Keys.First(k => IdsToFileNames[k] == file))
                                    : file.StartsWith("pack://", StringComparison.OrdinalIgnoreCase)
                                        ? LoadSkinFromXamlUri(file)
                                        : file.Contains('|')
                                           ? LoadSkinFromBaml(ExpandPath(file))
                                           : LoadSkinFromXamlFile(ExpandPath(file)));
        }

        public ResourceDictionary LoadSkinFromXamlFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var obj = XamlReader.Load(fs);
                if (!(obj is ResourceDictionary))
                {
                    LogManager.GetLogger(GetType()).Warn("XAML file is not a skin: " + path);
                    return null;
                }
                var dict = (ResourceDictionary)obj;
                if (!dict.Contains("SkinIdentifier"))
                {
                    LogManager.GetLogger(GetType()).Warn("XAML file is missing Skin Identifier: " + path);
                    return null;
                }
                IdsToFileNames.Add(dict.GetIdentifier().Id, ShortenPath(path));
                LoadedSkins.Add(dict);
                return dict;
            }
        }

        public ResourceDictionary LoadSkinFromXamlUri(string path)
        {
            return new ResourceDictionary {Source = new Uri(path)};
        }

        public void LoadSkinsFromBaml(string dll)
        {
            var a = Assembly.LoadFrom(dll);
            var ms = a.GetManifestResourceNames().Where(m => m.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase)).ToList();
            var found = false;
            foreach (var r in ms.Select(m => a.GetManifestResourceStream(m)))
            {
                var reader = new Baml2006Reader(r);
                var writer = new XamlObjectWriter(reader.SchemaContext);
                while (reader.Read()) { writer.WriteNode(reader); }
                var obj = writer.Result;
                if (!(obj is ResourceDictionary)) continue;
                var dict = (ResourceDictionary)obj;
                if (!dict.Contains("SkinIdentifier")) continue;
                found = true;
                if (IdsToFileNames.ContainsKey(dict.GetIdentifier().Id)) continue;
                IdsToFileNames.Add(dict.GetIdentifier().Id, ShortenPath(dll + "|" + r));
                LoadedSkins.Add(dict);
            }
            if (!found) LogManager.GetLogger(GetType()).Warn("DLL file contains no skins: " + dll);
        }

        public ResourceDictionary LoadSkinFromBaml(string dllResource)
        {
            var split = dllResource.Split('|');
            split[0] += ".dll";
            var a = Assembly.LoadFrom(split[0]);
            var r = a.GetManifestResourceStream(split[1]);
            if (r == null) throw new KeyNotFoundException($"{dllResource} not found!");
            var reader = new Baml2006Reader(r);
            var writer = new XamlObjectWriter(reader.SchemaContext);
            while (reader.Read()) { writer.WriteNode(reader); }
            var obj = writer.Result;
            if (!(obj is ResourceDictionary)) throw new InvalidDataException($"{dllResource} inavlid resource type!");
            var dict = (ResourceDictionary)obj;
            if (!dict.Contains("SkinIdentifier")) throw new KeyNotFoundException($"{dllResource} missing Skin Identifier!");
            IdsToFileNames.Add(dict.GetIdentifier().Id, ShortenPath(dllResource));
            LoadedSkins.Add(dict);
            return dict;
        }

        public ResourceDictionary GetSkin(string id)
        {
            return LoadedSkins.First(d => d.GetIdentifier().Id == id);
        }

        public List<ResourceDictionary> CreateSimpleSkinChain(string id)
        {
            var chain = new List<ResourceDictionary>();
            while (id != null)
            {
                var s = GetSkin(id);
                chain.Insert(0, s);
                id = s.GetIdentifier().Id;
            }
            var t = VerifySkinChain(chain);
            if (!string.IsNullOrWhiteSpace(t))
                throw new InvalidOperationException("Invalid skin dependency, report to skin author(s): " + t);
            return chain;
        }

        public string VerifySkinChain(IEnumerable<ResourceDictionary> chain)
        {
            // Base, Addon, Color skins can each depend on an Addon or Base skin.
            // Base can only appear right after the skin it depends on.
            // Multiple Colors or Addons are allowed.
            // See ResourceDictionaryIdentifier.cs for more info.
            if (LogManager.GetLogger(GetType()).IsDebugEnabled)
                LogManager.GetLogger(GetType()).Debug("Verifying skin chain:\n\t" + string.Join("\n\t", chain));
            var lastType = ResourceDictionaryType.Standalone;
            string lastBase = null, lastNonColor = null, lastId = null, ret = null;
            foreach (var i in chain.Select(rd => rd.GetIdentifier()))
            {
                // This check applies to the first skin
                if (lastId == null
                    && (i.SkinType == ResourceDictionaryType.Color || i.SkinType == ResourceDictionaryType.Addon))
                    ret = "Missing dependency - only Base or Standalone can be first in the chain.";
                // Make sure standalone is standalone
                if (lastId != null && lastType == ResourceDictionaryType.Standalone)
                    ret = lastId + " is Standalone, can't load other skins after a standalone skin.";
                // Check the order makes sense, with the exception that Base can follow Addon
                if (i.SkinType < lastType
                    && (i.SkinType != ResourceDictionaryType.Base || lastType != ResourceDictionaryType.Addon))
                    ret = "Bad skin order - " + lastId + " was "
                           + Enum.GetName(typeof(ResourceDictionaryType), lastType)
                           + ", " + i.Id + " is "
                           + Enum.GetName(typeof(ResourceDictionaryType), i.SkinType) + ".";
                // Check that the dependency is correct (for Addon and Color)
                if ((i.Parent != lastBase || i.Parent != lastNonColor)
                    && (i.SkinType == ResourceDictionaryType.Color || i.SkinType == ResourceDictionaryType.Addon))
                    ret = "Incorrect dependency - " + i.Id + " depends on " + i.Parent + ", not " + lastBase
                          + (lastBase != lastNonColor ? " or " + lastNonColor : "") + ".";
                // Check that the dependency is correct (for Base)
                if (i.Parent != lastId && i.SkinType == ResourceDictionaryType.Base)
                    ret = "Incorrect dependency - " + i.Id + " depends on " + i.Parent + ", not " + lastId + ".";

                if (ret != null)
                {
                    LogManager.GetLogger(GetType()).Warn(ret);
                    return ret;
                }
                lastType = i.SkinType;
                lastId = i.Id;
                if (lastType == ResourceDictionaryType.Base) lastBase = lastId;
                if (lastType != ResourceDictionaryType.Color) lastNonColor = lastId;
            }
            LogManager.GetLogger(GetType()).Debug("Skin chain was OK.");
            return null;
        }

        public IEnumerable<ResourceDictionary> GetAvailableChildSkins(string id)
        {
            return LoadedSkins.Where(rd => rd.GetIdentifier().Parent == id);
        }

        public IEnumerable<ResourceDictionary> GetAvailableChildSkins(IEnumerable<ResourceDictionary> chain)
        {
            chain = chain.ToList();
            if (chain.First().GetIdentifier().SkinType == ResourceDictionaryType.Standalone)
                return new List<ResourceDictionary>();
            // !color
            var ch = chain.Where(r => r.GetIdentifier().SkinType != ResourceDictionaryType.Color).ToList();
            // last!color
            var la = ch.Last().GetIdentifier();
            // last
            var li = chain.Last().GetIdentifier();
            // get children of last!color
            var l = GetAvailableChildSkins(la.Id);
            // Addon and Color allow multiple dependency, so if last!color is not base,
            // find last base and add children which don't have to directly follow (!base):
            if (la.SkinType != ResourceDictionaryType.Base)
                l = l.Union(GetAvailableChildSkins(ch.Last(r => r.GetIdentifier().SkinType == ResourceDictionaryType.Base).GetIdentifier().Id)
                                .Where(r => r.GetIdentifier().SkinType != ResourceDictionaryType.Base));
            // If last is color, only color can follow
            if (li.SkinType == ResourceDictionaryType.Color)
                l = l.Where(r => r.GetIdentifier().SkinType == ResourceDictionaryType.Color);
            return l;
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
        public static ResourceDictionaryIdentifier GetIdentifier(this ResourceDictionary rd)
        {
            return ((ResourceDictionaryIdentifier)rd["SkinIdentifier"]);
        }
    }
}
