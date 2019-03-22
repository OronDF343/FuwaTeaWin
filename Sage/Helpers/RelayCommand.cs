using System;
using System.Windows.Input;

namespace Sage.Helpers
{
    public class RelayCommand : ICommand
    {
        private Action _action;
        public RelayCommand() { }

        public RelayCommand(Action action)
        {
            Action = action;
        }

        public Action Action
        {
            get => _action;
            set
            {
                var prev = _action;
                _action = value;
                if (prev == null ^ _action == null)
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            return Action != null;
        }

        public void Execute(object parameter)
        {
            Action?.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}
