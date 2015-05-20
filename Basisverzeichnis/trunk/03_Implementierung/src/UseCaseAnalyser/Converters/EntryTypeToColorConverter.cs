using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    public class EntryTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Report.Entrytype? type = value as Report.Entrytype?;
            if (type == null || type.Value == Report.Entrytype.DEFAULT)
            {
                return null;
            }


            Brush entryTypeBrush = type.Value == Report.Entrytype.LOG
                ? Brushes.LightBlue : type.Value == Report.Entrytype.WARNING ? Brushes.LightYellow : Brushes.LightSalmon;

            return entryTypeBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}