#region Copyright information
// <summary>
// <copyright file="Edge.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>30/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using GraphFramework.Interfaces;
using System;

namespace GraphFramework
{
    /// <summary>
    /// The edge class
    /// </summary>
    public class Edge : GraphElement, IEdge
    {
        /// <summary>
        /// The first node the edge connects
        /// </summary>
        public INode Node1 { get; private set; }

        /// <summary>
        /// The second node the edge connects
        /// </summary>
        public INode Node2 { get; private set; }

        /// <summary>
        /// Add also attributes to the edge
        /// </summary>
        /// <param name="node1">first node</param>
        /// <param name="node2">second node</param>
        /// <param name="attributes"></param>
        internal Edge(INode node1, INode node2, params IAttribute[] attributes)
            : base(attributes)
        {
            if (node1 == null) throw new ArgumentNullException("node1");
            if (node2 == null) throw new ArgumentNullException("node2");
            this.Node1 = node1;
            this.Node2 = node2;
        }
    }
}