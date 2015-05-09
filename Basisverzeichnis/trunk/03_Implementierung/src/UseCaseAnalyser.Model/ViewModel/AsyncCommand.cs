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
        private bool mIsExecuting;

        public AsyncCommand(Action<object> executeAction) : this(executeAction, o => true) { }

        public AsyncCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc)
        {
            mExecuteAction = executeAction;
            mCanExecuteFunc = canExecuteFunc;
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
                mExecuteAction(parameter);
                mIsExecuting = false;
            });
        }

        public event EventHandler CanExecuteChanged;
    }
}