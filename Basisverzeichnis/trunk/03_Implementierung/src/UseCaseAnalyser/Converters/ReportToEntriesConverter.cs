#region Copyright information
// <summary>
// <copyright file="ReportToEntriesConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>20/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System.Linq;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts the report to a collection of all its entries
    /// </summary>
    public class ReportToEntriesConverter : GenericValueConverter<Report, Report.ReportEntry[]>
    {
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override Report.ReportEntry[] Convert(Report source)
        {
            return source.LogReportEntries.Concat(source.WarningReportEntries).Concat(source.ErrorReportEntries).ToArray();
        }
    }
}
