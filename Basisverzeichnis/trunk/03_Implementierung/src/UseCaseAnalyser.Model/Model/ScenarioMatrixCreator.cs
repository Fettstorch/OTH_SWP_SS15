using System;
using System.Collections.Generic;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.Model.Model
{
    public static class ScenarioMatrixCreator
    {
        /// <summary>
        /// creates the scenarios from a use case graph
        /// </summary>
        /// <param name="useCaseGraph">use case graph to get its scenarios from</param>
        /// <returns>scenario matrix (as array of graphs --> scenarios)</returns>
        public static IEnumerable<IGraph> CreateScenarios(UseCaseGraph useCaseGraph)
        {
            throw new NotImplementedException();
        }
    }
}