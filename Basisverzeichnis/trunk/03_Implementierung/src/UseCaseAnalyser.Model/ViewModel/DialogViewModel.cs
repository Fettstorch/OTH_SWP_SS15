using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Model.ViewModel
{
    public class DialogViewModel : INotifyPropertyChanged
    {
        private UseCaseGraph[] _mUseCaseGraphs;
        private ICommand _mOpenWordFile;
        private ICommand _mExportScenarioMatrix;


        public UseCaseGraph[] UseCaseGraphs
        {
            get { return this._mUseCaseGraphs; }
            private set 
            {
                this._mUseCaseGraphs = value;
                //  notify gui by fireing property changed
                this.OnPropertyChanged();
            }
        }

        //  set by gui
        public UseCaseGraph SelectedGraph { get; set; }

        public ICommand OpenWordFile
        {
            get
            {
                //  lazy initialization
                return this._mOpenWordFile ?? (this._mOpenWordFile = new AsyncCommand(o =>
                {
                    //  bad mvvm practice (dialogs should be done in view --> can't be tested automatically)
                    OpenFileDialog dialog = new OpenFileDialog {Filter = "Word files (.docx)|*.docx", Multiselect = false};
                    if (dialog.ShowDialog() != DialogResult.OK) return;

                    string filePath = dialog.FileName;
                    this.UseCaseGraphs = WordImporter.ImportUseCases(new FileInfo(filePath)).ToArray();
                }));
            }
        }

        public ICommand ExportScenarioMatrix
        {
            get
            {
                //  lazy initialization
                return this._mExportScenarioMatrix ?? (this._mExportScenarioMatrix = new AsyncCommand(o =>
                {
                    //  bad mvvm practice (dialogs should be done in view --> can't be tested automatically)
                    SaveFileDialog dialog = new SaveFileDialog { Filter = "Excel files (.xlsx)|*.xlsx" };
                    if (dialog.ShowDialog() != DialogResult.OK) return;

                    string filePath = dialog.FileName;
                    ScenarioMatrixExporter.ExportScenarioMatrix(this.SelectedGraph, new FileInfo(filePath));
                }, o => this.SelectedGraph != null));
                //  condtion to run the command (a graph has to be selected)
            }
        }

        #region Property Changed event + invoker to notify gui about changes
        public event PropertyChangedEventHandler PropertyChanged;

       [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "False positive - attribute introduced in .NET 4.5 is similiar to overloading")]
       protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}