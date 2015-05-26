#region Copyright information
// <summary>
// <copyright file="Graph.cs">Copyright (c) 2015</copyright>
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
using System.Collections.Generic;
using System.Linq;
using GraphFramework.Interfaces;

namespace GraphFramework
{
    /// <summary>
    /// the graph class: holder of edges and nodes + methods to modify the graph
    /// </summary>
    public class Graph : GraphElement, IGraph
    {
        private readonly List<IEdge> mEdges;
        private readonly List<INode> mNodes;

        /// <summary>
        /// creates a new graph with the given parameters
        /// </summary>
        /// <param name="attributes">attributes to add to the graph</param>
        public Graph(params IAttribute[] attributes) : base(attributes)
        {
            mNodes = new List<INode>();
            mEdges = new List<IEdge>();
        }

        /// <summary>
        /// contains all nodes
        /// </summary>
        public IEnumerable<INode> Nodes
        {
            get { return mNodes; }
        }

        /// <summary>
        /// contains all edges
        /// </summary>
        public IEnumerable<IEdge> Edges
        {
            get { return mEdges; }
        }

        /// <summary>
        /// adds the node to the node collection of the graph
        /// </summary>
        /// <param name="node">reference of the node to add</param>
        public void AddNode(INode node)
        {
            CheckForNull(node, "node");
            if (mNodes.Contains(node))
            {
                throw new InvalidOperationException("The specified node is already part of the graph!");
            }

            mNodes.Add(node);
        }

        /// <summary>
        /// removes the nodes identified by its reference from the graph
        /// </summary>
        /// <param name="nodesToRemove">reference of the nodes to remove</param>
        public void RemoveNode(params INode[] nodesToRemove)
        {
            CheckForNull(nodesToRemove, "nodesToRemove");
            //  if any node can't be removed --> exception
            if (nodesToRemove.Any(node => !mNodes.Remove(node)))
            {
                throw new InvalidOperationException("The specified node is not part of the graph!");
            }

            foreach (INode node in nodesToRemove)
            {
                IEdge[] edgesToRemove = mEdges.Where(e => e.Node1 == node || e.Node2 == node).ToArray();
                RemoveEdge(edgesToRemove);
            }
        }

        /// <summary>
        /// adds the edge to the edge collection of the graph
        /// </summary>
        /// <param name="node1">first node to which the edge is connected</param>
        /// <param name="node2">second node to which the edge is connected</param>
        /// <param name="attributes">attributes of the edge</param>
        public void AddEdge(INode node1, INode node2, params IAttribute[] attributes)
        {
            CheckForNull(node1, "node1");
            CheckForNull(node2, "node2");
            CheckForNull(attributes, "attributes");
            //  add node 1 if it's not part of the graph
            AddNodeIfNotExists(node1);

            //  add node 2 if it's not part of the graph
            AddNodeIfNotExists(node2);

            Edge edge = new Edge(node1, node2, attributes);

            mEdges.Add(edge);
        }

        /// <summary>
        /// removes the edge identified by its reference from the graph
        /// </summary>
        /// <param name="edgesToRemove">reference of the edges to remove</param>
        public void RemoveEdge(params IEdge[] edgesToRemove)
        {
            CheckForNull(edgesToRemove, "edgesToRemove");
            //  if any edge can't be removed --> exception
            if (edgesToRemove.Any(edge => !mEdges.Remove(edge)))
            {
                throw new InvalidOperationException("The specified edge is not part of the graph!");
            }
        }

        /// <summary>
        /// returns all Nodes that are not connected by Edges
        /// </summary>
        /// <returns>returns all single Nodes</returns>
        public IEnumerable<INode> GetSingleNodes()
        {
            //  list of nodes without nodes which are contained in node1 or node2 in the list of edges
            return mNodes.Except(mEdges.Select(e => e.Node1).Concat(mEdges.Select(e => e.Node2)));
        }

        /// <summary>
        /// adds one Graph object to this graph
        /// </summary>
        /// <param name="graphToAdd">Graph object that will be added to this</param>
        public void AddGraph(IGraph graphToAdd)
        {
            CheckForNull(graphToAdd, "graphToAdd");
            //  to avoid duplicates --> except my nodes / edges
            mNodes.AddRange(graphToAdd.Nodes.Except(mNodes));
            mEdges.AddRange(graphToAdd.Edges.Except(Edges));
        }

