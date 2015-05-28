using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class EntryTypeToImageConverter : IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Report.Entrytype? entrytype = value as Report.Entrytype?;
            if (entrytype == null) return null;

            Report.Entrytype actualType = entrytype.Value;
            Uri uri = actualType == Report.Entrytype.ERROR
                ? new Uri(@"/Resources/Error.png", UriKind.Relative)
                : actualType == Report.Entrytype.WARNING ? new Uri(@"/Resources/Warning.png", UriKind.Relative) :
                new Uri(@"/Resources/Information.png", UriKind.Relative);

            return new BitmapImage(uri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}