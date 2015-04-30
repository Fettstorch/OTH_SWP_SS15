using System;

namespace GraphFramework.Interfaces
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
        Type Type { get; }

        /// <summary>
        /// name to identify the attribute
        /// </summary>
        string Name { get; set; }
    }
}