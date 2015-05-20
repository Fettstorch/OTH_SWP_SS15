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
        public DialogView()
        {
            InitializeComponent();
            DataContext = new DialogViewModel(this);
        }

        public FileInfo OpenFileDialog(string filter, FileDialogType dialogType)
        {
            FileDialog dialog = dialogType == FileDialogType.Open ? (FileDialog) new OpenFileDialog() : new SaveFileDialog();
            dialog.Filter = filter;

            if (dialog.ShowDialog() != true) return null;

            FileInfo file = new FileInfo(dialog.FileName);
            return file;
        }

        public void OpenMessageBox(string header, string content, MessageType messageType)
        {
            MessageBox.Show(content, header, MessageBoxButton.OK,
                messageType == MessageType.Error
                    ? MessageBoxImage.Information
                    : messageType == MessageType.Warning ? MessageBoxImage.Warning : MessageBoxImage.Information);
        }

        public void OpenReportResult(Report viewModel)
        {
            ReportView view = new ReportView(viewModel);
            view.ShowDialog();
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
