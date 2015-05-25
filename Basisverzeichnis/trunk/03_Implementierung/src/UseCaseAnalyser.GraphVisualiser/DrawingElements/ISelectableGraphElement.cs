#region Copyright information
// <summary>
// <copyright file="ISelectableGraphElement.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>25/05/2015</creationDate>
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
    internal interface ISelectableGraphElement
    {
        bool Selected { get;}
        void Select();
        void Unselect();
        void ChangeSelection();
        IGraphElement CurrentElement { get; }
    }
}
