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
    /// <summary>
    /// class to create the scenarios for a use case graph
    /// </summary>
    public static class ScenarioMatrixCreator
    {
        private const string CUseCase = "Scenario of UseCase";
        private const string COrder = "Order of Visit";
        private const string CScenarioName = "Name";

        private static string ExtendOrderAttribute(string attributeValue, INode nextNode)
        {
            string stepNumber = "";
            string variantName = "";
            string variantNumber = "";

            if (nextNode.Attributes.Any(t => t.Name == UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NormalIndex]))
                stepNumber = nextNode.GetAttributeByName(UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NormalIndex]).Value.ToString();
            if (nextNode.Attributes.Any(t => t.Name == UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.VariantIndex]))
                variantName = nextNode.GetAttributeByName(UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.VariantIndex]).Value.ToString();
            if (nextNode.Attributes.Any(t => t.Name == UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.VarSeqStep]))
                variantNumber = nextNode.GetAttributeByName(UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.VarSeqStep]).Value.ToString();

            return attributeValue + " " + stepNumber + variantName + variantNumber;
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

            if (currentNode == null)
            {
                throw new ArgumentNullException("currentNode");
            }
            if (useCaseGraph == null)
            {
                throw new ArgumentNullException("useCaseGraph");
            }

            //Copy Scenario from last recursion
            if (existingScenario != null)
            {
                internalGraph.AddGraph(existingScenario);
                internalGraph.AddAttribute(existingScenario.Attributes.Any(t => t.Name == COrder)
                    ? existingScenario.GetAttributeByName(COrder) //take Order of visits from existingScenario 
                    : new GraphFramework.Attribute(COrder, "")); // initialize empty new Scenario if existingScenario == new Graph()
            }
            else //initialize empty new Scenario if existingScenario == null
            {
                internalGraph.AddAttribute(new GraphFramework.Attribute(COrder, ""));
            }

            //fault: currentNode not part of useCaseGraph, returns empty List
            if (!useCaseGraph.Nodes.Contains(currentNode))
                return retScenario;

            //include currentNode to Scenario
            if (!internalGraph.Nodes.Contains(currentNode))
            {
                internalGraph.AddNode(currentNode);
                internalGraph.GetAttributeByName(COrder).Value =
                    ExtendOrderAttribute((string)internalGraph.GetAttributeByName(COrder).Value, currentNode);

            }

            //end of recursion, EndNode found
            if (currentNode.GetAttributeByName(UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NodeType]).Value
                    .Equals(UseCaseGraph.NodeTypeAttribute.EndNode))
            {
                retScenario.Add(internalGraph);
                return retScenario;
            }

            //visit all connected Nodes
            IEnumerable<IEdge> edges = useCaseGraph.Edges.Where(edge => edge.Node1 == currentNode ||edge.Node2 == currentNode);
            IList<IEdge> edgeList= edges as IList<IEdge> ?? edges.ToList();
            
            for (int i = 0; i < edgeList.Count(); i++)
            {
                //check if destinationNode is variant and already visited
                if (edgeList[i].Node2.GetAttributeByName(
                    UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NodeType]).Value
                        .Equals(UseCaseGraph.NodeTypeAttribute.VariantNode)
                    ||
                    edgeList[i].Node2.GetAttributeByName(
                    UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.NodeType]).Value
                        .Equals(UseCaseGraph.NodeTypeAttribute.JumpNode))
                {
                    if (internalGraph.Edges.Contains(edgeList[i]))
                        continue;
                }

                if (!edgeList[i].Node1.Equals(currentNode))//SourceNode != currentNode
                    continue;
                INode destNode = edgeList[i].Node2;
                if(!internalGraph.Nodes.Contains(destNode))
                    internalGraph.AddNode(destNode);
                if(!internalGraph.Edges.Contains(edgeList[i]))
                    internalGraph.AddEdge(edgeList[i]);

                //Save Order
                IAttribute orderAttribute = new GraphFramework.Attribute(internalGraph.GetAttributeByName(COrder).Name, 
                    internalGraph.GetAttributeByName(COrder).Value);
                    
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
            
            IEnumerable<IGraph> allScenarios = CreateScenarioMatrix(startNode, new Graph(), useCaseGraph);
            IList<IGraph> scenarioList = allScenarios as IList<IGraph>;

            //Name Scenarios
            if (allScenarios != null)
            {
                string useCaseName = useCaseGraph.GetAttributeByName("Name").Value.ToString();
                int count = 0;
                if (scenarioList != null)
                {
                    count += scenarioList.Count();
                    for (int i = 0; i < count; i++)
                    {
                        scenarioList[i].AddAttribute(new GraphFramework.Attribute(CScenarioName,
                            string.Format("Scenario No. '{0}' of use case '{1}'", i+1, useCaseName)));
                        scenarioList[i].AddAttribute(new GraphFramework.Attribute(CUseCase, useCaseName));
                    }
                }
            }

            return allScenarios;
        }
    }
}