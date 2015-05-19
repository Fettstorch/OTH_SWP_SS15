using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCaseAnalyser.Model.ViewModel
{
    public interface IDialogView
    {

        FileInfo OpenFileDialog(string filter, FileDialogType dialogType);

        void OpenMessageBox(string header, string content, MessageType messageType);

        void OpenReportResult(DialogViewModel viewModel);
    }

    public enum MessageType
    {
        Information,
        Warning,
        Error
    }

    public enum FileDialogType
    {
        Open,
        Save
    }
}
