using System.Linq;

namespace FuwaTea.Extensibility
{
    public static class ExtensibilityConstants
    {
        public const string InfoExportKey = "info";
        // API versions to allow loading automatically
        public static readonly int[] ApiVersionWhitelist = { 1 };
        // The current API version - highest version to allow loading (even manually! unless it is in the array above this)
        public const int CurrentApiVersion = 1;
        // API versions to never allow loading (even manually!)
        public static readonly int[] ApiVersionBlacklist = { 0 };
        public const string AutoInitKey = "autoinit";

        public static bool CheckApiVersion(int ver, bool overrideWhitelist = false)
        {
            return ApiVersionWhitelist.Contains(ver)
                   || overrideWhitelist && ver <= CurrentApiVersion && !ApiVersionBlacklist.Contains(ver);
        }
    }
}
