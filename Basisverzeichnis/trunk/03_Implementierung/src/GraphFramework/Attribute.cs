using System;
using GraphFramework.Interfaces;

namespace GraphFramework
{
    public class Attribute : IAttribute
    {
        public object Value { get; set; }
        public Type Type { get; private set; }
        public string Name { get; set; }
    }
}