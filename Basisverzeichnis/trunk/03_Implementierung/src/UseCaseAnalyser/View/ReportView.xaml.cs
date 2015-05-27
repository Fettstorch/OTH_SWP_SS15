#region Copyright information
// <summary>
// <copyright file="ReportView.xaml.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>20/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion

using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.View
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView
    {
        /// <summary>
        /// creates a new report view with the corresponding report as view model
        /// </summary>
        /// <param name="correspondingReport">the report</param>
        public ReportView(Report correspondingReport)
        {
            InitializeComponent();
            DataContext = correspondingReport;
        }
    }
}
