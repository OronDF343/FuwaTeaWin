using System;
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
        public ObservableCollection<ResourceDictionary> LoadedSkins { get; } = new ObservableCollection<ResourceDictionary>();

        public void LoadAllSkins(ErrorCallback ec)
        { 
            // Installed dll skins:
            var skDir = Assembly.GetEntryAssembly().GetSpecificPath(false, "skins", true);
            var dllSkins =
                Directory.EnumerateFiles(skDir, "*.dll")
                         .Concat(Directory.EnumerateDirectories(skDir)
                                          .SelectMany(d => Directory.EnumerateFiles(d, "*.dll")));
            foreach (var skin in dllSkins)
                try { LoadSkinFromBaml(skin); }
                catch (Exception e) { ec(new Exception("Error loading BAML! File: " + skin, e)); }
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
            foreach (var skin in xamlSkins)
                try { LoadSkinFromXamlFile(skin); }
                catch (Exception e) { ec(new Exception("Error loading XAML! File: " + skin, e)); }
        }

        public void LoadSkinFromXamlFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var obj = XamlReader.Load(fs);
                if (!(obj is ResourceDictionary))
                {
                    LogManager.GetLogger(GetType()).Warn("XAML file is not a skin: " + path);
                    return;
                }
                var dict = (ResourceDictionary)obj;
                if (!dict.Contains("SkinIdentifier"))
                {
                    LogManager.GetLogger(GetType()).Warn("XAML file is missing Skin Identifier: " + path);
                    return;
                }
                LoadedSkins.Add(dict);
            }
        }

        public void LoadSkinFromBaml(string dll)
        {
            var a = Assembly.LoadFrom(dll);
            var ms = a.GetManifestResourceNames().Where(m => m.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase)).ToList();
            if (!ms.Any())
            {
                LogManager.GetLogger(GetType()).Warn("DLL file contains no skins: " + dll);
                return;
            }
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
                LoadedSkins.Add(dict);
            }
            if (!found) LogManager.GetLogger(GetType()).Warn("DLL file contains no skins: " + dll);
        }
    }
}
