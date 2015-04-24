public interface IAttribute
{
	public object Value { get; set; }		// value of Attribute
	public System.Type Type { get; set; }	// type of Value
	public string Name { get; set; }		// name to identify Attribute
}