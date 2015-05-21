using GraphFramework.Interfaces;

namespace UseCaseAnalyser.GraphVisualiser.DrawingElements
{
    internal interface ISelectableObject
    {
        bool Selected { get;}
        void Select();
        void Unselect();
        void ChangeSelection();
        IGraphElement CurrentElement { get; }
    }
}
