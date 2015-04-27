using System.Collections.Generic;

namespace GraphFramework
{
    public interface IGraphElement
    {
        List<IAttribute> Attributes { get; }					// contains all Attributes of this GraphElement
        void addAttribute(IAttribute newAttribute);				// adds an Attribute to Attributes
        bool removeAttribute(string Name);						// removes Attribute from Attributes identified by Name if found
        bool removeAttribute(IAttribute attributeToBeRemoved);	// removes Attribute from Attributes if found
    }
}