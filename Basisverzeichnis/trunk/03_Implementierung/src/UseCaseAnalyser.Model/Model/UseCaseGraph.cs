using System.Collections.Generic;
using GraphFramework;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
    public class UseCaseGraph : Graph
    {
        private IEnumerable<IGraph> mScenarios;

        public UseCaseGraph(params IAttribute[] attributes)
            : base(attributes)
        {

        }

        public IEnumerable<IGraph> Scenarios
        {
            //  lazy initialization of the scenarios
            get { return mScenarios ?? (mScenarios = ScenarioMatrixCreator.CreateScenarios(this)); }
        }
    }
}