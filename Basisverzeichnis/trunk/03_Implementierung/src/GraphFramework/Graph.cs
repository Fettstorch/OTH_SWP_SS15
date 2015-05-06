using System;
using System.Collections.Generic;
using System.Linq;
using GraphFramework.Interfaces;

namespace GraphFramework
{
    public class Graph : GraphElement, IGraph
    {
        private readonly List<INode> mNodes;
        private readonly List<IEdge> mEdges;

        public Graph(params IAttribute[] attributes) : base(attributes)
        {
            mNodes = new List<INode>();
            mEdges = new List<IEdge>();
        }

        public IEnumerable<INode> Nodes
        {
            get { return mNodes; }
        }

        public IEnumerable<IEdge> Edges
        {
            get { return mEdges; }
        }

        public void AddNode(INode node)
        {
             if (mNodes.Contains(node))
             {
                 throw new InvalidOperationException("The specified node is already part of the graph!");
             }

             //  to remove when removing attributes parameter
             //  addattributes method would be nice in igraphelement or as extension method
             //foreach (IAttribute attribute in attributes)
             //{
             //    node.AddAttribute(attribute);
             //}

             mNodes.Add(node);
        }

        public void RemoveNode(params INode[] nodesToRemove)
        {
            //  if any node can't be removed --> exception
            if (nodesToRemove.Any(node => !mNodes.Remove(node)))
            {
                throw new InvalidOperationException("The specified node is not part of the graph!");
            }
        }

        public void AddEdge(INode n1, INode n2, params IAttribute[] attributes)
         {
             //  add node 1 if it's not part of the graph
             if (!mNodes.Contains(n1))
             {
                 mNodes.Add(n1);
             }

             //  add node 2 if it's not part of the graph
             if (!mNodes.Contains(n2))
             {
                 mNodes.Add(n2);
             }

             Edge edge = new Edge(n1, n2, attributes);
             ////  addattributes method would be nice in igraphelement or as extension method
             //foreach (var attribute in attributes)
             //{
             //    edge.AddAttribute(attribute);
             //}

             mEdges.Add(edge);
         }
        
        public void RemoveEdge(params IEdge[] edgesToRemove)
        {
            //  if any edge can't be removed --> exception
            if (edgesToRemove.Any(edge => !mEdges.Remove(edge)))
            {
                throw new InvalidOperationException("The specified edge is not part of the graph!");
            }
        }

        public IEnumerable<INode> GetSingleNodes()
         {
             //  list of nodes without nodes which are contained in node1 or node2 in the list of edges
             return mNodes.Except(mEdges.Select(e => e.Node1).Concat(mEdges.Select(e => e.Node2)));
         }

        public void AddGraph(IGraph g2)
         {
            //  no duplicates --> except my does / edges
            mNodes.AddRange(g2.Nodes.Except(mNodes));
            mEdges.AddRange(g2.Edges.Except(Edges));
         }

        public void AddGraph(IGraph g2, INode n1, INode n2, params IAttribute[] attributes)
         {
             //  check if the parameters are valid (n1 in g1, n2 in g2)
             if (!mNodes.Contains(n1))
             {
                 throw new InvalidOperationException("The specified node n1 is not part of the graph, which is calling the add method!");
             }
             if (!g2.Nodes.Contains(n2))
             {
                 throw new InvalidOperationException("The specified node n2 is not part of the provided graph g2!");
             }

             AddGraph(g2);
             //  add a new edge to connect the 2 parts of the graph
             AddEdge(n1, n2, attributes);
         }

        public IEnumerable<IEdge> GetEdges(INode n1, INode n2)
        {
            //  edges which contain n1 & n2
            return mEdges.Where(e => (e.Node1 == n1 && e.Node2 == n2) || (e.Node1 == n2 && e.Node2 == n1));
        }

        #region static methods

        public static IGraph MergeGraphs(IGraph g1, IGraph g2)
         {
             Graph graph = new Graph();
             graph.AddGraph(g1);
             graph.AddGraph(g2);

             return graph;
         }

        public static IGraph MergeGraphs(IGraph g1, IGraph g2, INode n1, INode n2, params IAttribute[] attributes)
         {
             //  check if the parameters are valid (n1 in g1, n2 in g2)
             if (!g1.Nodes.Contains(n1))
             {
                 throw new InvalidOperationException("The specified node n1 is not part of the provided graph g1!");
             }
             if (!g2.Nodes.Contains(n2))
             {
                 throw new InvalidOperationException("The specified node n2 is not part of the provided graph g2!");
             }

             IGraph graph = MergeGraphs(g1, g2);
             graph.AddEdge(n1, n2, attributes);

             return graph;
         }

        #endregion
    }
}
