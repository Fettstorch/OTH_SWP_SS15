using GraphFramework;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
    public class UseCaseGraph : Graph
    {
        private IGraph[] _mScenarios;

        public IGraph[] Scenarios
        {
            //  lazy initialization of the scenarios
            get { return this._mScenarios ?? (this._mScenarios = ScenarioCreator.CreateScenarios(this)); }
        }
    }
}