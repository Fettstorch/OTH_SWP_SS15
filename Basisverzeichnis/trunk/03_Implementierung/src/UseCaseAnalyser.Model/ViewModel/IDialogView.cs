using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Model.ViewModel
{
    /// <summary>
    /// abstraction of the dialog view
    /// used to execute view actions from viewmodel side
    /// </summary>
    public interface IDialogView
    {
        /// <summary>
        /// opens a file dialog and returns the file
        /// </summary>
        /// <param name="filter">filter of the files</param>
        /// <param name="dialogType">dialog type (open or save)</param>
        /// <returns>the file which has been selected</returns>
        FileInfo OpenFileDialog(string filter, FileDialogType dialogType);

        /// <summary>
        /// opens a message box with the given parameters
        /// </summary>
        /// <param name="header">header of the message box</param>
        /// <param name="content">content of the message box</param>
        /// <param name="messageType">type of the message --> determines the message box icon</param>
        void OpenMessageBox(string header, string content, MessageType messageType);

        /// <summary>
        /// opens the report view
        /// </summary>
        /// <param name="viewModel">the report which is used as viewmodel of the report view</param>
        void OpenReportResult(Report viewModel);
    }

    /// <summary>
    /// enum for the different message types to be displayed in message boxes
    /// </summary>
    public enum MessageType
    {
        Information,
        Warning,
        Error
    }

    /// <summary>
    /// enum for the different file dialog types
    /// </summary>
    public enum FileDialogType
    {
        Open,
        Save
    }
}
