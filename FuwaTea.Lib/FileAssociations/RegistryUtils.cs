using System.Collections.Generic;
using System.Linq;

namespace FuwaTea.Lib.FileAssociations
{
    public static class RegistryUtils
    {
        public static string GetSuffix(this RegistryClass rc)
        {
            return rc.ClassName.Substring(rc.ClassName.LastIndexOf('.') + 1);
        }

        public static void RemoveFuwaClasses(string prod)
        {
            var dir = new RegistryClass("Directory");
            dir.ShellEntries.RemoveAll(se => se.KeyName.StartsWith(prod + "."));
            new AppRegistryClasses(prod).Clear();
            // No need to delete capabilities since uninstaller takes care of that
        }

        public static void UpdateFuwaAssociations(Dictionary<string, string> supported, string loc, 
                                                  string prod, string playWith, string addTo, string desc)
        {
            // Classes
            var dir = new RegistryClass("Directory");
            var play = dir.FindOrCreateEntry($"{prod}.Play");
            play.Command = $"\"{loc}\" \"%1\"";
            play.Description = playWith; //string.Format(LocalizationProvider.GetLocalizedValue<string>("PlayWithFormatString"), title);
            var add = dir.FindOrCreateEntry($"{prod}.AddToPlaylist");
            add.Command = $"\"{loc}\" \"%1\" --add";
            add.Description = addTo; //string.Format(LocalizationProvider.GetLocalizedValue<string>("AddToPlaylistFormatString"), title);
            dir.WriteValues();
            var arc = new AppRegistryClasses(prod);
            foreach (var s in arc.Keys.Except(supported.Keys))
                arc.RemoveKey(s);
            foreach (var s in supported)
            {
                var c = arc.GetKey(s.Key);
                if (c == default(RegistryClass))
                {
                    c = new RegistryClass(prod + "." + s.Key)
                    {
                        Description = s.Value,
                        DefaultIconPath = $"\"{loc}\",0",
                        DefaultShellKeyName = "Play"
                    };
                }
                var playF = c.FindOrCreateEntry("Play");
                playF.Command = $"\"{loc}\" \"%1\"";
                playF.Description = playWith; //string.Format(LocalizationProvider.GetLocalizedValue<string>("PlayWithFormatString"), title);
                var addF = c.FindOrCreateEntry("AddToPlaylist");
                addF.Command = $"\"{loc}\" \"%1\" --add";
                addF.Description = addTo; //string.Format(LocalizationProvider.GetLocalizedValue<string>("AddToPlaylistFormatString"), title);
                c.WriteValues();
            }
            // Capabilities
            var capabillities = new AppCapabilities("Media", prod)
            {
                AppDescription = desc
            };
            // TODO: Simplify
            foreach (var s in capabillities.FileAssociationEntries.Select(s => s.Type.Substring(1)).Except(supported.Keys))
                capabillities.FileAssociationEntries.Remove(capabillities.FileAssociationEntries.First(ae => ae.Type.Substring(1) == s));
            foreach (var s in supported.Keys.Where(s => capabillities.FileAssociationEntries.All(ae => ae.Type.Substring(1) != s)))
                capabillities.FileAssociationEntries.Add(new AppCapabilities.AssociationEntry
                {
                    Type = "." + s,
                    ClassName = prod + "." + s
                });
            capabillities.WriteValues();
        }
    }
}
