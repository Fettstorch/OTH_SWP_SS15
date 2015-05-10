using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UseCaseAnalyser.Model.ViewModel
{
    public class AsyncCommand : ICommand
    {
        private static readonly TaskFactory UiTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

        private readonly Action<object> mExecuteAction;
        private readonly Func<object, bool> mCanExecuteFunc;
        private readonly Action<Exception> mOnError;
        private bool mIsExecuting;

        public AsyncCommand(Action<object> executeAction) : this(executeAction, o => true, e => { }) { }

        public AsyncCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc, Action<Exception> onError)
        {
            mExecuteAction = executeAction;
            mCanExecuteFunc = canExecuteFunc;
            mOnError = onError;
        }

        public bool CanExecute(object parameter)
        {
            bool result = !mIsExecuting && mCanExecuteFunc(parameter);
            return result;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;

            mIsExecuting = true;
            UiTaskFactory.StartNew(() =>
            {
                try
                {
                    mExecuteAction(parameter);
                }
                catch (Exception ex)
                {
                    mOnError(ex);
                }

                mIsExecuting = false;
            });
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}