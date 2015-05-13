using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphFramework.Interfaces;

namespace UseCaseAnalyser.GraphVisualiser.DrawingElements
{
    /// <summary>
    ///     Interaction logic for UseCaseEdge.xaml
    /// </summary>
    public partial class UseCaseEdge : ISelectableObject
    {
        public readonly UseCaseNode mDestUseCaseNode;
        public readonly UseCaseNode mSourceUseCaseNode;
      
        #region constructors

        public UseCaseEdge(UseCaseNode source, UseCaseNode dest, IEdge edge)
        {
            InitializeComponent();
            Edge = edge;
            mSourceUseCaseNode = source;
            mDestUseCaseNode = dest;
            mSourceUseCaseNode.AddEdge(this);
            mDestUseCaseNode.AddEdge(this);
            ProcessType = source.YOffset < dest.YOffset ? EdgeProcessType.ForwardEdge : EdgeProcessType.BackwardEdge;

            Stroke = mDrawingBrush = new SolidColorBrush(Colors.Black);
            StrokeThickness = 1.5;
            EndCap = Geometry.Parse("M0,0 L6,-6 L6,6 z");

            RecalcBezier();
        }

        #endregion

        public IEdge Edge { get; private set; }

        public void RecalcBezier()
        {
            switch (ProcessType)
            {
                case EdgeProcessType.ForwardEdge:
                    //Source node over destination node
                    if (Canvas.GetTop(mSourceUseCaseNode) + mSourceUseCaseNode.Height < Canvas.GetTop(mDestUseCaseNode))
                    {
                        StatusSourceElement = DockedStatus.Bottom;
                        StatusDestElement = DockedStatus.Top;
                    }
                    //Source node under destination node
                    else if (Canvas.GetTop(mDestUseCaseNode) + mDestUseCaseNode.Height <
                             Canvas.GetTop(mSourceUseCaseNode))
                    {
                        StatusSourceElement = DockedStatus.Top;
                        StatusDestElement = DockedStatus.Bottom;
                    }
                    //Source node left of destination node
                    else if (Canvas.GetLeft(mSourceUseCaseNode) + mSourceUseCaseNode.Width <
                             Canvas.GetLeft(mDestUseCaseNode))
                    {
                        StatusSourceElement = DockedStatus.Right;
                        StatusDestElement = DockedStatus.Left;
                    }
                    //Source node right of destination node
                    else if (Canvas.GetLeft(mSourceUseCaseNode) >
                             Canvas.GetLeft(mDestUseCaseNode) + mDestUseCaseNode.Width)
                    {
                        StatusSourceElement = DockedStatus.Left;
                        StatusDestElement = DockedStatus.Right;
                    }
                    break;
                case EdgeProcessType.BackwardEdge:
                    if (Canvas.GetLeft(mSourceUseCaseNode) + mDestUseCaseNode.Width < Canvas.GetLeft(mDestUseCaseNode))
                    {
                        StatusSourceElement = DockedStatus.Right;
                        StatusDestElement = DockedStatus.Left;

                    }
                    else
                    {
                        StatusSourceElement = DockedStatus.Right;
                        StatusDestElement = DockedStatus.Right;
                    }

                    break;
            }
            int indexStartElement = mSourceUseCaseNode.GetEdgeIndex(this);
            int indexDestElement = mDestUseCaseNode.GetEdgeIndex(this);
            int amountIndexStart = mSourceUseCaseNode.GetCountOfEdges(this);
            int amountIndexEnd = mDestUseCaseNode.GetCountOfEdges(this);

            PathGeometry pthGeometry = new PathGeometry();
            PathFigureCollection pthFigureCollection = new PathFigureCollection();
            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            BezierSegment bzSeg = new BezierSegment();
            PathFigure pthFigure = new PathFigure();


            Point startpoint;
            Point endpoint;
            switch (StatusSourceElement)
            {
                case DockedStatus.Top:
                case DockedStatus.Bottom:
                    startpoint =
                        new Point(
                            Canvas.GetLeft(mSourceUseCaseNode) +
                            (mSourceUseCaseNode.Width/amountIndexStart)*indexStartElement,
                            Canvas.GetTop(mSourceUseCaseNode));
                    endpoint =
                        new Point(
                            Canvas.GetLeft(mDestUseCaseNode) + (mDestUseCaseNode.Width/amountIndexEnd)*indexDestElement,
                            Canvas.GetTop(mDestUseCaseNode));
                    double heightStart = StatusSourceElement == DockedStatus.Bottom ? mSourceUseCaseNode.Height : 0;
                    double heightEnd = StatusDestElement == DockedStatus.Bottom ? mDestUseCaseNode.Height : 0;

                    pthFigure.StartPoint = new Point(startpoint.X, startpoint.Y + heightStart);
                    bzSeg.Point1 = new Point(startpoint.X, endpoint.Y + heightEnd);
                    bzSeg.Point2 = new Point(endpoint.X, startpoint.Y + heightStart);
                    bzSeg.Point3 = new Point(endpoint.X, endpoint.Y + heightEnd);
                    break;

                case DockedStatus.Right:
                case DockedStatus.Left:
                    double widthStart = StatusSourceElement == DockedStatus.Right ? mSourceUseCaseNode.Width : 0;
                    double widthEnd = StatusDestElement == DockedStatus.Right ? mDestUseCaseNode.Width : 0;

                    startpoint = new Point(Canvas.GetLeft(mSourceUseCaseNode) + widthStart,
                        Canvas.GetTop(mSourceUseCaseNode) +
                        (mSourceUseCaseNode.Height/amountIndexStart)*indexStartElement);
                    pthFigure.StartPoint = startpoint;

                    endpoint = new Point(Canvas.GetLeft(mDestUseCaseNode) + widthEnd,
                        Canvas.GetTop(mDestUseCaseNode) + (mDestUseCaseNode.Height/amountIndexEnd)*indexDestElement);
                   
                    double middlePos = (endpoint.X - startpoint.X)/ 2;
                    
                    //middlePos = middlePos < 0 ? middlePos * -1 : middlePos;
                    if (ProcessType == EdgeProcessType.BackwardEdge)
                    {
                        double resultEndPosY;

                        if (StatusDestElement == DockedStatus.Right &&
                            startpoint.Y - mSourceUseCaseNode.Height / 2 < endpoint.Y + mDestUseCaseNode.Height / 2)
                        {
                            if (startpoint.Y > endpoint.Y)
                                resultEndPosY = startpoint.Y - 1.5 * mSourceUseCaseNode.Height;
                            else
                                resultEndPosY = startpoint.Y + 1.5 * mSourceUseCaseNode.Height;
                        }
                        else
                        {
                            resultEndPosY = endpoint.Y;
                         }

                        bzSeg.Point1 = new Point(startpoint.X + (mSourceUseCaseNode.Width / 2), startpoint.Y);
                        bzSeg.Point2 = new Point(startpoint.X + (mDestUseCaseNode.Width / 2), resultEndPosY);

                    }
                    else
                    {
                       
                        bzSeg.Point1 = new Point(startpoint.X + middlePos , startpoint.Y);
                        bzSeg.Point2 = new Point(startpoint.X + middlePos , endpoint.Y);
                   
                    }
                    bzSeg.Point3 = endpoint;
                    break;
            }

            myPathSegmentCollection.Clear();
            myPathSegmentCollection.Add(bzSeg);
            pthFigure.Segments = myPathSegmentCollection;
            pthFigureCollection.Clear();
            pthFigureCollection.Add(pthFigure);

            pthGeometry.Figures = pthFigureCollection;

            LinePath = pthGeometry;



        }

        
        public void SetDrawingBrush(Brush newBrush)
        {
            Stroke = mDrawingBrush = newBrush;
            RecalcBezier();
        }

        #region DockedStatus enum

        public enum DockedStatus
        {
            Top,
            Bottom,
            Left,
            Right
        }

        #endregion

        #region EdgeProcessType enum

        public enum EdgeProcessType
        {
            ForwardEdge,
            BackwardEdge
        }

        #endregion

        #region Properties

    
        public bool Selected { get; set; }
        
        internal DockedStatus StatusSourceElement { get; set; }
        internal DockedStatus StatusDestElement { get; set; }
        internal EdgeProcessType ProcessType { get; set; }
        
        private Brush mDrawingBrush;
       
        #endregion
     

        public void Select()
        {
            Selected = true;
            Stroke = Brushes.Orange;
            RecalcBezier();
        }

        public void Unselect()
        {
            Selected = false;
            Stroke = mDrawingBrush;
            RecalcBezier();
        }


        public void ChangeSelection()
        {
            if(Selected)
                Unselect();
            else
                Select();
        }


        public IGraphElement CurrentElement
        {
            get
            {
                return Edge;
            }
        }
    }
}