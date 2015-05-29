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
    //[Serializable]
    //public class SourceNodeNotFoundException : Exception
    //{
    //    public SourceNodeNotFoundException() { }
    //    public SourceNodeNotFoundException(string message) : base(message) { }
    //    public SourceNodeNotFoundException(string message, Exception inner) : base(message, inner) { }

    //    // A constructor is needed for serialization when an
    //    // exception propagates from a remoting server to the client. 
    //    protected SourceNodeNotFoundException(System.Runtime.Serialization.SerializationInfo info,
    //        System.Runtime.Serialization.StreamingContext context) { }
    //}

    /// <summary>
    /// class to create the scenarios for a use case graph
    /// </summary>
    public static class ScenarioMatrixCreator
    {
        private const string COrder = "Order of Visit";

        private static string ExtendOrderAttribute(string attributeValue, INode nextNode)
        {
            return attributeValue + nextNode.GetAttributeByName(
                        UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NormalIndex]);
        }

        private static INode FindStartNode(IGraph graph)
        {
            return
                graph.Nodes.FirstOrDefault(
                    node =>
                        node.GetAttributeByName(UseCaseGraph.AttributeNames[(int) UseCaseGraph.NodeAttributes.NodeType])
                            .Value.Equals(UseCaseGraph.NodeTypeAttribute.StartNode));
        }

        private static IEnumerable<IGraph> CreateScenarioMatrix(INode currentNode, IGraph existingScenario, UseCaseGraph useCaseGraph)
        {
            List<IGraph> retScenario = new List<IGraph>();
            UseCaseGraph internalGraph = new UseCaseGraph();

            if (existingScenario != null)
            {
                internalGraph.AddGraph(existingScenario);
                internalGraph.AddAttribute(existingScenario.GetAttributeByName(COrder));
            }

            if (!useCaseGraph.Nodes.Contains(currentNode))
                return retScenario;

            if (!internalGraph.Nodes.Contains(currentNode))
            {
                internalGraph.AddNode(currentNode);
                internalGraph.GetAttributeByName(COrder).Value =
                    ExtendOrderAttribute((string)internalGraph.GetAttributeByName(COrder).Value, currentNode);

            }

            if (currentNode.GetAttributeByName(UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NodeType]).Value
                    .Equals(UseCaseGraph.NodeTypeAttribute.EndNode))
            {
                retScenario.Add(internalGraph);
                return retScenario;
            }

            IEnumerable<IEdge> edges = internalGraph.Edges.Where(edge => edge.Node1 == currentNode ||edge.Node2 == currentNode);
            IList<IEdge> edgeList= edges as IList<IEdge> ?? edges.ToList();
            
            for (int i = 0; i < edgeList.Count(); i++)
            {
                if (!internalGraph.Edges.Contains(edgeList[i]))//passt so nicht kann mehrmals vorkommen
                {
                    if (!edgeList[i].GetAttributeByName("SourceNode").Value.Equals(currentNode))//Änderung in UseCaseGraph und WordImporter fehlt
                        continue;
                    INode destNode = edgeList[i].Node1 == currentNode ? edgeList[i].Node2 : edgeList[i].Node1;
                    internalGraph.AddNode(destNode);
                    internalGraph.AddEdge(edgeList[i]);

                    //Save Order
                    IAttribute orderAttribute = new GraphFramework.Attribute(internalGraph.GetAttributeByName(COrder).Name, internalGraph.GetAttributeByName(COrder).Value);
                    
                    //Set Order for recursive call
                    internalGraph.GetAttributeByName(COrder).Value =
                    ExtendOrderAttribute((string)internalGraph.GetAttributeByName(COrder).Value, destNode);

                    retScenario.AddRange(CreateScenarioMatrix(destNode,internalGraph,useCaseGraph));

                    //Restore Order
                    internalGraph.RemoveAttribute(COrder);
                    internalGraph.AddAttribute(orderAttribute);

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
            if (useCaseGraph == null)
            {
                throw new ArgumentNullException("useCaseGraph");
            }

            INode startNode = FindStartNode(useCaseGraph);
            if (startNode == null)
            {
                throw new InvalidOperationException("No StartNode found.");
            }
            
            return CreateScenarioMatrix(startNode, new Graph(), useCaseGraph);
        }



    }
}