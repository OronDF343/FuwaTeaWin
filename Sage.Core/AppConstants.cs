using System.Collections.Generic;

namespace Sage.Core
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
            public const string SwitchCharShort = "-";
            public const string SwitchCharLong = "--";
            public const string SwitchValueSeparator = ":";
            public static readonly Argument Files = new Argument("", "", true);
            public static readonly Argument LogLevel = new Argument("log-level", "l", true);
            public static readonly Argument FileLogLevel = new Argument("file-log-level", "L", true);
            public static readonly Argument UpdateFileAssociations = new Argument("update-file-associations");
            public static readonly Argument DeleteFileAssociations = new Argument("delete-file-associations");
            public static readonly Argument FileAssociationsUi = new Argument("file-associations-ui");
            public static readonly Argument AsAdmin = new Argument("admin");
            public static readonly Argument SetLanguage = new Argument("set-language", hasValue: true);
            public static readonly Argument Wait = new Argument("wait", "w");
            public static readonly Argument Parameter = new Argument("parameter", "p", true);

            public static readonly HashSet<Argument> All = new HashSet<Argument>
            {
                LogLevel, FileLogLevel, UpdateFileAssociations, DeleteFileAssociations, FileAssociationsUi, AsAdmin,
                SetLanguage, Wait, Parameter
            };
        }
    }
}
