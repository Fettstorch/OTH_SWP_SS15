#region Copyright information
// <summary>
// <copyright file="AsyncCommand.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>03/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UseCaseAnalyser.Model.ViewModel
{
    /// <summary>
    /// implementation of the icommand interface.
    /// used to bind to from view side
    /// </summary>
    public class AsyncCommand : ICommand
    {
        private static readonly TaskFactory UiTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

        private readonly Action<object> mExecuteAction;
        private readonly Func<object, bool> mCanExecuteFunc;
        private readonly Action<Exception> mOnError;
        private bool mIsExecuting;
        
        /// <summary>
        /// creates a new command to bind to from the gui
        /// </summary>
        /// <param name="executeAction">action to execute on command execute</param>
        /// <param name="canExecuteFunc">function to determine weather the action can be executed</param>
        /// <param name="onError">action to run if the execute action throws an exception</param>
        public AsyncCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc, Action<Exception> onError)
        {
            mExecuteAction = executeAction;
            mCanExecuteFunc = canExecuteFunc;
            mOnError = onError;
        }

        /// <summary>
        /// checks if the command is currently executable
        /// </summary>
        /// <param name="parameter">parameter which can be passed from the view</param>
        /// <returns>weather the command is executable</returns>
        public bool CanExecute(object parameter)
        {
            bool result = !mIsExecuting && mCanExecuteFunc(parameter);
            return result;
        }

        /// <summary>
        /// executes the action of the command
        /// </summary>
        /// <param name="parameter">action parameter which can be passed from the view</param>
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


        /// <summary>
        /// invoked if the commandmanager detects action which might change the executable state --> can execute will be invoked
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}