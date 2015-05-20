using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
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
