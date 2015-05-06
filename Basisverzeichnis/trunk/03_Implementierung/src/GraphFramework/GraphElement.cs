using System;
using System.Collections.Generic;
using GraphFramework.Interfaces;
using System.Linq;

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
                return mAttributes;
            }
        }

        /// <summary>
        /// The attributes of the node, sorted by their names
        /// </summary>
        protected List<IAttribute> mAttributes;

        ///// <summary>
        ///// Standart constructor
        ///// </summary>
        //protected GraphElement()
        //{
        //    mAttributes = new List<IAttribute>();
        //}

        /// <summary>
        /// Special constructor which adds multiple attributes to the node
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
        /// Adds an attribute to the node. If the node allready has and attribute with this name, it is not added
        /// </summary>
        /// <param name="attribute">the attribute you want to add</param>
        public void AddAttribute(IAttribute attribute)
        {
            if (!mAttributes.Any(graphelem => graphelem.Name == attribute.Name))
            {
                mAttributes.Add(attribute);
            }
            else
            {
                throw new InvalidOperationException("the specified attribute is already a attribute of the graphelement!");
            }
        }

        /// <summary>
        /// Removes an attribute with the defined name from the node. (if the node has an attribute with this name)
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
        /// Removes the defined attribute if the node holds it
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