using System;
using System.Collections.Generic;
using System.Linq;
using GraphFramework;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
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

        private static IEnumerable<IEdge> GetConnectionsOfNode(IGraph graph, INode node)
        {
            return Enumerable.Empty<IEdge>();
        }

        private static void CreateScenario(INode startNode, IGraph currentScenarion, IEnumerable<INode> endNodes, ref IEnumerable<IGraph> scenarios)
        {


            return;
        }

        private static List<IGraph> CreateScenario(INode currentNode, IGraph existingScenario, UseCaseGraph useCaseGraph)
        {
            List<IGraph> retScenario = new List<IGraph>();
            UseCaseGraph internalGraph = new UseCaseGraph(useCaseGraph.Attributes.ToArray());
           
            if (existingScenario != null)
                internalGraph.AddGraph(existingScenario);
            
            if (!useCaseGraph.Nodes.Contains(currentNode))
                return retScenario;

            if (!internalGraph.Nodes.Contains(currentNode))
                internalGraph.AddNode(currentNode);

            //TODO atribute needs to be checked
            //if (currentNode.Attributes.EndNote)
            //{
            //    retScenario.Add(internalGraph);
            //    return retScenario;
            //}

            IEnumerable<IEdge> edges = internalGraph.Edges.Where(edge => edge.Node1 == currentNode ||edge.Node2 == currentNode);
            IList<IEdge> edgeList= edges as IList<IEdge> ?? edges.ToList();
            
            for (int i = 0; i < edgeList.Count(); i++)
            {
                if (!internalGraph.Edges.Contains(edgeList[i]))
                {
                    //TODO this atribute needs to be implemented by the word importer for the direction of the edge
                    //if(! edgesList[i].SourceNode)
                    // continue;
                    INode destNode = edgeList[i].Node1 == currentNode ? edgeList[i].Node1 : edgeList[i].Node2;
                    internalGraph.AddNode(destNode);
                    internalGraph.AddEdge(currentNode, destNode,edgeList[i].Attributes.ToArray());

                    retScenario.AddRange(CreateScenario(destNode,internalGraph,useCaseGraph));

                    //Remove last node 
                    internalGraph.RemoveNode(destNode);
                }

            }

            return retScenario;

        }

        /// <summary>
        /// creates the scenarios from a use case graph
        /// </summary>
        /// <param name="useCaseGraph">use case graph to get its scenarios from</param>
        /// <returns>scenario matrix (as array of graphs --> scenarios)</returns>
        public static IEnumerable<IGraph> CreateScenarios(UseCaseGraph useCaseGraph)
        {
            IEnumerable<IGraph> allScenarios = new List<IGraph>();

            INode startNode = FindStartingNode(useCaseGraph);

            IEnumerable<INode> endNodes = FindEndNodes(useCaseGraph);

            CreateScenario(startNode, null, endNodes, ref allScenarios);
            
            //  EMPTY ENUMERABLE SO THE VIEW CAN BE TESTED WITHOUT CRASHES
            return Enumerable.Empty<IGraph>();
        }



    }
}