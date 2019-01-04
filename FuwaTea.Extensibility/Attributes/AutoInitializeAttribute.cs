using System.ComponentModel.Composition;

namespace FuwaTea.Extensibility
{
    public class AutoInitializeAttribute : ExportAttribute
    {
        public AutoInitializeAttribute()
            : base(ExtensibilityConstants.AutoInitKey) { }
    }
    public class InheritedAutoInitializeAttribute : InheritedExportAttribute
    {
        public InheritedAutoInitializeAttribute()
            : base(ExtensibilityConstants.AutoInitKey) { }
    }
}
