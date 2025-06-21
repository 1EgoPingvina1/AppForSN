using System;
using System.Windows.Input;

namespace AppForSNForUsers.Contracts
{
    public class NavigateCommand : ICommand
    {
        private readonly Action<object> _execute;

        public NavigateCommand(Action<object> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
