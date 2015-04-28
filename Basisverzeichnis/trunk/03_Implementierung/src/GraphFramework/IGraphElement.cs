using System.Collections.Generic;

namespace GraphFramework
{
    public interface IGraphElement
    {
        /// <summary>
        /// contains all attributes of the graphElement
        /// </summary>
        List<IAttribute> Attributes { get; }

        /// <summary>
        /// adds the attribute to the attribute collection of the graphElement
        /// </summary>
        /// <param name="newAttribute">reference of the attribute to add to the attribute collection</param>
        void AddAttribute(IAttribute newAttribute);
        
        /// <summary>
        /// removes the attribute identified by its name
        /// </summary>
        /// <param name="name">name property of the attribute to remove</param>
        /// <returns>weather the remove was successful</returns>
        bool RemoveAttribute(string name);

        /// <summary>
        /// remove the attribute identified by its reference
        /// </summary>
        /// <param name="attributeToBeRemoved">reference of the attribute to remove</param>
        /// <returns>weather the remove was successful</returns>
        bool RemoveAttribute(IAttribute attributeToBeRemoved);
    }
}