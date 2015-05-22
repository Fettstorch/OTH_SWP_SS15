using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GraphFramework.Interfaces;
using LogManager;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Model.ViewModel
{
    /// <summary>
    /// main view model of the application
    /// provides all properties which will be displayed in view
    /// </summary>
    public class DialogViewModel : INotifyPropertyChanged
    {
        private readonly IDialogView mView;
        private IEnumerable<UseCaseGraph> mMUseCaseGraphs;
        private UseCaseGraph mSelectedGraph;
        private IGraph mSelectedScenario;
        private ICommand mExportScenarioMatrix;
        private ICommand mOpenWordFile;
        private ICommand mOpenLogfile;
        private ICommand mOpenReportView;
        //private IGraphElement mSelectedGraphElement;

        /// <summary>
        /// creates a new dialogviewmodel without interface of the view 
        /// (for tests and wpf designer)
        /// </summary>
        public DialogViewModel() : this(null) { }

        /// <summary>
        /// creates a new dialogviewmodel. view specific actions can be invoked over the view interface
        /// </summary>
        /// <param name="view">interface abstraction of the view</param>
        public DialogViewModel(IDialogView view)
        {
            mView = view;
        }

     

        /// <summary>
        /// the currently selected graph from the view --> set via binding
        /// </summary>
        public UseCaseGraph SelectedGraph
        {
            get { return mSelectedGraph; }
            set
            {
                mSelectedGraph = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// the currently selection scenario from the view --> set via binding
        /// </summary>
        public IGraph SelectedScenario
        {
            get { return mSelectedScenario; }
            set
            {
                mSelectedScenario = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// the latest word import report gotten from the word importer
        /// </summary>
        public Report LatestWordImportReport { get; private set; }

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

        /// <summary>
        /// opens a word file and tries to read in the use cases
        /// </summary>
        public ICommand OpenWordFile
        {
            get
            {
                //  lazy initialization
                return mOpenWordFile ?? (mOpenWordFile = new AsyncCommand(o =>
                {
                    FileInfo file = mView.OpenFileDialog("Word files |*.docx|All files |*.*", FileDialogType.Open);
                    if (file == null) return;

                    Report newReport;
                    UseCaseGraphs = WordImporter.ImportUseCases(file, out newReport);
                    LatestWordImportReport = newReport;

                    if (LatestWordImportReport.ErrorReportEntries.Any() || LatestWordImportReport.WarningReportEntries.Any())
                    {
                        mView.OpenReportResult(LatestWordImportReport); 
                    }
                }, o => true, OnError));
            }
        }

        /// <summary>
        /// exports the scenarios from the currently selected use case
        /// 
        /// enabled if: a use case is selected
        /// </summary>
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

        /// <summary>
        /// opens the logfile as seperate process
        /// 
        /// enabled if: the logfile exists
        /// </summary>
        public ICommand OpenLogfile
        {
            get
            {
                string logfile = Path.Combine(LoggingFunctions.FilePath, LoggingFunctions.FileName);
                //  lazy initialization
                return mOpenLogfile ?? (mOpenLogfile = new AsyncCommand(o =>
                {
                    Process.Start(logfile);
                }, o => File.Exists(logfile), OnError));
                //  condtion to run the command (always true)
            }
        }

        /// <summary>
        /// opens the latest word import report view in its seperate window
        /// 
        /// enabled if: there is a latest word import report
        /// </summary>
        public ICommand OpenReportView
        {
            get
            {
                //  lazy initialization
                return mOpenReportView ?? (mOpenReportView = new AsyncCommand(o =>
                {
                    mView.OpenReportResult(LatestWordImportReport);
                }, o => LatestWordImportReport != null, OnError));
                //  condtion to run the command (always true)
            }
        }

        private void OnError(Exception ex)
        {
            LoggingFunctions.Exception(ex);
            mView.OpenMessageBox(ex.GetType().Name, string.Format("An error occured:{0}{1}", Environment.NewLine, ex.Message), MessageType.Error);
        }


        /// <summary>
        /// all use cases which are currently saved (have been read by the word importer)
        /// </summary>
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
        #region Property Changed event + invoker to notify gui about changes
        /// <summary>
        /// invoked to notify the gui about changed of properties
        /// </summary>
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