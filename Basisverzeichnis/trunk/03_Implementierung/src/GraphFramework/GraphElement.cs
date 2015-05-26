#region Copyright information
// <summary>
// <copyright file="GraphElement.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>30/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
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

            if(attributes == null)
                throw new ArgumentNullException();

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
            if(attribute == null) throw new ArgumentNullException();
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
        /// <param name="nameOfAttributeToRemove">the name of the attribute you want to remove</param>
        public void RemoveAttribute(string nameOfAttributeToRemove)
        {
            if (nameOfAttributeToRemove == null) throw new ArgumentNullException();
            IAttribute attr = mAttributes.Find(graphelem => string.Equals(graphelem.Name, nameOfAttributeToRemove));
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
        /// <param name="attributeToRemove"></param>
        public void RemoveAttribute(IAttribute attributeToRemove)
        {
            if (attributeToRemove == null) throw new ArgumentNullException();
            if (!mAttributes.Remove(attributeToRemove))
            {
                throw new InvalidOperationException("the defined attribute can not be found!");
            }
        }

        /// <summary>
        /// Returns the attribute with the given name. Warning: can be also null
        /// </summary>
        /// <param name="name"></param>
        /// <returns>null or the attribute with the given name</returns>
        public IAttribute GetAttributeByName(string name)
        {
            if (name == null) throw new ArgumentNullException();
            return mAttributes.Find(graphelem => string.Equals(graphelem.Name, name));            
        }
    }
}