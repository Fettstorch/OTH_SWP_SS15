#region Copyright information
// <summary>
// <copyright file="UseCaseGraph.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>22/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System.Collections.Generic;
using System.Linq;
using GraphFramework;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
    /// <summary>
    /// class to represent a use case. 
    /// </summary>
    public class UseCaseGraph : Graph
    {
        /// <summary>
        /// The attribute names of the graph nodes. You can access this array with the enum NodeAttributes
        /// </summary>
        public static readonly string[] AttributeNames = 
        {
            "Index",
            "Normal Index",
            "Variant Index",
            "Variant Sequnce Step",
            "Description",
            "NodeType"
        };

        /// <summary>
        /// This enum is used to access the attribute names of the string array AttributeNames
        /// </summary>
        public enum NodeAttributes
        {
            /// <summary>
            /// The index [obsulete]
            /// </summary>
            Index = 0,
            /// <summary>
            /// the index of the normal routine, e.g. "2"
            /// </summary>
            NormalIndex,
            /// <summary>
            /// the variant identifier, e.g. "a"
            /// </summary>
            VariantIndex,
            /// <summary>
            /// the variant sequence step, e.g. "1."
            /// </summary>
            VarSeqStep,
            /// <summary>
            /// The description of the node
            /// </summary>
            Description,
            /// <summary>
            /// The type of the node which are start, end, jump nodes, etc.
            /// </summary>
            NodeType
        }

        /// <summary>
        /// The nodes are sorted in their different node types
        /// </summary>
        public enum NodeTypeAttribute
        {
            /// <summary>
            /// The node with which the use case starts
            /// </summary>
            StartNode,
            /// <summary>
            /// A variant sequence node which is connected with a normal routine node
            /// </summary>
            JumpNode,
            /// <summary>
            /// A normal routine node
            /// </summary>
            NormalNode,
            /// <summary>
            /// a variant node
            /// </summary>
            VariantNode,
            /// <summary>
            /// a node which ends the use case
            /// </summary>
            EndNode
        }

        private IEnumerable<IGraph> mScenarios;

        /// <summary>
        /// creates a new use case graph with the given attributes
        /// </summary>
        /// <param name="attributes">attributes to add to the use case graph</param>
        public UseCaseGraph(params IAttribute[] attributes)
            : base(attributes) { }

        /// <summary>
        /// scenarios of the use case graph
        /// lazy initialized when getter is called
        /// </summary>
        public IEnumerable<IGraph> Scenarios
        {
            //  lazy initialization of the scenarios
            get { return mScenarios ?? (mScenarios = /*ScenarioMatrixCreator.CreateScenarios(this)*/ CreateScenarios(this)); }
        }

        /// <summary>
        /// returns the use case graph as a string by returning its name attribute
        /// </summary>
        /// <returns>the use case graph as string</returns>
        public override string ToString()
        {
            return (string) Attributes.Single(a => a.Name == "Name").Value;
        }

        #region BindingTest

        private IEnumerable<IGraph> CreateScenarios(UseCaseGraph useCase)
        {
            int i = 0;
            foreach (INode node in useCase.Nodes)
            {
                Graph scenario = new Graph(new Attribute("Name", string.Format("Scenario No. '{0}' of use case '{1}'", ++i, useCase)));
                scenario.AddNode(node);

                yield return scenario;
            }
        }
        #endregion
    }
}