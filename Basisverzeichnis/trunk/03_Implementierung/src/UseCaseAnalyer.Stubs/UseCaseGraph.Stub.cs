using System.Linq;
using GraphFramework;
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyer.Stubs
{
    /// <summary>
    /// Stub for UseCaseGraph (singleton)
    /// </summary>
    public class UseCaseGraphStub
    {
        public UseCaseGraph mDummy1;

        private static UseCaseGraphStub singleReference;

        public UseCaseGraphStub GetInstance()
        {
            UseCaseGraphStub retUseCaseGraphStub;

            if (singleReference != null)
            {
                retUseCaseGraphStub=singleReference;
            }
            else
            {
                singleReference = new UseCaseGraphStub();
                retUseCaseGraphStub = singleReference;
            }

            return retUseCaseGraphStub;
        }


        private UseCaseGraphStub()
        {
            mDummy1 = new UseCaseGraph("DummyUseCase_1");

            mDummy1.AddNode(new Node(new Attribute("Index","1")));
            mDummy1.AddNode(new Node(new Attribute("Index", "2")));
            mDummy1.AddNode(new Node(new Attribute("Index", "3")));
            mDummy1.AddNode(new Node(new Attribute("Index", "4")));

            mDummy1.AddNode(new Node(new Attribute("Index", "2a1")));
            mDummy1.AddNode(new Node(new Attribute("Index", "2a2")));
            mDummy1.AddNode(new Node(new Attribute("Index", "2a3")));

            mDummy1.AddNode(new Node(new Attribute("Index", "2b1")));
            mDummy1.AddNode(new Node(new Attribute("Index", "3a1")));

            mDummy1.AddEdge(SearchNodeByAttributeValue("Index", "1"), SearchNodeByAttributeValue("Index", "2"));
            mDummy1.AddEdge(SearchNodeByAttributeValue("Index", "2"), SearchNodeByAttributeValue("Index", "3"));
            mDummy1.AddEdge(SearchNodeByAttributeValue("Index", "2"), SearchNodeByAttributeValue("Index", "2a1"));
            mDummy1.AddEdge(SearchNodeByAttributeValue("Index", "2a1"), SearchNodeByAttributeValue("Index", "2a2"));
            mDummy1.AddEdge(SearchNodeByAttributeValue("Index", "2a2"), SearchNodeByAttributeValue("Index", "2a3"));

            mDummy1.AddEdge(SearchNodeByAttributeValue("Index", "2a3"), SearchNodeByAttributeValue("Index", "1"));

            mDummy1.AddEdge(SearchNodeByAttributeValue("Index", "2"), SearchNodeByAttributeValue("Index", "2b1"));
            mDummy1.AddEdge(SearchNodeByAttributeValue("Index", "3"), SearchNodeByAttributeValue("Index", "3a1"));
        }

        private INode SearchNodeByAttributeValue(string usecaseIndex, string indexValue)
        {
            return mDummy1.Nodes.FirstOrDefault(node => node.Attributes.Any(attr => attr.Name.Equals(usecaseIndex) && ((string)attr.Value).Equals(indexValue)));
        }
    }
}
