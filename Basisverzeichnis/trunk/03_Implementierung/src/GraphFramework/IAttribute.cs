using System;

namespace GraphFramework
{
    public interface IAttribute
    {
        /// <summary>
        /// value of the attriute
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// type of the value
        /// </summary>
        Type Type { get; set; }

        /// <summary>
        /// name to identify the attribute
        /// </summary>
        string Name { get; set; }
    }
}