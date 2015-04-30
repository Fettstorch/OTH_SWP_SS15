using System;
using GraphFramework.Interfaces;

namespace GraphFramework
{
    /// <summary>
    /// The attribute class. A attribute is a holder of a generic value which has a name
    /// </summary>
    public class Attribute : IAttribute
    {
        /// <summary>
        /// Gets and sets the value of the attribute
        /// </summary>
        public object Value
        {
            get
            {
                return mValue;
            }

            set
            {
                if (Type == value.GetType())
                {
                    mValue = value;
                }
            }
        }

        /// <summary>
        /// Gets the type of the attribute
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the name of the attribute
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// the generic value of the attribute
        /// </summary>
        private object mValue;

        /// <summary>
        /// attribute constructor
        /// </summary>
        /// <param name="name">the name</param>
        /// <param name="val">the generic value</param>
        public Attribute(string name, object val)
        {
            if (val == null || name == "")
            {
                throw new Exception("Incorrect attribute initialisation!");
            }
            else
            {
                Name = name;
                Type = val.GetType();
                Value = val;
            }
        }
    }
}