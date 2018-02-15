using System;

namespace FuwaTea.Extensibility
{
    public static class ExtensibilityUtils
    {
        public static string MakeScopedKey(string module, string area, string name)
        {
            return module + "." + area + "." + name;
        }
    }
}