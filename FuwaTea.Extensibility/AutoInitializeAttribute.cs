using System.ComponentModel.Composition;

namespace FuwaTea.Extensibility
{
    public class AutoInitializeAttribute : ExportAttribute
    {
        public AutoInitializeAttribute()
            : this(InitIf.UiMode) { }

        public AutoInitializeAttribute(InitIf condition)
            : base(ExtensibilityConstants.AutoInitKey + condition)
        {
            Condition = condition;
        }

        public InitIf Condition { get; }
    }

    public enum InitIf
    {
        Always = 0,
        ConsoleMode = 1,
        UiMode = 2
    }
}
