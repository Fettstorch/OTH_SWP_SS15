#region Copyright information
// <summary>
// <copyright file="IGraph.cs">Copyright (c) 2015</copyright>
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
    /// Classes that implement this interface are 
    /// a representation of a set of objects that implement 
    /// the interfaces INode and IEdge and the relations 
    /// between them. It is able to
    /// perform actions on this objects. 
    /// </summary>
    public interface IGraph : IGraphElement
    {
        /// <summary>
        /// Contains all INode objects that are part of the IGraph.
        /// </summary>
        IEnumerable<INode> Nodes { get; }

        /// <summary>
        /// Contains all IEdge objects that are part of the IGraph.
        /// </summary>
        IEnumerable<IEdge> Edges { get; }

        /// <summary>
        /// Adds the INode to property Nodes of the IGraph.
        /// </summary>
        /// <param name="node">reference of the INode to add</param>
        void AddNode(INode node);

        /// <summary>
        /// Removes the INode from the property Nodes of the IGraph.
        /// </summary>
        /// <param name="nodesToRemove">reference of the INode objects to remove</param>
        void RemoveNode(params INode[] nodesToRemove);

        /// <summary>
        /// Adds the IEdge to the property Edges of the IGraph.
        /// </summary>
        /// <param name="node1">first INode to which the IEdge is connected</param>
        /// <param name="node2">second INode to which the IEdge is connected</param>
        /// <param name="attributes">IAttribute objects of the IEdge</param>
        void AddEdge(INode node1, INode node2, params IAttribute[] attributes);

        /// <summary>
        /// Removes the IEdge from the property Edges of the IGraph.
        /// </summary>
        /// <param name="edgesToRemove">reference of the IEdge objects to remove</param>
        void RemoveEdge(params IEdge[] edgesToRemove);

        /// <summary>
        /// Returns all INode objects of this IGraph that are not connected by any IEdges.
        /// </summary>
        /// <returns>returns all single INode objects</returns>
        IEnumerable<INode> GetSingleNodes();


        /// <summary>
        /// Adds one IGraph object to this IGraph.
        /// </summary>
        /// <param name="graphToAdd">IGraph object that will be added to this</param>
        void AddGraph(IGraph graphToAdd);

        /// <summary>
        /// Adds one IGraph object to this IGraph and connects it with an IEdge.
        /// </summary>
        /// <param name="graphToAdd">IGraph object that will be added to this</param>
        /// <param name="thisGraphConnectionNode">first INode for new IEdge</param>
        /// <param name="graphToAddConnectionNode">second INode for new IEdge</param>
        /// <param name="attributes">IAttribute objects of new IEdge</param>
        void AddGraph(IGraph graphToAdd, INode thisGraphConnectionNode, INode graphToAddConnectionNode, params IAttribute[] attributes);

        /// <summary>
        /// Gets the IEdge objects which are connected to the INode obejcts node1 and node2.
        /// </summary>
        /// <param name="node1">first INode to which the IEdge has to be connected</param>
        /// <param name="node2">second INode to which the IEdge has to be connected</param>
        /// <returns>all the IEdge objects which connect the INode objects node1 and node2</returns>
        IEnumerable<IEdge> GetEdges(INode node1, INode node2);
    }
}