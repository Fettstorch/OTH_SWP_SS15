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

        public Graph()
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

         //  user can add attributes himself --> remove attributes parameter ?!
         public void AddNode(INode node, params IAttribute[] attributes)
         {
             if (this._nodes.Contains(node))
             {
                 throw new InvalidOperationException("The specified node is already part of the graph!");
             }

             //  to remove when removing attributes parameter
             //  addattributes method would be nice in igraphelement or as extension method
             foreach (var attribute in attributes)
             {
                 node.AddAttribute(attribute);
             }

             this._nodes.Add(node);
         }

         public void RemoveNode(INode node)
         {
             if (!this._nodes.Remove(node))
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

             var edge = new Edge(n1, n2);
             //  addattributes method would be nice in igraphelement or as extension method
             foreach (var attribute in attributes)
             {
                 edge.AddAttribute(attribute);
             }

             this._edges.Add(edge);
         }

         public void RemoveEdge(IEdge edge)
         {
             if (!this._edges.Remove(edge))
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
             this._nodes.AddRange(g2.Nodes);
             this._edges.AddRange(g2.Edges);
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


         #region static methods

         public static IGraph MergeGraphs(IGraph g1, IGraph g2)
         {
             var graph = new Graph();
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

             var graph = Graph.MergeGraphs(g1, g2);
             graph.AddEdge(n1, n2, attributes);

             return graph;
         }

         #endregion
    }
}
