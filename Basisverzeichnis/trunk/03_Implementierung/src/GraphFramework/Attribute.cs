#region Copyright information
// <summary>
// <copyright file="Attribute.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>30/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
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
                else
                {
                    throw new InvalidOperationException("You cannot define a new attribute value with a different type!");
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
            if (val == null || name == null)
            {
                throw new ArgumentNullException("val");
            }
            if(string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name string of the attribute shall not be empty");
            }
            
            Name = name;
            Type = val.GetType();
            Value = val;
        }
    }
}