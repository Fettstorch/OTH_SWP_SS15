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
