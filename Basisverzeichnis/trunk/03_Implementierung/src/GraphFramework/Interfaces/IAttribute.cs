#region Copyright information
// <summary>
// <copyright file="IAttribute.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>30/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System;

namespace GraphFramework.Interfaces
{
    /// <summary>
    /// An interface for classes that contain a value that has a name.
    /// Objects of classes that implement this interface can be added
    /// to objects of classes that implement the interface IGraphElement.
    /// </summary>
    public interface IAttribute
    {
        /// <summary>
        /// Value of the IAttribute.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// Type of the property Value of the IAttribute.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Name to identify the IAttribute.
        /// </summary>
        string Name { get; }
    }
}