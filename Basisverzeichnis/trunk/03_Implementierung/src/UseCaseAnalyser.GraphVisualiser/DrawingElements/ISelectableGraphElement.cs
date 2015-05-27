#region Copyright information

// <summary>
// <copyright file="ISelectableGraphElement.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>13/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>

#endregion

using GraphFramework.Interfaces;

namespace UseCaseAnalyser.GraphVisualiser.DrawingElements
{
    /// <summary>
    /// An interface for classes that contain a GraphElement and should be selectable via the visualisation.
    /// </summary>
    internal interface ISelectableGraphElement
    {
        /// <summary>
        /// Check if object is selected
        /// </summary>
        bool Selected { get; }

        /// <summary>
        /// Change selection state to selected
        /// </summary>
        void Select();

        /// <summary>
        /// Reset selection state
        /// </summary>
        void Unselect();
        
        /// <summary>
        /// Toggle selection state
        /// </summary>
        void ChangeSelection();

        /// <summary>
        /// Get reference to IGraphElement if selected
        /// </summary>
        IGraphElement CurrentElement { get; }
    }
}