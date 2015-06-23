#region Copyright information
// <summary>
// <copyright file="EntryTypeToImageConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>26/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System;
using System.Windows.Media.Imaging;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts a report entry type to a bitmap image
    /// </summary>
    public class EntryTypeToImageConverter : GenericValueConverter<Report.Entrytype, BitmapImage>
    {
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override BitmapImage Convert(Report.Entrytype source)
        {
            return new BitmapImage(source == Report.Entrytype.ERROR
                ? new Uri(@"/Resources/Error.png", UriKind.Relative)
                : source == Report.Entrytype.WARNING ? new Uri(@"/Resources/Warning.png", UriKind.Relative) :
                new Uri(@"/Resources/Information.png", UriKind.Relative));
        }
    }
}