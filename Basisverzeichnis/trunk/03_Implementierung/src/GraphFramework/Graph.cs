using System;
using System.Collections.Generic;
using System.Linq;
using GraphFramework.Interfaces;

namespace GraphFramework
{
    public class Graph : GraphElement, IGraph
    {
        private readonly List<INode> _nodes;
        private readonly List<IEdge> _edges;

        public Graph(params IAttribute[] attributes) : base(attributes)
        {
            this._nodes = new List<INode>();
            this._edges = new List<IEdge>();
        }

        public IEnumerable<INode> Nodes
        {
            get { return this._nodes; }
        }

        public IEnumerable<IEdge> Edges
        {
            get { return this._edges; }
        }

        public void AddNode(INode node)
        {
             if (this._nodes.Contains(node))
             {
                 throw new InvalidOperationException("The specified node is already part of the graph!");
             }

             //  to remove when removing attributes parameter
             //  addattributes method would be nice in igraphelement or as extension method
             //foreach (IAttribute attribute in attributes)
             //{
             //    node.AddAttribute(attribute);
             //}

             this._nodes.Add(node);
        }

        public void RemoveNode(params INode[] nodes)
        {
            //  if any node can't be removed --> exception
            if (nodes.Any(node => !this._nodes.Remove(node)))
            {
                throw new InvalidOperationException("The specified node is not part of the graph!");
            }
        }

        public void AddEdge(INode n1, INode n2, params IAttribute[] attributes)
         {
             //  add node 1 if it's not part of the graph
             if (!this._nodes.Contains(n1))
             {
                 this._nodes.Add(n1);
             }

             //  add node 2 if it's not part of the graph
             if (!this._nodes.Contains(n2))
             {
                 this._nodes.Add(n2);
             }

             Edge edge = new Edge(n1, n2, attributes);
             ////  addattributes method would be nice in igraphelement or as extension method
             //foreach (var attribute in attributes)
             //{
             //    edge.AddAttribute(attribute);
             //}

             this._edges.Add(edge);
         }
        
        public void RemoveEdge(params IEdge[] edges)
        {
            //  if any edge can't be removed --> exception
            if (edges.Any(edge => !this._edges.Remove(edge)))
            {
                throw new InvalidOperationException("The specified edge is not part of the graph!");
            }
        }

        public IEnumerable<INode> GetSingleNodes()
         {
             //  list of nodes without nodes which are contained in node1 or node2 in the list of edges
             return this._nodes.Except(this._edges.Select(e => e.Node1).Concat(this._edges.Select(e => e.Node2)));
         }

        public void AddGraph(IGraph g2)
         {
            //  no duplicates --> except my nodes / edges
            this._nodes.AddRange(g2.Nodes.Except(this._nodes));
            this._edges.AddRange(g2.Edges.Except(this.Edges));
         }

        public void AddGraph(IGraph g2, INode n1, INode n2, params IAttribute[] attributes)
         {
             //  check if the parameters are valid (n1 in g1, n2 in g2)
             if (!this._nodes.Contains(n1))
             {
                 throw new InvalidOperationException("The specified node n1 is not part of the graph, which is calling the add method!");
             }
             if (!g2.Nodes.Contains(n2))
             {
                 throw new InvalidOperationException("The specified node n2 is not part of the provided graph g2!");
             }

             this.AddGraph(g2);
             //  add a new edge to connect the 2 parts of the graph
             this.AddEdge(n1, n2, attributes);
         }

        public IEnumerable<IEdge> GetEdges(INode n1, INode n2)
        {
            //  edges which contain n1 & n2
            return this._edges.Where(e => (e.Node1 == n1 && e.Node2 == n2) || (e.Node1 == n2 && e.Node2 == n1));
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
