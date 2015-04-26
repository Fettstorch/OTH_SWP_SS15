public interface IEdge
{
	INode Node1 { get; set; }	// first Node that is connected by Edge
	INode Node2 { get; set; }	// second Node that is connected by Edge
}