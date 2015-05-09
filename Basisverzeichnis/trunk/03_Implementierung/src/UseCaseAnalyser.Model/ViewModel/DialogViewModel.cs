using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using GraphFramework;
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Model.ViewModel
{
    public class DialogViewModel : INotifyPropertyChanged
    {
        private IEnumerable<UseCaseGraph> mMUseCaseGraphs;
        private ICommand mOpenWordFile;
        private ICommand mExportScenarioMatrix;
        private UseCaseGraph mSelectedGraph;

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
            set { mSelectedGraph = value; OnPropertyChanged(); }
        }

        public IGraph SelectedScenario { get; set; }

        public IGraphElement SelectedGraphElement { get; set; }

        //  commands in menu
        public ICommand OpenWordFile
        {
            get
            {
                //  lazy initialization
                return mOpenWordFile ?? (mOpenWordFile = new AsyncCommand(o =>
                {
                    //  bad mvvm practice (dialogs should be done in view --> can't be tested automatically)
                    //  still handled here for easier understanding
                    OpenFileDialog dialog = new OpenFileDialog { Filter = "Word files (.docx)|*.docx", Multiselect = false };
                    if (dialog.ShowDialog() != DialogResult.OK) return;

                    string filePath = dialog.FileName;
                    UseCaseGraphs = WordImporter.ImportUseCases(new FileInfo(filePath));
                }));
            }
        }

        public ICommand ExportScenarioMatrix
        {
            get
            {
                //  lazy initialization
                return mExportScenarioMatrix ?? (mExportScenarioMatrix = new AsyncCommand(o =>
                {
                    //  bad mvvm practice (dialogs should be done in view --> can't be tested automatically)
                    SaveFileDialog dialog = new SaveFileDialog { Filter = "Excel files (.xlsx)|*.xlsx" };
                    if (dialog.ShowDialog() != DialogResult.OK) return;

                    string filePath = dialog.FileName;
                    ScenarioMatrixExporter.ExportScenarioMatrix(SelectedGraph, new FileInfo(filePath));
                }, o => SelectedGraph != null));
                //  condtion to run the command (a graph has to be selected)
            }
        }

        #region Property Changed event + invoker to notify gui about changes
        public event PropertyChangedEventHandler PropertyChanged;

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "False positive - attribute introduced in .NET 4.5 is similiar to overloading")]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}