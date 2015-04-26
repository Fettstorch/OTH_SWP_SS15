using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                    if (Canvas.GetTop(SourceKnotenUc) < Canvas.GetTop(DestKnotenUc))
                    {
                        StatusSourceElement = DockedStatus.Bottom;
                        StatusDestElement = DockedStatus.Top;
                    }
                    else
                    {
                        StatusSourceElement = DockedStatus.Top;
                        StatusDestElement = DockedStatus.Bottom;
                    }
                    break;
                case KantenType.Recursiv:
                    StatusSourceElement = DockedStatus.Left;
                    StatusDestElement = DockedStatus.Left;
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

            Point startpoint,endpoint;
            switch (Type)
            {
                case KantenType.Proecess:
                    startpoint = new Point(Canvas.GetLeft(SourceKnotenUc) + (SourceKnotenUc.Width / amountIndexStart) * indexStartElement, Canvas.GetTop(SourceKnotenUc));
                    endpoint = new Point(Canvas.GetLeft(DestKnotenUc) + (DestKnotenUc.Width / amountIndexEnd) * indexDestElement, Canvas.GetTop(DestKnotenUc));
                    double heightStart, heightEnd;
                    heightStart = StatusSourceElement == DockedStatus.Bottom ? startElement.Height : 0;
                    heightEnd = StatusDestElement == DockedStatus.Bottom ? startElement.Height : 0;

                    pthFigure.StartPoint = new Point(startpoint.X, startpoint.Y + heightStart);
                    bzSeg.Point1 = new Point(startpoint.X, endpoint.Y + heightEnd);
                    bzSeg.Point2 = new Point(endpoint.X, startpoint.Y + heightStart);
                    bzSeg.Point3 = new Point(endpoint.X, endpoint.Y + heightEnd); ;   
                    break;
                case KantenType.Recursiv:
                    startpoint = new Point(Canvas.GetLeft(SourceKnotenUc), Canvas.GetTop(SourceKnotenUc)+(SourceKnotenUc.Height/ amountIndexStart) * indexStartElement);
                    pthFigure.StartPoint = startpoint;
                    bzSeg.Point1 = new Point(startpoint.X - (SourceKnotenUc.Width/2), startpoint.Y - SourceKnotenUc.Width/2);
                    bzSeg.Point2 = new Point(startpoint.X - (SourceKnotenUc.Width/2), startpoint.Y + SourceKnotenUc.Width/2);
                    bzSeg.Point3 = startpoint;
                    break;
                case KantenType.ReturnProcess:
                    startpoint = new Point(Canvas.GetLeft(SourceKnotenUc) + SourceKnotenUc.Width, Canvas.GetTop(SourceKnotenUc) + (SourceKnotenUc.Height / amountIndexStart) * indexStartElement);
                    endpoint = new Point(Canvas.GetLeft(DestKnotenUc) + DestKnotenUc.Width, Canvas.GetTop(DestKnotenUc) + (DestKnotenUc.Height / amountIndexEnd) * indexDestElement);
            
                    pthFigure.StartPoint = startpoint;
                    bzSeg.Point1 = new Point(startpoint.X + SourceKnotenUc.Width / 2, startpoint.Y);
                    bzSeg.Point2 = new Point(startpoint.X + DestKnotenUc.Width / 2, endpoint.Y);
                    bzSeg.Point3 = endpoint;   
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
