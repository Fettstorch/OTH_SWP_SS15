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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts the report to a collection of all its entries
    /// </summary>
    public class ReportToEntriesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Report report = value as Report;

            if (report == null)
            {
                return null;
            }

            IEnumerable<Report.ReportEntry> entries =
                report.LogReportEntries.Concat(report.WarningReportEntries).Concat(report.ErrorReportEntries);

            return entries;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
