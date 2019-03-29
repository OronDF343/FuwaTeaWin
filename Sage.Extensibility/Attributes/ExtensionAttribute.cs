using System;

namespace Sage.Extensibility.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ExtensionAttribute : Attribute
    {
        public ExtensionAttribute(string key, int apiVersion)
        {
            Key = key;
            ApiVersion = apiVersion;
        }
        public string Key { get; }
        public int ApiVersion { get; }
    }
}
