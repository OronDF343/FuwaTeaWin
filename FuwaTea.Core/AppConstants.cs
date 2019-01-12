namespace FuwaTea.Core
{
    public static class AppConstants
    {
        public const string PublisherName = "OronDF343";
        public const string ProductName = "Sage";

        public const string LogsDirName = "Logs";
        public const string ConfigDirName = "Config";
        public const string ExtensionsDirName = "Extensions";

        internal const string SettingsFileName = "appsettings.json";

        public static class Arguments
        {
            public const string UpdateFileAssociations = "--update-file-associations";
            public const string DeleteFileAssociations = "--delete-file-associations";
            public const string FileAssociationsUi = "--file-associations-ui";
            public const string ShouldBeAdmin = "--admin";
            public const string SetLanguage = "--set-language";
            public const string Wait = "--wait";
        }
    }
}
