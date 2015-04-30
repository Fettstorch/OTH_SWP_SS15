using System.Collections.Generic;
using GraphFramework.Interfaces;

namespace GraphFramework
{
    public abstract class GraphElement : IGraphElement
    {
        /// <summary>
        /// Gets the node's attributes as a collection
        /// </summary>
        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                return MAttributes.Values;
            }
        }

        /// <summary>
        /// The attributes of the node, sorted by their names
        /// </summary>
        protected Dictionary<string, IAttribute> MAttributes;

        /// <summary>
        /// Standart constructor
        /// </summary>
        protected GraphElement()
        {
            MAttributes = new Dictionary<string, IAttribute>();
        }

        /// <summary>
        /// Special constructor which adds multiple attributes to the node
        /// </summary>
        /// <param name="attributes">the attributes you want to add</param>
        protected GraphElement(params IAttribute[] attributes)
        {
            MAttributes = new Dictionary<string, IAttribute>();

            foreach (var attribute in attributes)
            {
                MAttributes.Add(attribute.Name, attribute);
            }
        }

        /// <summary>
        /// Adds an attribute to the node. If the node allready has and attribute with this name, it is not added
        /// </summary>
        /// <param name="attribute">the attribute you want to add</param>
        public void AddAttribute(IAttribute attribute)
        {
            if (attribute == null) return;
            if (!MAttributes.ContainsKey(attribute.Name))
            {
                MAttributes.Add(attribute.Name, attribute);
            }
        }

        /// <summary>
        /// Removes an attribute with the defined name from the node. (if the node has an attribute with this name)
        /// </summary>
        /// <param name="name">the name of the attribute you want to remove</param>
        public void RemoveAttribute(string name)
        {
            if (MAttributes.ContainsKey(name))
            {
                MAttributes.Remove(name);
            }
        }

        /// <summary>
        /// Removes the defined attribute if the node holds it
        /// </summary>
        /// <param name="attribute"></param>
        public void RemoveAttribute(IAttribute attribute)
        {
            if (MAttributes.ContainsValue(attribute))
            {
                MAttributes.Remove(attribute.Name);
            }
        }
    }
}