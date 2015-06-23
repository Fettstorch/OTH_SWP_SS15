#region Copyright information
// <summary>
// <copyright file="IsNullConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>23/06/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// determines weather the object is null
    /// </summary>
    public class IsNullConverter : GenericValueConverter<object, bool>
    {
        /// <summary>
        /// initializes the converter without checks for null in base class
        /// </summary>
        public IsNullConverter() : base(false)
        {
            
        }
        
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override bool Convert(object source)
        {
            return source == null;
        }
    }
}