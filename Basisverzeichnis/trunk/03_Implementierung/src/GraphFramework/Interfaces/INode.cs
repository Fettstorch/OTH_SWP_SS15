#region Copyright information
// <summary>
// <copyright file="INode.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>30/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
namespace GraphFramework.Interfaces
{
    /// <summary>
    /// An interface for a class that is able to hold objects 
    /// of a class that implements the interface IAttribute.
    /// Objects of classes that implement this interface can be added to
    /// objects of classes that implement the interface IGraph.
    /// </summary>
    public interface INode : IGraphElement
    {

    }
}