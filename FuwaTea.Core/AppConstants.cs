using System;
using System.IO;

namespace FuwaTea.Core
{
    public static class AppConstants
    {
        public const string PublisherName = "OronDF343";
        public const string ProductName = "Sage";

        public const string LogsDirName = "Logs";
        public const string ConfigDirName = "Config";

        public static class Arguments
        {
            public const string UpdateFileAssociations = "--update-file-associations";
            public const string DeleteFileAssociations = "--delete-file-associations";
            public const string FileAssociationsUi = "--file-associations-ui";
            public const string ShouldBeAdmin = "--admin";
            public const string SetLanguage = "--set-language";
        }
        
        public static string MakeAppPath(Environment.SpecialFolder sf, string dirName)
        {
            return Path.Combine(Environment.GetFolderPath(sf), AppConstants.ProductName, dirName);
        }
    }
}
