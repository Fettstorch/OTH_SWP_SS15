#region Copyright information
// <summary>
// <copyright file="IGraphElement.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>30/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System.Collections.Generic;

namespace GraphFramework.Interfaces
{
    /// <summary>
    /// An interface for classes that are able to hold objects of classes
    /// that implement the interface IAttribute. The interfaces 
    /// INode, IEdge and IGraph inherit from this interface.
    /// </summary>
    public interface IGraphElement
    {
        /// <summary>
        /// Contains all IAttribute objects of the IGraphElement.
        /// </summary>
        IEnumerable<IAttribute> Attributes { get; }

        /// <summary>
        /// Adds the IAttribute to the property Attributes of the IGraphElement.
        /// </summary>
        /// <param name="attribute">reference of the IAttribute to add to the property Attributes</param>
        void AddAttribute(IAttribute attribute);
        
        /// <summary>
        /// Removes the IAttribute identified by its name from the property Attributes.
        /// </summary>
        /// <param name="nameOfAttributeToRemove">property Name of the IAttribute to remove</param>
        void RemoveAttribute(string nameOfAttributeToRemove);

        /// <summary>
        /// Removes the IAttribute from the property Attributes.
        /// </summary>
        /// <param name="attributeToRemove">reference of the IAttribute to remove</param>
        void RemoveAttribute(IAttribute attributeToRemove);
    }
}