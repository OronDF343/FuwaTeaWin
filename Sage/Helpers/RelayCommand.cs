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

    public class RelayCommand<T> : ICommand
    {
        private Action<T> _action;
        public RelayCommand() { }

        public RelayCommand(Action<T> action)
        {
            Action = action;
        }

        public Action<T> Action
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

        public bool CanExecute(T parameter)
        {
            return Action != null;
        }

        public void Execute(T parameter)
        {
            Action?.Invoke(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
