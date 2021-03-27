using System;
using System.Windows.Input;

namespace EBind.Tests.Models
{
    public class ExecuteCountCommand : ICommand
    {
        private readonly Action? _execute;

        public ExecuteCountCommand(Action? execute = null)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public int InvokeCount { get; private set; }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            InvokeCount++;
            _execute?.Invoke();
        }
    }
}
