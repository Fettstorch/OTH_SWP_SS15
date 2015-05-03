using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UseCaseAnalyser.Model.ViewModel
{
    public class AsyncCommand : ICommand
    {
        private readonly Action<object> _executeAction;
        private readonly Func<object, bool> _canExecuteFunc;
        private bool _isExecuting;

        public AsyncCommand(Action<object> executeAction) : this(executeAction, o => true){ }

        public AsyncCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc)
        {
            this._executeAction = executeAction;
            this._canExecuteFunc = canExecuteFunc;
        }

        public bool CanExecute(object parameter)
        {
            bool result = !this._isExecuting && this._canExecuteFunc(parameter);
            return result;
        }

        public void Execute(object parameter)
        {
            if (!this.CanExecute(parameter)) return;

            this._isExecuting = true;
            Task.Factory.StartNew(() =>
            {
                this._executeAction(parameter);
                this._isExecuting = false;
            });
        }

        public event EventHandler CanExecuteChanged;
    }
}