using System.Collections.Generic;
using System.Windows;

namespace FTWPlayer.Skins
{
    public class SkinPackage
    {
        public SkinPackage(string path, Dictionary<string, ResourceDictionary> skinParts)
        {
            Path = path;
            SkinParts = skinParts;
        }

        public string Path { get; }
        public Dictionary<string, ResourceDictionary> SkinParts { get; }
    }
}
