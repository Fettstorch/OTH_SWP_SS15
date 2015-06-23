#region Copyright information
// <summary>
// <copyright file="GenericValueConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>23/06/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// a base class for value converters to extract some logic used in all converters like checking the type
    /// </summary>
    /// <typeparam name="TSource">type of the source object</typeparam>
    /// <typeparam name="TTarget">type of the target object</typeparam>
    public abstract class GenericValueConverter<TSource, TTarget> : IValueConverter
    {
        private readonly bool mCheckNull;

        /// <summary>
        /// creates a new generic value converter with a value to determine weather the base class should check the values to convert for type, null
        /// </summary>
        /// <param name="checkNull">weather to check for null of the object to convert</param>
        protected GenericValueConverter(bool checkNull = true)
        {
            mCheckNull = checkNull;
        }

        /// <summary>
        /// converts the source value to the target value
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (mCheckNull)
            {
                if (value == null) return null;

                if (!(value is TSource))
                {
                    throw new NotSupportedException(string.Format("The converter '{0}' is not made for the source type '{1}'. The required type is '{2}'.", GetType().Name,
                        value.GetType().Name, typeof(TSource).Name));
                }
            }

            TTarget convertedValue = Convert((TSource) value);
            return convertedValue;
        }

        /// <summary>
        /// converts the target value to the source value
        /// </summary>
        /// <param name="value">object to convert</param>
        /// <param name="targetType">target type for the conversion</param>
        /// <param name="parameter">parameter which can be passed in view</param>
        /// <param name="culture">the current culture info</param>
        /// <returns>the converted object</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (!(value is TTarget))
            {
                throw new NotSupportedException(string.Format("The converter '{0}' is not made for the source type '{1}'. The required type is '{2}'.", GetType().Name,
                    value.GetType().Name, typeof(TTarget).Name));
            }

            TSource convertedValue = ConvertBack((TTarget) value);
            return convertedValue;
        }
        
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public abstract TTarget Convert(TSource source);

        /// <summary>
        /// converts the target value of type TTarget back to the source value of type TSource
        /// 
        /// throws NotSupportedException if not overridden
        /// </summary>
        /// <param name="target">value to be converted</param>
        /// <returns>the converted value</returns>
        public virtual TSource ConvertBack(TTarget target)
        {
            throw new NotSupportedException(string.Format("The conversion from type '{0}' back to type '{1}' is not supported.", typeof(TTarget).Name, typeof(TSource).Name));
        }
    }
}
