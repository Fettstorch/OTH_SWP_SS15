#region Copyright information
// <summary>
// <copyright file="EntryTypeToColorConverter.cs">Copyright (c) 2015</copyright>
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
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// convertes the report entry type to a color 
    /// log --> blue,
    /// warning --> yellow,
    /// error --> red
    /// default --> none
    /// </summary>
    public class EntryTypeToColorConverter : IValueConverter
    {   
        /// <summary>
        /// converts a entry type to a color for better displaying in the view.
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Report.Entrytype? type = value as Report.Entrytype?;
            if (type == null || type.Value == Report.Entrytype.DEFAULT)
            {
                return null;
            }


            Brush entryTypeBrush = type.Value == Report.Entrytype.LOG
                ? Brushes.LightGreen : type.Value == Report.Entrytype.WARNING ? Brushes.LightYellow : Brushes.LightSalmon;

            return entryTypeBrush;
        }

        /// <summary>
        /// converts the converted value back to its original type
        /// -- not supported here --> only 1 way binding is supported
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}