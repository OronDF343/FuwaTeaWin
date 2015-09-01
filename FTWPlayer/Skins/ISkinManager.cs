using System.Collections.ObjectModel;
using System.Windows;
using FuwaTea.Lib.Exceptions;

namespace FTWPlayer.Skins
{
    public interface ISkinManager : IUIPart
    {
        ObservableCollection<ResourceDictionary> LoadedSkins { get; }
        void LoadAllSkins(ErrorCallback ec);
        void LoadSkinFromXamlFile(string path);
        void LoadSkinFromBaml(string dll);
    }
}
