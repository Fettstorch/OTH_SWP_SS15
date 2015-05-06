using System.Collections.Generic;

namespace GraphFramework.Interfaces
{
    public interface IGraph : IGraphElement
    {
        /// <summary>
        /// contains all nodes
        /// </summary>
        IEnumerable<INode> Nodes { get; }

        /// <summary>
        /// contains all edges
        /// </summary>
        IEnumerable<IEdge> Edges { get; }

        /// <summary>
        /// adds the node to the node collection of the graph
        /// </summary>
        /// <param name="node">reference of the node to add</param>
        void AddNode(INode node);

        /// <summary>
        /// removes the node identified by its reference from the graph
        /// </summary>
        /// <param name="node">reference of the node to remove</param>
        /// <returns></returns>
        void RemoveNode(params INode[] node);

        /// <summary>
        /// adds the edge to the edge collection of the graph
        /// </summary>
        /// <param name="n1">first node to which the edge is connected</param>
        /// <param name="n2">second node to which the edge is connected</param>
        /// <param name="attributes">attributes of the edge</param>
        void AddEdge(INode n1, INode n2, params IAttribute[] attributes);

        /// <summary>
        /// removes the edge identified by its reference from the graph
        /// </summary>
        /// <param name="edge">reference of the edge to remove</param>
        /// <returns></returns>
        void RemoveEdge(params IEdge[] edge);

        /// <summary>
        /// returns all Nodes that are not connected by Edges
        /// </summary>
        /// <returns>returns all single Nodes</returns>
        IEnumerable<INode> GetSingleNodes();

        ///// <summary>
        ///// creates new Graph object from to Graphs
        ///// </summary>
        ///// <param name="g1">first graph</param>
        ///// <param name="g2">second graph</param>
        ///// <returns>new Graph object</returns>
        //static IGraph MergeGraph(IGraph g1, IGraph g2);

        ///// <summary>
        ///// creates new Graph object from Graphs connected by new Edge
        ///// </summary>
        ///// <param name="g1">first Graph</param>
        ///// <param name="g2">second Graph</param>
        ///// <param name="n1">first Node for new Edge</param>
        ///// <param name="n2">second Node for new Edge</param>
        ///// <param name="attributes">Attributes of new Edge</param>
        ///// <returns>new Graph object</returns>
        //static IGraph Mergeraph(IGraph g1, IGraph g2, INode n1, INode n2, params IAttribute[] attributes);

        /// <summary>
        /// adds one Graph object to this graph
        /// </summary>
        /// <param name="g2">Graph object that will be added to this</param>
        void AddGraph(IGraph g2);

        /// <summary>
        /// adds one Graph object to this graph and connects it with an edge
        /// </summary>
        /// <param name="g2">Graph object that will be added to this</param>
        /// <param name="n1">first Node for new Edge</param>
        /// <param name="n2">second Node for new Edge</param>
        /// <param name="attributes">Attributes of new Edge</param>
        void AddGraph(IGraph g2, INode n1, INode n2, params IAttribute[] attributes);

        /// <summary>
        /// gets the edges which are connected to the nodes n1 and n2
        /// </summary>
        /// <param name="n1">first node to which the edge has to be connected</param>
        /// <param name="n2">second node to which the edge has to be connected</param>
        /// <returns>all the edges which connect the nodes n1 and n2</returns>
        IEnumerable<IEdge> GetEdges(INode n1, INode n2);
    }
}