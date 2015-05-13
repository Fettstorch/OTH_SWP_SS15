using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphFramework;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.GraphVisualiser.DrawingElements
{
    public interface ISelectableObject
    {
        bool Selected { get; set; }
        void Select();
        void Unselect();
        void ChangeSelection();
        IGraphElement CurrentElement { get; }
    }
}
