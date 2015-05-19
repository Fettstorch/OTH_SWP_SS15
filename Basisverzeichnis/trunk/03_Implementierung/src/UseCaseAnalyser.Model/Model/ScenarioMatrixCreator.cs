using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
                        WordImporter.UseCaseNodeAttributeNames[(int) WordImporter.UseCaseNodeAttributes.NodeType]))
                        continue;
                    if (attribute.Value.Equals(WordImporter.NodeTypeAttribute.StartNode))
                    {
                        return node;
                    }
                }
            }
        }

        private static IEnumerable<INode> FindEndNodes(IGraph graph)
        {
            List<INode> endNodes = new List<INode>();
            foreach (INode node in graph.Nodes)
            {
                foreach (IAttribute attribute in node.Attributes)
                {
                    if (!attribute.Name.Equals(
                        WordImporter.UseCaseNodeAttributeNames[(int)WordImporter.UseCaseNodeAttributes.NodeType]))
                        continue;
                    if (attribute.Value.Equals(WordImporter.NodeTypeAttribute.EndNode))
                    {
                        endNodes.Add(node);
                    }
                }
            }
            return endNodes;
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

            foreach (INode endNode in endNodes)
            {
                //find all paths from startNode
            }
            
            //  EMPTY ENUMERABLE SO THE VIEW CAN BE TESTED WITHOUT CRASHES
            return Enumerable.Empty<IGraph>();
        }
    }
}