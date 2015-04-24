using System.Collections.Generic;

public interface IGraphElement
{
	public List<IAttribute> Attributes { get; }						// contains all Attributes of this GraphElement
	public void addAttribute(IAttribute newAttribute);				// adds an Attribute to Attributes
	public bool removeAttribute(string Name);						// removes Attribute from Attributes identified by Name if found
	public bool removeAttribute(IAttribute attributeToBeRemoved);	// removes Attribute from Attributes if found
}