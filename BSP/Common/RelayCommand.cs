using System;
using System.Windows.Input;

namespace BSP.Common
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action<object>? execute, Func<object, bool>? canExecute = null)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        public Func<object?, bool>? canExecute;
        public Action<object?>? execute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute.Invoke(parameter);
        }

        public void Execute(object? parameter)
        {
            execute?.Invoke(parameter);
        }
    }
}
