#region Copyright information
// <summary>
// <copyright file="ScenarioMatrixCreator.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>22/04/2015</creationDate>
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
using GraphFramework;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
    [Serializable()]
    public class SourceNodeNotFoundException : System.Exception
    {
        public SourceNodeNotFoundException() : base() { }
        public SourceNodeNotFoundException(string message) : base(message) { }
        public SourceNodeNotFoundException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected SourceNodeNotFoundException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }

    public static class ScenarioMatrixCreator
    {       
        private static INode FindStartingNode(IGraph graph)
        {          
            foreach (INode node in graph.Nodes)
            {
                foreach (IAttribute attribute in node.Attributes)
                {
                    if (!attribute.Name.Equals(
                        UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NodeType]))
                        continue;
                    if (attribute.Value.Equals(UseCaseGraph.NodeTypeAttribute.StartNode))
                    {
                        return node;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<INode> FindEndNodes(IGraph graph)
        {
            List<INode> endNodes = new List<INode>();
            foreach (INode node in graph.Nodes)
            {
                foreach (IAttribute attribute in node.Attributes)
                {
                    if (!attribute.Name.Equals(
                        UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NodeType]))
                        continue;
                    if (attribute.Value.Equals(UseCaseGraph.NodeTypeAttribute.EndNode))
                    {
                        endNodes.Add(node);
                    }
                }
            }
            return endNodes;
        }

        private static INode FindSourceNode(IEdge edge)
        {
            //Todo: Im UseCaseGraph/WordImporter muss die Richtung von edges implementiert werden
            //foreach (INode node in new[] {edge.Node1, edge.Node2})
            //{
            //    foreach (IAttribute attribute in node.Attributes)
            //    {
            //        if (!attribute.Name.Equals(
            //            UseCaseGraph.AttributeNames[(int) UseCaseGraph.NodeAttributes.Direction]))
            //            continue;
            //        if (attribute.Value.Equals(UseCaseGraph.NodeTypeAttribute.SourceNode))
            //        {
            //            return node;
            //        }
            //    }
            //}
            throw new SourceNodeNotFoundException();
        }

        private static List<IGraph> CreateScenario(INode currentNode, IGraph existingScenario, UseCaseGraph useCaseGraph, params INode[] endNodes)
        {
            List<IGraph> retScenario = new List<IGraph>();
            UseCaseGraph internalGraph = new UseCaseGraph(useCaseGraph.Attributes.ToArray());
           
            if (existingScenario != null)
                internalGraph.AddGraph(existingScenario);
            
            if (!useCaseGraph.Nodes.Contains(currentNode))
                return retScenario;

            if (!internalGraph.Nodes.Contains(currentNode))
                internalGraph.AddNode(currentNode);

            if (endNodes.Contains(currentNode))
            {
                retScenario.Add(internalGraph);
                return retScenario;
            }

            IEnumerable<IEdge> edges = internalGraph.Edges.Where(edge => edge.Node1 == currentNode ||edge.Node2 == currentNode);
            IList<IEdge> edgeList= edges as IList<IEdge> ?? edges.ToList();
            
            for (int i = 0; i < edgeList.Count(); i++)
            {
                if (!internalGraph.Edges.Contains(edgeList[i]))
                {
                    if(! currentNode.Equals(FindSourceNode(edgeList[i])))
                     continue;
                    //Todo: Patrick please check if this is now correct, the order of the Nodes was the other way round before
                    INode destNode = edgeList[i].Node1 == currentNode ? edgeList[i].Node2 : edgeList[i].Node1; 
                    internalGraph.AddNode(destNode);
                    internalGraph.AddEdge(currentNode, destNode,edgeList[i].Attributes.ToArray());

                    retScenario.AddRange(CreateScenario(destNode,internalGraph,useCaseGraph, endNodes));

                    //Remove last node 
                    internalGraph.RemoveNode(destNode);
                }

            }

            return retScenario;

        }

        /// <summary>
        /// Creates all scenarios from a Use-Case graph.
        /// </summary>
        /// <param name="useCaseGraph">Use-Case graph to get its scenarios from</param>
        /// <returns>scenario matrix (as array of graphs --> scenarios)</returns>
        public static IEnumerable<IGraph> CreateScenarios(UseCaseGraph useCaseGraph)
        {           
            INode startNode = FindStartingNode(useCaseGraph);

            IEnumerable<INode> endNodes = FindEndNodes(useCaseGraph);

            IEnumerable<IGraph> allScenarios = CreateScenario(startNode, new Graph(), useCaseGraph, endNodes.ToArray());
            
            //  EMPTY ENUMERABLE SO THE VIEW CAN BE TESTED WITHOUT CRASHES
            return Enumerable.Empty<IGraph>();
        }



    }
}