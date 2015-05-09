using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UseCaseAnalyser.GraphVisualiser.DrawingElements;

namespace UseCaseAnalyser.GraphVisualiser
{
    /// <summary>
    ///     Interaction logic for GraphVisualiser.xaml
    /// </summary>
    public partial class GraphVisualiser : UserControl
    {
        private const double ElementWidth = 160;
        private const double ElementHeight = 150;
        private readonly List<UseCaseNode> mNodes = new List<UseCaseNode>();
        private Point mOffsetElementPosition;
        private FrameworkElement mSelectedElement;

        /// <summary>
        /// GraphVisualiser default constructor
        /// </summary>
        public GraphVisualiser()
        {
            InitializeComponent();

            UseCaseNode kn1 = AddNode(1, "1");
            UseCaseNode kn2 = AddNode(1, "2");
            UseCaseNode kn3 = AddNode(1, "3");
            UseCaseNode kn2B1 = AddNode(4, "2b1", kn2);
            UseCaseNode kn2A1 = AddNode(3, "2a1", kn2);
            UseCaseNode kn2A2 = AddNode(3, "2a2");
            UseCaseNode kn2A3 = AddNode(3, "2a3");
            UseCaseNode kn4 = AddNode(1, "4");
            UseCaseNode kn3A1 = AddNode(2, "3a1", kn3);
            AddEdge(kn1, kn2);
            AddEdge(kn2, kn3);
            AddEdge(kn2, kn2A1);
            AddEdge(kn2A1, kn2A2);
            AddEdge(kn2A2, kn2A3);
            AddEdge(kn2A3, kn1);
            AddEdge(kn2, kn2B1);
            AddEdge(kn3, kn4);
            AddEdge(kn3, kn3A1);
            //ReCalcPositionsOfElements();
        }

        public UseCaseNode AddNode(uint slotNumber, string name, UseCaseNode referenceUseCaseNode = null)
        {
            UseCaseNode node = new UseCaseNode(slotNumber, name);
            node.PreviewMouseLeftButtonDown += GraphVisualiser_OnMouseDown;
            DrawingCanvas.Children.Add(node);

            if (referenceUseCaseNode != null)
                node.YOffset = referenceUseCaseNode.YOffset;

            foreach (UseCaseNode ucNode in mNodes)
            {
                if (ucNode.SlotNumber == slotNumber && ucNode.YOffset > node.YOffset)
                    node.YOffset = ucNode.YOffset;
            }
            if (mNodes.Count > 0)
                node.YOffset += ElementHeight;

            mNodes.Add(node);
            Canvas.SetTop(node, node.YOffset);
            Canvas.SetLeft(node, ElementWidth*(slotNumber - 1) + 40);

            if (DrawingCanvas.Width < (ElementWidth)*(slotNumber) + 40)
                DrawingCanvas.Width = (ElementWidth)*(slotNumber) + 40;
            if (DrawingCanvas.Height < (node.YOffset + ElementHeight))
                DrawingCanvas.Height = node.YOffset + ElementHeight;

            return node;
        }

        //public void ReCalcPositionsOfElements()
        //{
        //    UseCaseNode searchKnotenUc = Nodes.Find(uc => uc.StartKnoten);
        //    if (searchKnotenUc != null)
        //    {
        //        CalcPosition(searchKnotenUc, 0);
        //    }
        //}

        //private void CalcPosition(UseCaseNode source, double offset)
        //{
        //    for (int i = 0; i < source.KantenList.Count; i++)
        //    {
        //        if (!source.KantenList[i].SourceUseCaseNode.Equals(source) && source.KantenList[i].ProcessType == UseCaseEdge.EdgeProcessType.ForwardEdge)
        //        {
        //            source.KantenList[i].DestUseCaseNode.YOffset = source.YOffset + ElementHeight;
        //            CalcPosition(source.KantenList[i].DestUseCaseNode, source.KantenList[i].DestUseCaseNode.YOffset);
        //        }
        //    }
        //}

        public UseCaseEdge AddEdge(UseCaseNode firstNode, UseCaseNode secondNode)
        {
            if (firstNode == null || secondNode == null)
                return null;
            UseCaseEdge useCaseEdge = new UseCaseEdge(firstNode, secondNode);
            DrawingCanvas.Children.Add(useCaseEdge);
            if (firstNode.SlotNumber < secondNode.SlotNumber)
                secondNode.YOffset = firstNode.YOffset + ElementHeight;

            firstNode.RenderEdges();
            secondNode.RenderEdges();

            return useCaseEdge;
        }

        private void GraphVisualiser_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                mSelectedElement = element;
                mOffsetElementPosition = Mouse.GetPosition(DrawingCanvas);
                Point elementPoint = new Point(Canvas.GetLeft(mSelectedElement), Canvas.GetTop(mSelectedElement));
                mOffsetElementPosition.X -= elementPoint.X;
                mOffsetElementPosition.Y -= elementPoint.Y;
            }
        }

        private void GraphVisualiser_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mSelectedElement != null)
            {
                foreach (UseCaseNode useCaseNode in DrawingCanvas.Children)
                {
                    if (!useCaseNode.Equals(mSelectedElement))
                        continue;

                    //Canvas.SetTop(fe, e.GetPosition(this).Y - offsetElementPosition.Y);
                    //Canvas.SetLeft(fe, e.GetPosition(this).X - offsetElementPosition.X);

                    useCaseNode.RenderEdges();
                    mSelectedElement = null;
                    break;
                }
            }
        }

        private void GraphVisualiser_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (mSelectedElement != null)
            {
                foreach (FrameworkElement frameworkElement in DrawingCanvas.Children)
                {
                    if (!frameworkElement.Equals(mSelectedElement))
                        continue;


                    Canvas.SetTop(frameworkElement, e.GetPosition(this).Y - mOffsetElementPosition.Y);
                    Canvas.SetLeft(frameworkElement, e.GetPosition(this).X - mOffsetElementPosition.X);
                    UseCaseNode node = frameworkElement as UseCaseNode;
                    if (node != null)
                        node.RenderEdges();
                }
            }
        }
    }
}