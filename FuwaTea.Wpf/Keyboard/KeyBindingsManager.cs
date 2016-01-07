using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using log4net;

namespace FuwaTea.Wpf.Keyboard
{
    public sealed class KeyBindingsManager
    {
        private static KeyBindingsManager _instance;
        public static KeyBindingsManager Instance => _instance ?? (_instance = new KeyBindingsManager());

        private KeyBindingsManager() { }

        private KeyboardListener _listener;

        public KeyboardListener Listener
        {
            get { return _listener; }
            set
            {
                if (_listener != null)
                {
                    _listener.KeyDown -= Listener_KeyDown;
                    _listener.KeyUp -= Listener_KeyUp;
                }
                _keysDown.Clear();
                _cmd = null;
                _chosenKeyBinding = null;
                _listener = value;
                if (_listener == null) return;
                _listener.KeyDown += Listener_KeyDown;
                _listener.KeyUp += Listener_KeyUp;
            }
        }

        private readonly HashSet<Key> _keysDown = new HashSet<Key>();
        private KeyBinding _chosenKeyBinding;
        private KeyCommand _cmd;

        private void Listener_KeyDown(object sender, RawKeyEventArgs e)
        {
            if (_chosenKeyBinding == null || !_keysDown.Contains(e.Key)) _keysDown.Add(e.Key);
            else return;
            if (_chosenKeyBinding == null)
                _chosenKeyBinding = KeyBindings.FirstOrDefault(kb => kb.KeyGesture.SetEquals(_keysDown) && kb.Enabled);
            if (_chosenKeyBinding == null) return;
            if (_cmd == null)
                _cmd = KeyCommands.FirstOrDefault(kc => kc.Key == _chosenKeyBinding.CommandKey);
            if (_cmd == null)
            {
                LogManager.GetLogger(GetType())
                          .Error("Key command \"" + _chosenKeyBinding.CommandKey + "\" not found!");
                return;
            }
            if (!_cmd.SupportedKinds.Contains(_chosenKeyBinding.Kind))
            {
                LogManager.GetLogger(GetType())
                          .Error("Invalid key binding kind \""
                                 + Enum.GetName(typeof(KeyBindingKind), _chosenKeyBinding.Kind)
                                 + "\" - it is not supported by the key command \"" + _cmd.Key + "\"!");
                _cmd = null;
                return;
            }
            _chosenKeyBinding.OnGestureDown(_cmd);
        }

        private void Listener_KeyUp(object sender, RawKeyEventArgs e)
        {
            _keysDown.Remove(e.Key);
            if (_keysDown.Count > 0) return;
            if (_chosenKeyBinding != null && _cmd != null)
                _chosenKeyBinding.OnGestureUp(_cmd);
            _cmd = null;
            _chosenKeyBinding = null;
        }
        
        public List<KeyCommand> KeyCommands { get; } = new List<KeyCommand>();
        
        public ObservableCollection<KeyBinding> KeyBindings { get; } = new ObservableCollection<KeyBinding>();
    }
}
