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
using LogManager;
using Attribute = GraphFramework.Attribute;

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

        private static string GetNodeNumber(INode node)
        {
            string nodeNumber = "";
            if (node.Attributes.Any(t => t.Name == NodeAttributes.NormalIndex.AttributeName()))
                nodeNumber += node.GetAttributeByName(NodeAttributes.NormalIndex.AttributeName()).Value.ToString();
            else
            {
                LoggingFunctions.Exception("Invalid Node found while creating ScenarioMatrix.");
                throw new NullReferenceException("No NodeIndex found.");
            }
            if (node.Attributes.Any(t => t.Name == NodeAttributes.VariantIndex.AttributeName()))
                nodeNumber += node.GetAttributeByName(NodeAttributes.VariantIndex.AttributeName()).Value.ToString();
            if (node.Attributes.Any(t => t.Name == NodeAttributes.VarSeqStep.AttributeName()))
                nodeNumber += node.GetAttributeByName(NodeAttributes.VarSeqStep.AttributeName()).Value.ToString();
            return nodeNumber;

        }

        private static string ExtendOrderAttribute(string attributeValue, INode nextNode, UseCaseGraph useCaseGraph, IEdge correspondingEdge = null)
        {           
            IEnumerable<IEdge> edges = useCaseGraph.Edges.Where(edge => edge.Node2 == nextNode);
            IEdge[] edgeArray = correspondingEdge == null ? edges as IEdge[] ?? edges.ToArray() : new[]{correspondingEdge};

            string seperator = (string.IsNullOrEmpty(attributeValue) ? string.Empty : " ");
            if (IsAlternativeNode(nextNode))
            {
                if (edgeArray.Any(edge => !IsAlternativeNode(edge.Node1)))
                {
                    seperator = "\r\n  ";
                }                
            }
            else
            {
                if (edgeArray.Any(edge => IsAlternativeNode(edge.Node1)))
                {
                    seperator = "\r\n";
                }  
            }
            return attributeValue + seperator + GetNodeNumber(nextNode);
        }

        private static INode FindStartNode(UseCaseGraph graph)
        {
            return
                graph.Nodes.FirstOrDefault(
                    node =>
                        node.GetAttributeByName(NodeAttributes.NodeType.AttributeName())
                            .Value.Equals(UseCaseGraph.NodeTypeAttribute.StartNode) || graph.Edges.All(edge => edge.Node2 != node));
        }

        private static bool IsEndNode(INode node, UseCaseGraph useCaseGraph)
        {
            return node.GetAttributeByName(NodeAttributes.NodeType.AttributeName()).Value
                .Equals(UseCaseGraph.NodeTypeAttribute.EndNode) || useCaseGraph.Edges.All(edge => edge.Node1 != node);
        }

        private static bool IsAlternativeNode(INode node)
        {
            return node.GetAttributeByName(NodeAttributes.NodeType.AttributeName())
                .Value.Equals(UseCaseGraph.NodeTypeAttribute.VariantNode)
                   ||
                   node.GetAttributeByName(NodeAttributes.NodeType.AttributeName())
                       .Value.Equals(UseCaseGraph.NodeTypeAttribute.JumpNode)
            || (node.Attributes.Any(attr1 => attr1.Name.Equals(NodeAttributes.VariantIndex.AttributeName())));
        }

        private static int CountVariants(IGraph graph)
        {
            return graph.Edges.Count(IsVariantEntry);
        }

        private static bool IsVariantEntry(IEdge edge)
        {
            return !IsAlternativeNode(edge.Node1) && IsAlternativeNode(edge.Node2);
        }

        private static IEnumerable<IGraph> CreateScenarioMatrix(INode currentNode, IGraph existingScenario, UseCaseGraph useCaseGraph, 
            int maxVariantTraversions, IDictionary<IEdge,int> numLoopTraversions,  int maxLoopTraversions)
        {
            List<IGraph> retScenario = new List<IGraph>();
            UseCaseGraph internalGraph = new UseCaseGraph();
            IDictionary<IEdge, int> variantTraversions = new Dictionary<IEdge, int>();
            if (currentNode == null)
            {
                LoggingFunctions.Exception("Invalid Function call while creating ScenarioMatrix.");
                throw new ArgumentNullException("currentNode");
            }
            if (useCaseGraph == null)
            {
                LoggingFunctions.Exception("Invalid Function call while creating ScenarioMatrix.");
                throw new ArgumentNullException("useCaseGraph");
            }
            if (numLoopTraversions != null)
            {
                foreach (KeyValuePair<IEdge, int> value in numLoopTraversions)
                {
                    variantTraversions.Add(value.Key, value.Value);
                }
            }

            //Copy Scenario from last recursion
            if (existingScenario != null)
            {
                internalGraph.AddGraph(existingScenario);
                internalGraph.AddAttribute(existingScenario.Attributes.Any(t => t.Name == COrder)
                    ? existingScenario.GetAttributeByName(COrder) //take Order of visits from existingScenario 
                    : new Attribute(COrder, "")); // initialize empty new Scenario if existingScenario == new Graph()
            }
            else //initialize empty new Scenario if existingScenario == null
            {
                internalGraph.AddAttribute(new Attribute(COrder, ""));
            }

            //fault: currentNode not part of useCaseGraph, returns empty List
            if (!useCaseGraph.Nodes.Contains(currentNode))
                return retScenario;

            //include currentNode to Scenario
            if (!internalGraph.Nodes.Contains(currentNode))
            {
                internalGraph.AddNode(currentNode);
                internalGraph.GetAttributeByName(COrder).Value =
                    ExtendOrderAttribute((string)internalGraph.GetAttributeByName(COrder).Value, currentNode, internalGraph);
            }

            //end of recursion, EndNode found
            if (IsEndNode(currentNode, useCaseGraph))
            {
                retScenario.Add(internalGraph);
                //check if there are edges from this endNode that need to be traversed
                IEnumerable<IEdge> edgesFromEndNode = useCaseGraph.Edges.Where(edge => edge.Node1.Equals(currentNode));
                IEdge[] fromEndNode = edgesFromEndNode as IEdge[] ?? edgesFromEndNode.ToArray();
                foreach (IEdge edge in fromEndNode.Where(edge => !variantTraversions.ContainsKey(edge)))
                {
                    variantTraversions.Add(edge, 0);
                }
                IEnumerable<IEdge> validEdgesFromeEndNode =
                    fromEndNode.Where(edge => variantTraversions[edge] < maxLoopTraversions);

                if(!validEdgesFromeEndNode.Any())
                    return retScenario;
            }

            //Save old Scenario for comparison
            IGraph saveGraph = new Graph(internalGraph.Attributes.ToArray());
            saveGraph.AddGraph(internalGraph);

            //visit all connected Nodes
            IEnumerable<IEdge> edges = useCaseGraph.Edges.Where(edge => edge.Node1 == currentNode);
            IList<IEdge> edgeList = edges as IList<IEdge> ?? edges.ToList();
            
            for (int i = 0; i < edgeList.Count(); i++)
            {
                if (!edgeList[i].Node1.Equals(currentNode))//SourceNode != currentNode
                    continue;

                //check for maximum number of loop traversions
                if (IsVariantEntry(edgeList[i]))
                {
                    if (!variantTraversions.ContainsKey(edgeList[i]))
                    {
                        variantTraversions.Add(edgeList[i], 0);
                    }
                    variantTraversions[edgeList[i]]++;
                    if (variantTraversions[edgeList[i]] > maxLoopTraversions)
                        continue;
                }

                //check for maximum number of variant traversions
                if (CountVariants(internalGraph) > maxVariantTraversions &&
                    !(variantTraversions.ContainsKey(edgeList[i])))
                    continue;

                //Add next Node 
                INode destNode = edgeList[i].Node2;
                if(!internalGraph.Nodes.Contains(destNode))
                    internalGraph.AddNode(destNode);
                if(!internalGraph.Edges.Contains(edgeList[i]))
                    internalGraph.AddEdge(edgeList[i]);

                //Save Order
                IAttribute orderAttribute = new Attribute(internalGraph.GetAttributeByName(COrder).Name, 
                    internalGraph.GetAttributeByName(COrder).Value);
                    
                //Set Order for recursive call
                internalGraph.GetAttributeByName(COrder).Value =
                    ExtendOrderAttribute((string)internalGraph.GetAttributeByName(COrder).Value, destNode, internalGraph, edgeList[i]);

                retScenario.AddRange(CreateScenarioMatrix(destNode, internalGraph, useCaseGraph, maxVariantTraversions, variantTraversions, maxLoopTraversions));

                //Restore Variant Counter
                if(IsVariantEntry(edgeList[i]))
                    variantTraversions[edgeList[i]]--;

                //Restore Order
                internalGraph.RemoveAttribute(COrder);
                internalGraph.AddAttribute(orderAttribute);

                //Remove last node if necessary
                if(!saveGraph.Nodes.Contains(edgeList[i].Node2))
                {
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
            //get information about use case           
            if (useCaseGraph == null)
            {
                throw new ArgumentNullException("useCaseGraph");
            }

            string useCaseName = "<no name found>";
            if (useCaseGraph.Attribute(UseCaseAttributes.Name, false) != null)
            {
                useCaseName = useCaseGraph.GetAttributeByName("Name").Value.ToString();
            }

            int traverseVariantCount = useCaseGraph.Attribute(UseCaseAttributes.TraverseVariantCount, false) != null
                ? useCaseGraph.AttributeValue<int>(UseCaseAttributes.TraverseVariantCount)
                : CountVariants(useCaseGraph);

            int traverseLoopCount = useCaseGraph.Attribute(UseCaseAttributes.TraverseLoopCount, false) != null
                ? useCaseGraph.AttributeValue<int>(UseCaseAttributes.TraverseLoopCount) : 1;

            INode startNode = FindStartNode(useCaseGraph);
            if (startNode == null)
            {
                LoggingFunctions.Status(string.Format("No StartNode found in {0}.", useCaseName));
                return Enumerable.Empty<IGraph>();
            }

            foreach (INode node in useCaseGraph.Nodes)
            {
                if (
                    node.Attributes.Any(
                        t => t.Name == NodeAttributes.NodeType.AttributeName()))
                    continue;
                LoggingFunctions.Exception(string.Format("Invalid Nodes in {0}.", useCaseName));
                try
                {                   
                    throw new InvalidOperationException(string.Format("No NodeType attribute found at node {0}",
                        GetNodeNumber(node)));
                }
                catch (NullReferenceException)
                {
                    throw new InvalidOperationException("No NodeType and no NormalIndex attribute found");
                }
            }

            IEnumerable<IGraph> allScenarios = CreateScenarioMatrix(startNode, new Graph(), useCaseGraph, traverseVariantCount, null, traverseLoopCount);

            //name scenarios
            IGraph[] returnScenarios = allScenarios as IGraph[] ?? allScenarios.ToArray();
            int count = returnScenarios.Count();
            for (int i = 0; i < count; i++)
            {
                returnScenarios[i].AddAttribute(new Attribute(CScenarioName,
                    string.Format("Scenario No. '{0}' of use case '{1}'", i + 1, useCaseName)));
                returnScenarios[i].AddAttribute(new Attribute(CUseCase, useCaseName));
            }

            LoggingFunctions.Trace(string.Format("Scenarios of {0} were generated successfully.", useCaseName));

            return returnScenarios;
        }
    }
}