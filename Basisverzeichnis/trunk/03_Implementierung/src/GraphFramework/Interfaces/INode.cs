namespace GraphFramework.Interfaces
{
    /// <summary>
    /// An interface for a class that is able to hold objects 
    /// of a class that implements the interface IAttribute.
    /// Objects of classes that implement this interface can be added to
    /// objects of classes that implement the interface IGraph.
    /// </summary>
    public interface INode : IGraphElement
    {

    }
}