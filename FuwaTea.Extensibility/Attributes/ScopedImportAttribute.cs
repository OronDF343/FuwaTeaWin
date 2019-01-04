using System.ComponentModel.Composition;

namespace FuwaTea.Extensibility.Attributes
{
    public class ScopedImportAttribute : ImportAttribute
    {
        public ScopedImportAttribute(string module, string area, string name)
            : base(Utils.MakeScopedKey(module, area, name))
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
