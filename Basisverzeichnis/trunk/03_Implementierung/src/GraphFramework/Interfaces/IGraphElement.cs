using System.Collections.Generic;

namespace GraphFramework.Interfaces
{
    public interface IGraphElement
    {
        /// <summary>
        /// contains all attributes of the graphElement
        /// </summary>
        IEnumerable<IAttribute> Attributes { get; }

        /// <summary>
        /// adds the attribute to the attribute collection of the graphElement
        /// </summary>
        /// <param name="attribute">reference of the attribute to add to the attribute collection</param>
        void AddAttribute(IAttribute attribute);
        
        /// <summary>
        /// removes the attribute identified by its name
        /// </summary>
        /// <param name="nameOfAttributeToRemove">name property of the attribute to remove</param>
        /// <returns>weather the remove was successful</returns>
        void RemoveAttribute(string nameOfAttributeToRemove);

        /// <summary>
        /// remove the attribute identified by its reference
        /// </summary>
        /// <param name="attributeToRemove">reference of the attribute to remove</param>
        /// <returns>weather the remove was successful</returns>
        void RemoveAttribute(IAttribute attributeToRemove);
    }
}