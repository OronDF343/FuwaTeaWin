using System.ComponentModel.Composition;

namespace FuwaTea.Extensibility
{
    public class ScopedImportAttribute : ImportAttribute
    {
        public ScopedImportAttribute(string module, string area, string name)
            : base(ExtensibilityUtils.MakeScopedKey(module, area, name))
        {
            Module = module;
            Area = area;
            Name = name;
        }

        public string Name { get; }

        public string Area { get; }

        public string Module { get; }
    }
}
