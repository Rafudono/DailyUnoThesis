using System;
using System.Windows.Input;

namespace DailyUnoThesis.Models
{
    public class VmCommand : ICommand
    {
        Action action;

        public VmCommand(Action action)
        {
            this.action = action;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action();
        }
    }
}
