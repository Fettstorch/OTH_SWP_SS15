#region Copyright information
// <summary>
// <copyright file="DialogView.xaml.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>22/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System.IO;
using System.Windows;
using Microsoft.Win32;
using UseCaseAnalyser.Model.Model;
using UseCaseAnalyser.Model.ViewModel;

namespace UseCaseAnalyser.View
{
    /// <summary>
    /// Interaction logic for DialogView.xaml
    /// </summary>
    public partial class DialogView : IDialogView
    {
        private ReportView mView;


        /// <summary>
        /// creates a new dialog view and sets the data context
        /// </summary>
        public DialogView()
        {
            InitializeComponent();
            DataContext = new DialogViewModel(this);
        }

        /// <summary>
        /// opens a file dialog and returns the file
        /// </summary>
        /// <param name="filter">filter of the files</param>
        /// <param name="dialogType">dialog type (open or save)</param>
        /// <returns>the file which has been selected</returns>
        public FileInfo OpenFileDialog(string filter, FileDialogType dialogType)
        {
            FileDialog dialog = dialogType == FileDialogType.Open ? (FileDialog) new OpenFileDialog() : new SaveFileDialog();
            dialog.Filter = filter;

            if (dialog.ShowDialog() != true) return null;

            FileInfo file = new FileInfo(dialog.FileName);
            return file;
        }

        /// <summary>
        /// opens a message box with the given parameters
        /// </summary>
        /// <param name="header">header of the message box</param>
        /// <param name="content">content of the message box</param>
        /// <param name="messageType">type of the message --> determines the message box icon</param>
        public void OpenMessageBox(string header, string content, MessageType messageType)
        {
            MessageBox.Show(content, header, MessageBoxButton.OK,
                messageType == MessageType.Error
                    ? MessageBoxImage.Error
                    : messageType == MessageType.Warning ? MessageBoxImage.Warning : MessageBoxImage.Information);
        }

        /// <summary>
        /// opens the report view
        /// </summary>
        /// <param name="viewModel">the report which is used as viewmodel of the report view</param>
        public void OpenReportResult(Report viewModel)
        {
            mView = new ReportView(viewModel) {Owner = this};
            mView.ShowDialog();
        }

        /// <summary>
        /// lets the use case graph viewer redraw the graph, which is currently represented
        /// </summary>
        public void RedrawGraph()
        {
            UseCaseGraphViewer.RedrawGraph();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
