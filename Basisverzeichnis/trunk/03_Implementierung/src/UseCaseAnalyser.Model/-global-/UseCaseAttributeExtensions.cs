using System;
using System.Collections.Generic;
using System.Linq;
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;
using Attribute = GraphFramework.Attribute;

// ReSharper disable once CheckNamespace
/// <summary>
/// extension methods for easier attribute access
/// </summary>
public static class UseCaseAttributeExtensions
{
    /// <summary>
    /// gets the attribute value represented by the given usecasegraph attribute type
    ///     throws 'KeyNotFoundException' if the attribute is not contained in the source item's attributes
    /// </summary>
    /// <param name="source">the node which contains the attributes</param>
    /// <param name="useCaseAttribute">the attribute type which should be read</param>
    /// <returns>the attribute value of type T</returns>
    public static T AttributeValue<T>(this UseCaseGraph source, UseCaseAttributes useCaseAttribute)
    {
        IAttribute attribute = source.Attribute(useCaseAttribute);
        if (typeof (T) != attribute.Type)
        {
            throw new NotSupportedException(string.Format("The given type parameter '{0}' is not the type of the attribute.", typeof(T).Name));
        }

        return (T) attribute.Value;
    }

    /// <summary>
    /// gets the attribute value represented by the given nodeAttribute type 
    ///     throws 'KeyNotFoundException' if the attribute is not contained in the source item's attributes
    /// </summary>
    /// <param name="source">the node which contains the attributes</param>
    /// <param name="nodeAttribute">the attribute type which should be read</param>
    /// <returns>the attribute value of type T</returns>
    public static T AttributeValue<T>(this INode source, NodeAttributes nodeAttribute)
    {
        IAttribute attribute = source.Attribute(nodeAttribute); 
        if (typeof(T) != attribute.Type)
        {
            throw new NotSupportedException(string.Format("The given type parameter '{0}' is not the type of the attribute.", typeof(T).Name));
        }

        return (T) attribute.Value;
    }

    /// <summary>
    /// gets the attribute represented by the given usecasegraph attribute type 
    ///     throws 'KeyNotFoundException' if throwException is true and the attribute is not contained in the source item's attributes
    /// </summary>
    /// <param name="source">the node which contains the attributes</param>
    /// <param name="usecaseAttribute">the attribute type which should be read</param>
    /// <param name="throwException">weather to throw an exception or to return null, if the attribute is not contained in the attributes collection of the usecasegraph</param>
    /// <returns>the attribute value of type T</returns>
    public static IAttribute Attribute(this UseCaseGraph source, UseCaseAttributes usecaseAttribute,  bool throwException = true)
    {
        string attributeName = UseCaseGraph.UseCaseGraphAttributeNames[(int)usecaseAttribute];
        IAttribute attribute = source.Attributes.ByName(attributeName, throwException);

        return attribute;
    }

    /// <summary>
    /// gets the attribute represented by the given nodeAttribute type 
    ///     throws 'KeyNotFoundException' if throwException is true and the attribute is not contained in the source item's attributes
    /// </summary>
    /// <param name="source">the node which contains the attributes</param>
    /// <param name="nodeAttribute">the attribute type which should be read</param>
    /// <param name="throwException">weather to throw an exception or to return null, if the attribute is not contained in the attributes collection of the node</param>
    /// <returns>the attribute value of type T</returns>
    public static IAttribute Attribute(this INode source, NodeAttributes nodeAttribute, bool throwException = true)
    {
        string attributeName = UseCaseGraph.NodeAttributeNames[(int)nodeAttribute];
        IAttribute attribute = source.Attributes.ByName(attributeName, throwException);

        return attribute;
    }

    /// <summary>
    /// gets the attribute with the given name from the source enumerable
    ///     throws 'KeyNotFoundException' if throwException is true and the attribute is not contained in the source item's attributes
    /// </summary>
    /// <param name="sourceEnumerable">the enumerable which should contain the attribute with the given name</param>
    /// <param name="name">name of the attribute to search</param>
    /// <param name="throwException">weather to throw an exception or to return null, if there is no attribute with the given name.</param>
    /// <returns>the attribute with the given name</returns>
    public static IAttribute ByName(this IEnumerable<IAttribute> sourceEnumerable, string name,
        bool throwException = true)
    {
        IAttribute attribute = sourceEnumerable.FirstOrDefault(a => a.Name == name);
        if (attribute == null && throwException)
        {
            throw new KeyNotFoundException(string.Format("The attribute with the name '{0}' could not be found within the attributes collection of the usecasegraph!", name));
        }

        return attribute;
    }

    /// <summary>
    /// creates a attribute with the name from the given usecasegraph attribute enum value and the given attributeValue
    /// </summary>
    /// <param name="sourceUsecasegraphAttribute">the nodeAttribute enum value</param>
    /// <param name="attributeValue">the value of the attribute</param>
    /// <param name="hidden">value to determine weather the attribute should be hidden in the gui</param>
    /// <returns>a new attribute</returns>
    public static IAttribute CreateAttribute<T>(this UseCaseAttributes sourceUsecasegraphAttribute, T attributeValue, bool hidden = false)
    {
        IAttribute attribute = CreateAttribute((int)sourceUsecasegraphAttribute, UseCaseGraph.UseCaseGraphAttributeNames, attributeValue, hidden);
        return attribute;
    }

    /// <summary>
    /// creates a attribute with the name from the given nodeAttributes enum value and the given attributeValue
    /// </summary>
    /// <param name="sourceNodeAttribute">the nodeAttribute enum value</param>
    /// <param name="attributeValue">the value of the attribute</param>
    /// <param name="hidden">value to determine weather the attribute should be hidden in the gui</param>
    /// <returns>a new attribute</returns>
    public static IAttribute CreateAttribute<TValue>(this NodeAttributes sourceNodeAttribute, TValue attributeValue, bool hidden = false)
    {
        IAttribute attribute = CreateAttribute((int) sourceNodeAttribute, UseCaseGraph.NodeAttributeNames, attributeValue, hidden);
        return attribute;
    }

    /// <summary>
    /// gets the actual attribute name of the value represented by the enum
    /// </summary>
    /// <param name="sourceUseCaseAttribute">the enum value</param>
    /// <returns>the string of the attribute name</returns>
    public static string AttributeName(this UseCaseAttributes sourceUseCaseAttribute)
    {
        return UseCaseGraph.UseCaseGraphAttributeNames[(int)sourceUseCaseAttribute];
    }

    /// <summary>
    /// gets the actual attribute name of the value represented by the enum
    /// </summary>
    /// <param name="sourceNodeAttribute">the enum value</param>
    /// <returns>the string of the attribute name</returns>
    public static string AttributeName(this NodeAttributes sourceNodeAttribute)
    {
        return UseCaseGraph.NodeAttributeNames[(int)sourceNodeAttribute];
    }

    private static IAttribute CreateAttribute<T>(int enumValue, IList<string> attributeNameList, T attributeValue, bool hidden)
    {
        string attributeName = attributeNameList[enumValue];
        return hidden ? new HiddenAttribute(attributeName, attributeValue) : new Attribute(attributeName, attributeValue);
    }
}
