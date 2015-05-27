using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GraphDrawing.DrawingElements
{
    /// <summary>
    /// Interaction logic for KanteUc.xaml
    /// </summary>
    public partial class KanteUc : UserControl
    {
        public KanteUc(KnotenUc source, KnotenUc dest)
        {
            InitializeComponent();
            SourceKnotenUc = source;
            DestKnotenUc = dest;
            SourceKnotenUc.AddKante(this);
            DestKnotenUc.AddKante(this);
            if (source.Equals(dest))
                Type = KantenType.Recursiv;
            else if (source.Offset < dest.Offset)
                Type = KantenType.Proecess;
            else
                Type = KantenType.ReturnProcess;
       
            Recalc(SourceKnotenUc);
        }

        public readonly KnotenUc SourceKnotenUc;

        public readonly KnotenUc DestKnotenUc;


        private PathGeometry pthGeometry = new PathGeometry();
        private PathFigureCollection pthFigureCollection = new PathFigureCollection();
        private PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
        private BezierSegment bzSeg = new BezierSegment();
        private PathFigure pthFigure = new PathFigure();
                     
        public void Recalc(KnotenUc sender)
        {
            RecalcBezier(SourceKnotenUc, DestKnotenUc);
                    
        }
        private void RecalcBezier(KnotenUc startElement, KnotenUc endElement)
        {
            switch (Type)
            {
                case KantenType.Proecess:
                    //Source node over destination node
                    if (Canvas.GetTop(SourceKnotenUc) + SourceKnotenUc.Height < Canvas.GetTop(DestKnotenUc))
                    {
                        StatusSourceElement = DockedStatus.Bottom;
                        StatusDestElement = DockedStatus.Top;
                    }
                    //Source node under destination node
                    else if (Canvas.GetTop(DestKnotenUc) + DestKnotenUc.Height < Canvas.GetTop(SourceKnotenUc))
                    {
                        StatusSourceElement = DockedStatus.Top;
                        StatusDestElement = DockedStatus.Bottom;
                    }
                    //Source node left of destination node
                    else if (Canvas.GetLeft(SourceKnotenUc) + SourceKnotenUc.Width < Canvas.GetLeft(DestKnotenUc))
                    {
                        StatusSourceElement = DockedStatus.Rigth;
                        StatusDestElement = DockedStatus.Left;
                    }
                    //Source node right of destination node
                    else if (Canvas.GetLeft(SourceKnotenUc) > Canvas.GetLeft(DestKnotenUc)+ DestKnotenUc.Width)
                    {
                        StatusSourceElement = DockedStatus.Left;
                        StatusDestElement = DockedStatus.Rigth;
                    }
                    break;
                case KantenType.Recursiv:
                    StatusSourceElement = StatusDestElement = DockedStatus.Left;
                    break;
                case KantenType.ReturnProcess:
                    StatusSourceElement = DockedStatus.Rigth;
                    StatusDestElement = DockedStatus.Rigth;
                    break;
   
            }
            int indexStartElement = startElement.GetIndexOfKante(this);
            int indexDestElement = endElement.GetIndexOfKante(this);
            int amountIndexStart = startElement.GetAmountOfIndexOfKante(this);
            int amountIndexEnd = endElement.GetAmountOfIndexOfKante(this);

            Point startpoint = new Point(0,0);
            Point endpoint = new Point(0, 0); ;
            switch (StatusSourceElement)
            {
                case DockedStatus.Top:
                case DockedStatus.Bottom:
                    startpoint = new Point(Canvas.GetLeft(SourceKnotenUc) + (SourceKnotenUc.Width / amountIndexStart) * indexStartElement, Canvas.GetTop(SourceKnotenUc));
                    endpoint = new Point(Canvas.GetLeft(DestKnotenUc) + (DestKnotenUc.Width / amountIndexEnd) * indexDestElement, Canvas.GetTop(DestKnotenUc));
                    double heightStart = StatusSourceElement == DockedStatus.Bottom ? startElement.Height : 0;
                    double heightEnd = StatusDestElement == DockedStatus.Bottom ? endElement.Height : 0;

                    pthFigure.StartPoint = new Point(startpoint.X, startpoint.Y + heightStart);
                    bzSeg.Point1 = new Point(startpoint.X, endpoint.Y + heightEnd);
                    bzSeg.Point2 = new Point(endpoint.X, startpoint.Y + heightStart);
                    bzSeg.Point3 = new Point(endpoint.X, endpoint.Y + heightEnd); 
                break;

                case DockedStatus.Rigth:
                case DockedStatus.Left:
                    double widthStart = StatusSourceElement == DockedStatus.Rigth ? startElement.Width : 0;
                    double widthEnd = StatusDestElement == DockedStatus.Rigth ? endElement.Width : 0;

                    startpoint = new Point(Canvas.GetLeft(SourceKnotenUc) + widthStart, Canvas.GetTop(SourceKnotenUc) + (SourceKnotenUc.Height / amountIndexStart) * indexStartElement);
                    pthFigure.StartPoint = startpoint;
                        
                    if (Type == KantenType.Recursiv)
                    {
                        double correctionFactor = widthStart > 0 ? 1 : -1;
                        bzSeg.Point1 = new Point(startpoint.X + correctionFactor * (SourceKnotenUc.Width / 2), startpoint.Y - SourceKnotenUc.Width / 2);
                        bzSeg.Point2 = new Point(startpoint.X + correctionFactor * (SourceKnotenUc.Width / 2), startpoint.Y + SourceKnotenUc.Width / 2);
                        bzSeg.Point3 = startpoint;
                    }
                    else
                    {  
                        endpoint = new Point(Canvas.GetLeft(DestKnotenUc) + widthEnd, Canvas.GetTop(DestKnotenUc) + (DestKnotenUc.Height / amountIndexEnd) * indexDestElement);
                        double correctfactor = Type == KantenType.ReturnProcess ? 1 : 0;

                        bzSeg.Point1 = new Point(startpoint.X + correctfactor * SourceKnotenUc.Width / 2, startpoint.Y);
                        bzSeg.Point2 = new Point(startpoint.X + correctfactor * DestKnotenUc.Width / 2, endpoint.Y);
                        bzSeg.Point3 = endpoint;
                    }
                       
                break;
            }            
         
            myPathSegmentCollection.Clear();
            myPathSegmentCollection.Add(bzSeg);
            pthFigure.Segments = myPathSegmentCollection;
            pthFigureCollection.Clear();
            pthFigureCollection.Add(pthFigure);

            pthGeometry.Figures = pthFigureCollection;

            ArrowCappedLine.LinePath = pthGeometry;
       }

        private bool selected;

        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                if(selected)
                    ArrowCappedLine.Stroke = new SolidColorBrush(Colors.Orange);
                else
                    ArrowCappedLine.Stroke = new SolidColorBrush(Colors.Black);
                RecalcBezier(SourceKnotenUc,DestKnotenUc);
            }
        }

        public DockedStatus StatusSourceElement { get; set; }
        public DockedStatus StatusDestElement { get; set; }
        public KantenType Type { get; set; }
        
        public enum KantenType { Proecess,Recursiv,ReturnProcess }

        public enum DockedStatus { Top,Bottom, Left, Rigth}


        private void KanteUc_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Selected = !Selected;
        }
    }
}
