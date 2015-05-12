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
        /// <param name="nodes">reference of the nodes to remove</param>
        /// <returns></returns>
        void RemoveNode(params INode[] nodes);

        /// <summary>
        /// adds the edge to the edge collection of the graph
        /// </summary>
        /// <param name="node1">first node to which the edge is connected</param>
        /// <param name="node2">second node to which the edge is connected</param>
        /// <param name="attributes">attributes of the edge</param>
        void AddEdge(INode node1, INode node2, params IAttribute[] attributes);

        /// <summary>
        /// removes the edge identified by its reference from the graph
        /// </summary>
        /// <param name="edges">reference of the edges to remove</param>
        /// <returns></returns>
        void RemoveEdge(params IEdge[] edges);

        /// <summary>
        /// returns all Nodes that are not connected by Edges
        /// </summary>
        /// <returns>returns all single Nodes</returns>
        IEnumerable<INode> GetSingleNodes();


        /// <summary>
        /// adds one Graph object to this graph
        /// </summary>
        /// <param name="graphToAdd">Graph object that will be added to this</param>
        void AddGraph(IGraph graphToAdd);

        /// <summary>
        /// adds one Graph object to this graph and connects it with an edge
        /// </summary>
        /// <param name="graphToAdd">Graph object that will be added to this</param>
        /// <param name="thisGraphConnectionNode">first Node for new Edge</param>
        /// <param name="graphToAddConnectionNode">second Node for new Edge</param>
        /// <param name="attributes">Attributes of new Edge</param>
        void AddGraph(IGraph graphToAdd, INode thisGraphConnectionNode, INode graphToAddConnectionNode, params IAttribute[] attributes);

        /// <summary>
        /// gets the edges which are connected to the nodes node1 and node2
        /// </summary>
        /// <param name="node1">first node to which the edge has to be connected</param>
        /// <param name="node2">second node to which the edge has to be connected</param>
        /// <returns>all the edges which connect the nodes node1 and node2</returns>
        IEnumerable<IEdge> GetEdges(INode node1, INode node2);
    }
}