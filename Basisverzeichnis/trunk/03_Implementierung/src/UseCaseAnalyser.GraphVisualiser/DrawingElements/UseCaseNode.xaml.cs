using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.GraphVisualiser.DrawingElements
{
    /// <summary>
    ///     Interaction logic for UseCaseNode.xaml
    /// </summary>
    internal partial class UseCaseNode : ISelectableObject
    {
        public readonly List<UseCaseEdge> mEdges = new List<UseCaseEdge>();

        public UseCaseNode(INode node)
        {
            Selected = false;
            InitializeComponent();
            LblIndex.Content = node.Attributes.First(attr =>
                attr.Name == UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.Index]).Value;
            Node = node;
            mDrawingBrush = NodeBorder.BorderBrush = Brushes.Black;
        }



        public INode Node { get; private set; }

        public bool Selected { get; set; }

        /// <summary>
        /// Recursive function for rendering edges of this node by using RecalcBezier and its neighbours.
        /// </summary>
        /// <param name="notRenderNode">Optional parameter for use case node which prevents rendering of specified node's edges.</param>
        public void RenderEdges(UseCaseNode notRenderNode = null)
        {
            foreach (UseCaseEdge ucEdge in mEdges)
            {
                //render this node
                if (notRenderNode == null)
                {
                    //prevent re-rendering in recursive call by setting current node as not to render
                    if (!Equals(ucEdge.mDestUseCaseNode, this))
                        ucEdge.mDestUseCaseNode.RenderEdges(this);
                    else
                        ucEdge.mSourceUseCaseNode.RenderEdges(this);
                }
                //check if rendering of notRenderNode should be skipped (otherwise infinite recusive call of this function)
                else if (Equals(ucEdge.mSourceUseCaseNode, notRenderNode) || Equals(ucEdge.mDestUseCaseNode, notRenderNode))
                    continue;

                ucEdge.RecalcBezier();
            }
        }

        public void AddEdge(UseCaseEdge newEdge)
        {
            if (!mEdges.Contains(newEdge))
                mEdges.Add(newEdge);
        }

        public int GetEdgeIndex(UseCaseEdge sourceKante)
        {
            UseCaseEdge.DockedStatus currentDockedStatus = sourceKante.mSourceUseCaseNode.Equals(this)
                ? sourceKante.StatusSourceElement
                : sourceKante.StatusDestElement;

            for (int i = 0, index = 1; i < mEdges.Count; i++)
            {
                if (mEdges[i].mSourceUseCaseNode.Equals(this) && mEdges[i].StatusSourceElement == currentDockedStatus ||
                    mEdges[i].mDestUseCaseNode.Equals(this) && mEdges[i].StatusDestElement == currentDockedStatus)
                {
                    if (sourceKante.Equals(mEdges[i]))
                        return index;
                    index++;
                }
            }
            return 0;
        }

        public int GetCountOfEdges(UseCaseEdge sourceKante)
        {
            int index = 1;

            UseCaseEdge.DockedStatus currentDockedStatus = sourceKante.mSourceUseCaseNode.Equals(this)
                ? sourceKante.StatusSourceElement
                : sourceKante.StatusDestElement;


            // ReSharper disable once LoopCanBeConvertedToQuery
            // [Fettstorch] keep more readable version of loop
            foreach (UseCaseEdge useCaseEdge in mEdges)
            {
                if (useCaseEdge.mSourceUseCaseNode.Equals(this) &&
                    useCaseEdge.StatusSourceElement == currentDockedStatus ||
                    useCaseEdge.mDestUseCaseNode.Equals(this) && useCaseEdge.StatusDestElement == currentDockedStatus)
                {
                    index++;
                }
            }
            return index;
        }


        private Brush mDrawingBrush;

        public void SetDrawingBrush(Brush newBrush)
        {
            NodeBorder.BorderBrush = mDrawingBrush = newBrush;
        }


        public void Select()
        {
            Selected = true;
            NodeBorder.BorderBrush = Brushes.Orange;
        }

        public void Unselect()
        {
            Selected = false;
            NodeBorder.BorderBrush = mDrawingBrush;
        }
        public void ChangeSelection()
        {
            if (Selected)
                Unselect();
            else
                Select();
        }
        public IGraphElement CurrentElement
        {
            get
            {
                return Node;
            }
        }
    }
}