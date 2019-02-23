using System;
using System.Windows.Input;

namespace StardewValleyLocalization
{
    public class DelegateCommand : ICommand
    {
        private readonly Func<object, bool> canExecute;
        private readonly Action<object> execute;

        public DelegateCommand(Action<object> execute)
        {
            this.execute = execute;
            canExecute = x => true;
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}