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
using GraphDrawing.DrawingElements;

namespace GraphDrawing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            KnotenUc Kn1 = AddKnoten(1,"1");
            Kn1.StartKnoten = true;
            KnotenUc Kn2 = AddKnoten(1, "2");
            KnotenUc Kn3 = AddKnoten(1, "3");
            KnotenUc Kn2b1 = AddKnoten(4, "2b1", Kn2);
            KnotenUc Kn2a1 = AddKnoten(3, "2a1", Kn2);
            KnotenUc Kn2a2 = AddKnoten(3, "2a2");
            KnotenUc Kn2a3 = AddKnoten(3, "2a3");
            KnotenUc Kn4 = AddKnoten(1, "4");
            KnotenUc Kn3a1 = AddKnoten(2, "3a1", Kn3);
            AddKante(Kn1, Kn2);
            AddKante(Kn2, Kn3);
            AddKante(Kn3, Kn3);
            AddKante(Kn2, Kn2a1);
            AddKante(Kn2a1, Kn2a2);
            AddKante(Kn2a2, Kn2a3);
            AddKante(Kn2a3, Kn1);
            AddKante(Kn2, Kn2b1);
            AddKante(Kn3, Kn4);
            AddKante(Kn3, Kn3a1);
            //ReCalcPositionsOfElements();
        }

        private const double minWidth = 160;
        private const double minHeight = 150;
        private List<KnotenUc> KnotenList = new List<KnotenUc>(); 

        public KnotenUc AddKnoten(uint variante, string name,KnotenUc referenceKnotenUc = null)
        {
            KnotenUc Knoten = new KnotenUc(variante, name);
            Knoten.PreviewMouseLeftButtonDown += UIElement_OnMouseDown;
            DrawingCanvas.Children.Add(Knoten);

            if (referenceKnotenUc != null)
                Knoten.Offset = referenceKnotenUc.Offset;

            for (int i = 0; i < KnotenList.Count; i++)
            {
                if (KnotenList[i].Variante == variante && KnotenList[i].Offset > Knoten.Offset)
                    Knoten.Offset = KnotenList[i].Offset;
            }
            if (KnotenList.Count > 0)
                Knoten.Offset += minHeight;
            
            KnotenList.Add(Knoten);
            Canvas.SetTop(Knoten, Knoten.Offset);
            Canvas.SetLeft(Knoten, minWidth*variante);

            
            return Knoten;
        }

        //public void ReCalcPositionsOfElements()
        //{
        //    KnotenUc searchKnotenUc = KnotenList.Find(uc => uc.StartKnoten);
        //    if (searchKnotenUc != null)
        //    {
        //        CalcPosition(searchKnotenUc, 0);
        //    }
        //}

        //private void CalcPosition(KnotenUc source, double offset)
        //{
        //    for (int i = 0; i < source.KantenList.Count; i++)
        //    {
        //        if (!source.KantenList[i].SourceKnotenUc.Equals(source) && !source.KantenList[i].DestKnotenUc.Equals(source.KantenList[i].SourceKnotenUc))
        //        {
        //            source.KantenList[i].DestKnotenUc.Offset = source.Offset + minHeight;
        //            CalcPosition(source.KantenList[i].DestKnotenUc, source.KantenList[i].DestKnotenUc.Offset);
        //        }
        //    }
        //}

        public KanteUc AddKante(KnotenUc kn1, KnotenUc kn2)
        {
            if (kn1 == null || kn2 == null)
                return null;
            KanteUc ergKanteUc = new KanteUc(kn1, kn2);
            DrawingCanvas.Children.Add(ergKanteUc);
            if (kn1.Variante < kn2.Variante)
                kn2.Offset = kn1.Offset + minHeight;

            for (int i = 0; i < KnotenList.Count; i++)
            {
                KnotenList[i].RecalcKanten();
            }

            return ergKanteUc;
        }
        

        private FrameworkElement selElement;
        private FrameworkElement otherElement = null;
        private Point offsetElementPosition;        
        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement)
            {
                selElement = (FrameworkElement)sender;
                offsetElementPosition = Mouse.GetPosition(DrawingCanvas);
                Point elementPoint = new Point(Canvas.GetLeft(selElement), Canvas.GetTop(selElement));
                offsetElementPosition.X -= elementPoint.X;
                offsetElementPosition.Y -= elementPoint.Y;
            }

        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selElement != null)
            {
                foreach (KnotenUc fe in DrawingCanvas.Children)
                {
                    if(fe != selElement)
                        continue;

                    Canvas.SetTop(fe, e.GetPosition(this).Y - offsetElementPosition.Y);
                    Canvas.SetLeft(fe, e.GetPosition(this).X - offsetElementPosition.X);
                    
                    fe.RecalcKanten();
                    selElement = null;
                    break;

                }
            }
        }

        
    

        private void MainWindow_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (selElement != null)
            {
                foreach (FrameworkElement fe in DrawingCanvas.Children)
                {
                    if (fe != selElement)
                        continue;

                    Canvas.SetTop(fe, e.GetPosition(this).Y - offsetElementPosition.Y);
                    Canvas.SetLeft(fe, e.GetPosition(this).X - offsetElementPosition.X);

                }
            }
        }
    }
}
