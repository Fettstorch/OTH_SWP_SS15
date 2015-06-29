#region Copyright information
// <summary>
// <copyright file="IsNullVisibilityConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>29/06/2015</creationDate>
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
    /// determines weather the object is null and returns a visibility
    /// null --> hidden
    /// not null --> visible
    /// </summary>
    public class IsNullVisibilityConverter : GenericValueConverter<object, Visibility>
    {
        /// <summary>
        /// initializes the converter without checks for null in base class
        /// </summary>
        public IsNullVisibilityConverter()
            : base(false)
        {

        }

        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override Visibility Convert(object source)
        {
            return source == null ? Visibility.Hidden : Visibility.Visible;
        }
    }
}