        /// <summary>
        /// adds one Graph object to this graph and connects it with an edge
        /// </summary>
        /// <param name="graphToAdd">Graph object that will be added to this</param>
        /// <param name="thisGraphConnectionNode">first Node for new Edge. Contained in graph 1.</param>
        /// <param name="graphToAddConnectionNode">second Node for new Edge. Contained in graph 2.</param>
        /// <param name="attributes">Attributes of new Edge</param>
        public void AddGraph(IGraph graphToAdd, INode thisGraphConnectionNode, INode graphToAddConnectionNode,
            params IAttribute[] attributes)
        {
            CheckForNull(graphToAdd, "graphToAdd");
            CheckForNull(thisGraphConnectionNode, "thisGraphConnectionNode");
            CheckForNull(graphToAddConnectionNode, "graphToAddConnectionNode");
            CheckForNull(attributes, "attributes");
            //  check if the parameters are valid (thisGraphConnectionNode is part of graph1, graphToAddConnectionNode is part of graphToAdd)
            if (!mNodes.Contains(thisGraphConnectionNode))
            {
                throw new InvalidOperationException(
                    "The specified node node1 is not part of the graph, which is calling the add method!");
            }
            if (!graphToAdd.Nodes.Contains(graphToAddConnectionNode))
            {
                throw new InvalidOperationException(
                    "The specified node node2 is not part of the provided graph graphToAdd!");
            }

            AddGraph(graphToAdd);
            //  add a new edge to connect the 2 parts of the graph
            AddEdge(thisGraphConnectionNode, graphToAddConnectionNode, attributes);
        }

        /// <summary>
        /// gets the edges which are connected to the nodes node1 and node2
        /// </summary>
        /// <param name="node1">first node to which the edge has to be connected</param>
        /// <param name="node2">second node to which the edge has to be connected</param>
        /// <returns>all the edges which connect the nodes node1 and node2</returns>
        public IEnumerable<IEdge> GetEdges(INode node1, INode node2)
        {
            CheckForNull(node1, "node1");
            CheckForNull(node2, "node2");
            //  edges which contain node1 & node2
            return mEdges.Where(e => (e.Node1 == node1 && e.Node2 == node2) || (e.Node1 == node2 && e.Node2 == node1));
        }

        private void AddNodeIfNotExists(INode node)
        {
            if (!mNodes.Contains(node))
            {
                mNodes.Add(node);
            }
        }

        #region static methods

        /// <summary>
        ///     merges the two graphs to one new
        /// </summary>
        /// <param name="graph1">first graph</param>
        /// <param name="graph2">second graph</param>
        /// <returns>the new graph, which contains the given two</returns>
        public static IGraph MergeGraphs(IGraph graph1, IGraph graph2)
        {
            CheckForNull(graph1, "graph1");
            CheckForNull(graph2, "graph2");
            Graph graph = new Graph();
            graph.AddGraph(graph1);
            graph.AddGraph(graph2);

            return graph;
        }

        /// <summary>
        ///     merges the two graphs to one new and connects the two given nodes
        /// </summary>
        /// <param name="graph1">first graph</param>
        /// <param name="graph2">second graph</param>
        /// <param name="graphOneConnectionNode">node in graph one to connect with node in graph two</param>
        /// <param name="graphTwoConnectionNode">node in graph two to connect with node in graph one</param>
        /// <param name="attributes">attributes to add to the edge between graphOneConnectionNode and graphTwoConnectionNode</param>
        /// <returns>the new graph, which contains the given two and a edge between the given nodes</returns>
        public static IGraph MergeGraphs(IGraph graph1, IGraph graph2, INode graphOneConnectionNode,
            INode graphTwoConnectionNode, params IAttribute[] attributes)
        {
            CheckForNull(graph1, "graph1");
            CheckForNull(graph2, "graph2");
            CheckForNull(graphOneConnectionNode, "graphOneConnectionNode");
            CheckForNull(graphTwoConnectionNode, "graphTwoConnectionNode");
            CheckForNull(attributes, "attributes");
            //  check if the parameters are valid (node1 in graph1, node2 in graphToAdd)
            if (!graph1.Nodes.Contains(graphOneConnectionNode))
            {
                throw new InvalidOperationException("The specified node node1 is not part of the provided graph graph1!");
            }
            if (!graph2.Nodes.Contains(graphTwoConnectionNode))
            {
                throw new InvalidOperationException(
                    "The specified node node2 is not part of the provided graph graphToAdd!");
            }

            IGraph graph = MergeGraphs(graph1, graph2);
            graph.AddEdge(graphOneConnectionNode, graphTwoConnectionNode, attributes);

            return graph;
        }

        private static void CheckForNull(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        #endregion
    }
}