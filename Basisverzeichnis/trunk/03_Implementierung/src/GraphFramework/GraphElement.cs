using System;
using System.Collections.Generic;
using GraphFramework.Interfaces;

namespace GraphFramework
{
    public class GraphElement : IGraphElement
    {
        public IEnumerable<IAttribute> Attributes { get; private set; }
        public void AddAttribute(IAttribute attribute)
        {
            throw new NotImplementedException();
        }

        public void RemoveAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public void RemoveAttribute(IAttribute attribute)
        {
            throw new NotImplementedException();
        }
    }
}