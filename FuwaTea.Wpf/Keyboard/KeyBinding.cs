using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using FuwaTea.Lib.Collections;
using FuwaTea.Lib.DataModel;
using JetBrains.Annotations;
using log4net;

namespace FuwaTea.Wpf.Keyboard
{
    [XmlRoot]
    public class KeyBinding : SugoiObject<string>
    {
        [XmlAttribute]
        public string Name
        {
            get => Key;
            set
            {
                if (Key == value) return;
                Key = value;
                OnPropertyChanged();
            }
        }

        private bool _enabled = true;
        private ObservableHashSet<Key> _keyGesture = new ObservableHashSet<Key>();
        private string _commandKey;
        private ObservableCollection<string> _commandParameters = new ObservableCollection<string>();
        private KeyBindingKind _kind;

        [XmlAttribute]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                OnPropertyChanged();
            }
        }

        [XmlArray(nameof(KeyGesture))]
        [XmlArrayItem(nameof(System.Windows.Input.Key))]
        public ObservableHashSet<Key> KeyGesture
        {
            get => _keyGesture;
            set
            {
                if (_keyGesture == value) return;
                _keyGesture = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public string CommandKey
        {
            get => _commandKey;
            set
            {
                if (_commandKey == value) return;
                _commandKey = value;
                OnPropertyChanged();
            }
        }

        [XmlArray(nameof(CommandParameters))]
        [XmlArrayItem("Parameter")]
        public ObservableCollection<string> CommandParameters
        {
            get => _commandParameters;
            set
            {
                if (_commandParameters == value) return;
                _commandParameters = value;
                OnPropertyChanged();
            }
        }

        [XmlAttribute]
        public KeyBindingKind Kind
        {
            get => _kind;
            set
            {
                if (_kind == value) return;
                _kind = value;
                OnPropertyChanged();
            }
        }

        public override object Clone()
        {
            return new KeyBinding
            {
                Name = Name,
                KeyGesture = new ObservableHashSet<Key>(KeyGesture),
                CommandKey = CommandKey,
                CommandParameters = new ObservableCollection<string>(CommandParameters),
                Kind = Kind
            };
        }

        public override void RestoreValues(SugoiObject<string> so)
        {
            base.RestoreValues(so);
            var kb = (KeyBinding)so;
            KeyGesture = kb.KeyGesture;
            CommandKey = kb.CommandKey;
            CommandParameters = kb.CommandParameters;
            Kind = kb.Kind;
        }

        private bool _isGestureInProgress;
        private int _toggleCounter;

        public void OnGestureDown(KeyCommand command)
        {
            if (Kind == KeyBindingKind.Repeat)
            {
                if (CheckCommandCount(command, 0))
                    ExecuteWithCheck(command.Commands[0], CommandParameters.FirstOrDefault());
            }
            else if (!_isGestureInProgress)
            {
                _isGestureInProgress = true;
                if (Kind == KeyBindingKind.Hold && CheckCommandCount(command, 0))
                    ExecuteWithCheck(command.Commands[0], CommandParameters.FirstOrDefault());
            }
        }

        public void OnGestureUp(KeyCommand command)
        {
            if (!_isGestureInProgress) return;
            _isGestureInProgress = false;
            if (Kind == KeyBindingKind.Repeat) return;
            var i = Kind == KeyBindingKind.Hold
                        ? 1
                        : Kind == KeyBindingKind.Toggle
                              ? (_toggleCounter = (_toggleCounter + 1) % command.Commands.Count)
                              : 0;
            if (CheckCommandCount(command, i))
                ExecuteWithCheck(command.Commands[i], CommandParameters.Count > i ? CommandParameters[i] : null);
        }

        private bool ExecuteWithCheck(ICommand cmd, [CanBeNull] object param)
        {
            if (!cmd.CanExecute(param)) return false;
            cmd.Execute(param);
            return true;
        }

        private bool CheckCommandCount(KeyCommand cmd, int i)
        {
            if (cmd.Commands.Count > i) return true;
            LogManager.GetLogger(GetType()).Error("Invalid KeyBinding: Missing command(s) in KeyCommand or wrong KeyBindingKind!");
            return false;
        }
    }

    public enum KeyBindingKind
    {
        /// <summary>
        /// A standard keystroke (activates on KeyUp)
        /// </summary>
        Normal,
        /// <summary>
        /// A repeatable keystroke (activates on KeyDown)
        /// </summary>
        Repeat,
        /// <summary>
        /// A toggle keystroke (alternates between commands on KeyUp)
        /// </summary>
        Toggle,
        /// <summary>
        /// A held keystroke (different command on *first* KeyDown and on KeyUp)
        /// </summary>
        Hold
    }
}
