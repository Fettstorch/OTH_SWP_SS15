using System.Collections.Generic;
using System.Linq;
using GraphFramework;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
    public class UseCaseGraph : Graph
    {
        private IEnumerable<IGraph> mScenarios;

        public UseCaseGraph(string name = null, params IAttribute[] attributes)
            : base(attributes)
        {
            if (name != null)
            {
                AddAttribute(new Attribute("Name", name)); 
            }
        }

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