using System.Linq;

namespace FuwaTea.Extensibility
{
    public static class ExtensibilityUtils
    {
        public static string MakeScopedKey(string module, string area, string name)
        {
            return module + "." + area + "." + name;
        }

        public static bool CheckApiVersion(int ver, bool overrideWhitelist = false)
        {
            return ExtensibilityConstants.ApiVersionWhitelist.Contains(ver)
                   || overrideWhitelist && ver <= ExtensibilityConstants.CurrentApiVersion && !ExtensibilityConstants.ApiVersionBlacklist.Contains(ver);
        }
    }
}