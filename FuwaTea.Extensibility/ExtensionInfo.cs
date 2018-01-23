using System.Reflection;

namespace FuwaTea.Extensibility
{
    public class ExtensionInfo
    {
        public ExtensionInfo(AssemblyName assemblyName, string dllPath, string key, int apiVersion, IExtensionBasicInfo basicInfo)
        {
            AssemblyName = assemblyName;
            DllPath = dllPath;
            Key = key;
            ApiVersion = apiVersion;
            BasicInfo = basicInfo;
        }
        public AssemblyName AssemblyName { get; }
        public string DllPath { get; }

        // both defined using assembly attributes - because they are REQUIRED to load the assembly into the container
        public string Key { get; }
        public int ApiVersion { get; }

        // imported via IoC - because it allows localization!
        public IExtensionBasicInfo BasicInfo { get; }

        public override string ToString()
        {
            return $"{{Key: {Key}, FullName: {AssemblyName.FullName}, Path: {DllPath}, ApiVersion: {ApiVersion}}}";
        }
    }
}
