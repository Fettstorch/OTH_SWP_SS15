namespace GraphFramework
{
    public interface IAttribute
    {
        object Value { get; set; }		// value of Attribute
        System.Type Type { get; set; }	// type of Value
        string Name { get; set; }		// name to identify Attribute
    }
}