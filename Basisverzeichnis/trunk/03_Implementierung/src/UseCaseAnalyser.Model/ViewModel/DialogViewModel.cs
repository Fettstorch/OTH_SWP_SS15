using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using GraphFramework;
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;
using Attribute = GraphFramework.Attribute;

namespace UseCaseAnalyser.Model.ViewModel
{
    public class DialogViewModel : INotifyPropertyChanged
    {
        private readonly IDialogView mView;
        private ICommand mExportScenarioMatrix;
        private IEnumerable<UseCaseGraph> mMUseCaseGraphs;
        private ICommand mOpenWordFile;
        private UseCaseGraph mSelectedGraph;
        private IGraph mSelectedScenario;
        //private IGraphElement mSelectedGraphElement;

        public DialogViewModel(){ }

        public DialogViewModel(IDialogView view)
        {
            mView = view;
        }

        public IEnumerable<UseCaseGraph> UseCaseGraphs
        {
            get { return mMUseCaseGraphs; }
            private set
            {
                mMUseCaseGraphs = value;
                //  notify gui by fireing property changed
                OnPropertyChanged();
            }
        }

        //  set by gui
        public UseCaseGraph SelectedGraph
        {
            get { return mSelectedGraph; }
            set
            {
                mSelectedGraph = value;
                OnPropertyChanged();
            }
        }

        public IGraph SelectedScenario
        {
            get { return mSelectedScenario; }
            set
            {
                mSelectedScenario = value;
                OnPropertyChanged();
            }
        }

        //public IGraphElement SelectedGraphElement
        //{
        //    get { return mSelectedGraphElement; }
        //    set
        //    {
        //        mSelectedGraphElement = value;
        //        OnPropertyChanged();
        //    }
        //}

        //  commands in menu

        public ICommand OpenWordFile
        {
            get
            {
                //  lazy initialization
                return mOpenWordFile ?? (mOpenWordFile = new AsyncCommand(o =>
                {
                    FileInfo file = mView.OpenFileDialog("Word files |*.docx|All files |*.*", FileDialogType.Open);
                    if (file == null) return;

                    UseCaseGraphs = WordImporter.ImportUseCases(file);
                }, o => true, OnError));
            }
        }

        public ICommand ExportScenarioMatrix
        {
            get
            {
                //  lazy initialization
                return mExportScenarioMatrix ?? (mExportScenarioMatrix = new AsyncCommand(o =>
                {
                    FileInfo file = mView.OpenFileDialog("Excel files (.xlsx)|*.xlsx", FileDialogType.Save);

                    ScenarioMatrixExporter.ExportScenarioMatrix(SelectedGraph, file);
                }, o => SelectedGraph != null, OnError));
                //  condtion to run the command (a graph has to be selected)
            }
        }

        private void OnError(Exception ex)
        {
            mView.OpenMessageBox(ex.GetType().Name, string.Format("An error occured:{0}{1}", Environment.NewLine, ex.Message), MessageType.Error);
        }

        #region Property Changed event + invoker to notify gui about changes

        public event PropertyChangedEventHandler PropertyChanged;

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed",
            Justification = "False positive - attribute introduced in .NET 4.5 is similiar to overloading")]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}