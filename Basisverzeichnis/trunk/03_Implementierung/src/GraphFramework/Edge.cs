using System.Collections.Generic;
using GraphFramework.Interfaces;

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

        /// <summary>
        /// Edge constructor
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        public Edge(INode node1, INode node2)
        {
            Node1 = node1;
            Node2 = node2;
        }

        /// <summary>
        /// Add also attributes to the edge
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="attributes"></param>
        public Edge(INode node1, INode node2, params IAttribute[] attributes)
            : base(attributes)
        {
            Node1 = node1;
            Node2 = node2;
        }
    }
}