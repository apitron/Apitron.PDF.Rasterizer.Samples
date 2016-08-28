using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Apitron.WpfPdfViewer.Controls
{
    /// <summary>
    /// Basic implementation of ICommand interface.
    /// </summary>
    public class ApitronCommand : ICommand
    {
        private Predicate<object> canExecute;
        private Action<object> executeAction;

        public ApitronCommand(Predicate<object> canExecute, Action<object> executeAction)
        {
            this.canExecute = canExecute;
            this.executeAction = executeAction;
        }

        public void Execute(object parameter)
        {
            executeAction(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}