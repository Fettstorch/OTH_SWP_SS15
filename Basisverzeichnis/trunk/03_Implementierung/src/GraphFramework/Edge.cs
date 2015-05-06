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
        /// The scound node the edge connects
        /// </summary>
        public INode Node2 { get; private set; }

        ///// <summary>
        ///// Edge constructor
        ///// </summary>
        ///// <param name="node1"></param>
        ///// <param name="node2"></param>
        //internal Edge(INode node1, INode node2)
        //{
        //    if (node1 == null || node2 == null) throw new Exception("One node of the edge is null. Wrong edge initialization!");
        //    this.Node1 = node1;
        //    this.Node2 = node2;
        //}

        /// <summary>
        /// Add also attributes to the edge
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="attributes"></param>
        internal Edge(INode node1, INode node2, params IAttribute[] attributes)
            : base(attributes)
        {
            if (node1 == null || node2 == null) throw new Exception("One node of the edge is null. Wrong edge initialization!");
            this.Node1 = node1;
            this.Node2 = node2;
        }
    }
}