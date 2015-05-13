using System;
using System.Collections.Generic;
using GraphFramework.Interfaces;
using System.Linq;

namespace GraphFramework
{
    /// <summary>
    /// the abstract graphelement class nearly every framework class inherits from. The class generalize the relation between classes 
    /// that can have attributes and the attribute class
    /// </summary>
    public abstract class GraphElement : IGraphElement
    {
        /// <summary>
        /// Gets the element's attributes as a collection
        /// </summary>
        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                return mAttributes;
            }
        }

        /// <summary>
        /// The attributes of the element, sorted by their names
        /// </summary>
        protected List<IAttribute> mAttributes;

        /// <summary>
        /// Special constructor which adds multiple attributes to the element
        /// </summary>
        /// <param name="attributes">the attributes you want to add</param>
        protected GraphElement(params IAttribute[] attributes)
        {
            mAttributes = new List<IAttribute>();

            foreach (IAttribute attribute in attributes)
            {
                if (!mAttributes.Any(graphelem => string.Equals(graphelem.Name, attribute.Name)))
                {
                    mAttributes.Add(attribute);
                }
            }
        }

        /// <summary>
        /// Adds an attribute to the node. If the element allready has and attribute with this name, it is not added
        /// </summary>
        /// <param name="attribute">the attribute you want to add</param>
        public void AddAttribute(IAttribute attribute)
        {
            if (!mAttributes.Any(graphelem => string.Equals(graphelem.Name, attribute.Name)))
            {
                mAttributes.Add(attribute);
            }
            else
            {
                throw new InvalidOperationException("the specified attribute is already a attribute of the graphelement!");
            }
        }

        /// <summary>
        /// Removes an attribute with the defined name from the element. (if the element has an attribute with this name)
        /// </summary>
        /// <param name="name">the name of the attribute you want to remove</param>
        public void RemoveAttribute(string name)
        {
            IAttribute attr = mAttributes.Find(graphelem => string.Equals(graphelem.Name, name));
            if (attr != null)
            {
                mAttributes.Remove(attr);
            }
            else
            {
                throw new InvalidOperationException("the defined attribute can not be found!");
            }
        }

        /// <summary>
        /// Removes the defined attribute if the element holds it
        /// </summary>
        /// <param name="attribute"></param>
        public void RemoveAttribute(IAttribute attribute)
        {
            if (!mAttributes.Remove(attribute))
            {
                throw new InvalidOperationException("the defined attribute can not be found!");
            }
        }
    }
}