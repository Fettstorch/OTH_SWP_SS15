using System.Collections.Generic;
using System.Linq;
using GraphFramework;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
    public class UseCaseGraph : Graph
    {
        public static readonly string[] UseCaseNodeAttributeNames = 
        {
            "Index",
            "Normal Index",
            "Variant Index",
            "Variant Sequnce Step",
            "Description",
            "NodeType"
        };

        public enum UseCaseNodeAttributes
        {
            Index = 0,
            NormalIndex,
            VariantIndex,
            VarSeqStep,
            Description,
            NodeType
        }

        public enum NodeTypeAttribute
        {
            StartNode,
            JumpNode,
            NormalNode,
            VariantNode,
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