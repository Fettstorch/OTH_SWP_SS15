#region Copyright information
// <summary>
// <copyright file="Node.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>30/04/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using GraphFramework.Interfaces;

namespace GraphFramework
{
    /// <summary>
    /// Node class
    /// </summary>
    public class Node : GraphElement, INode
    {
        /// <summary>
        /// Adds also attributes to the node
        /// </summary>
        /// <param name="attributes">the attributes you want to add</param>
        public Node(params IAttribute[] attributes) : base(attributes) { }
    }
}