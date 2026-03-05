using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERP_Project.Commands
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T>? _execute;
        private readonly Predicate<T>? _canExecute;

        public RelayCommand(Action<T>? execute, Predicate<T>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
                return true;

            if (parameter == null && typeof(T).IsValueType)
                return _canExecute(default!);

            return _canExecute((T)parameter!);
        }

        public void Execute(object? parameter)
        {
            if (_execute == null)
                return;

            if (parameter == null && typeof(T).IsValueType)
                _execute(default!);

            _execute((T)parameter!);
        }
    }
}