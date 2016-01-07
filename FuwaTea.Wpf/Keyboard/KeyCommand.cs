using System.Collections.Generic;
using System.Windows.Input;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Wpf.Keyboard
{
    public class KeyCommand : IKeyedElement<string>
    {
        public KeyCommand()
        {
            SupportedKinds = new HashSet<KeyBindingKind>();
            Commands = new List<ICommand>();
        }

        public string Key { get; set; }

        public HashSet<KeyBindingKind> SupportedKinds { get; set; }

        public List<ICommand> Commands { get; set; }
    }
}
