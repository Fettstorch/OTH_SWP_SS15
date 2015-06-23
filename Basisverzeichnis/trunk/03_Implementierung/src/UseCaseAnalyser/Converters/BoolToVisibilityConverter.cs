#region Copyright information
// <summary>
// <copyright file="BoolToVisibilityConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>28/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System.Windows;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts a bool to a visibility
    /// </summary>
    public class BoolToVisibilityConverter : GenericValueConverter<bool, Visibility>
    {
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override Visibility Convert(bool source)
        {
            return source ? Visibility.Visible : Visibility.Hidden;
        }
    }
}