using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
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
    /// Interaction logic for KnotenUc.xaml
    /// </summary>
    public partial class KnotenUc : UserControl
    {
        public uint Variante { get; set; }
        private double offset;
        public bool StartKnoten { get; set; }

        public double Offset
        {
            get { return offset; }
            set
            {
                offset = value;
                Canvas.SetTop(this, Offset);
                RecalcKanten();
   
            }
        }

        public string Index { get; set; }
        public KnotenUc(uint variant, string useCaseIdnex)
        {
            StartKnoten = false;
            Offset = 0;
            Variante=variant;
            InitializeComponent();
            lblIndex.Content = Index =useCaseIdnex;
        }

        public void RecalcKanten()
        {
            for (int i = 0; i < KantenList.Count; i++)
            {
                KantenList[i].Recalc(this);
            }
        }

        public void AddKante(KanteUc addObj)
        {
            if (!KantenList.Contains(addObj))
                KantenList.Add(addObj);
        }

        public int GetIndexOfKante(KanteUc sourceKante)
        {
            KanteUc.DockedStatus currentDockedStatus;

            if(sourceKante.SourceKnotenUc.Equals(this))
                currentDockedStatus = sourceKante.StatusSourceElement;
            else
                currentDockedStatus = sourceKante.StatusDestElement;

            for (int i = 0,index = 1; i < KantenList.Count; i++)
            {
            
                if (KantenList[i].SourceKnotenUc.Equals(this) && KantenList[i].StatusSourceElement == currentDockedStatus||
                    KantenList[i].DestKnotenUc.Equals(this) && KantenList[i].StatusDestElement == currentDockedStatus)
                {
                    if (sourceKante.Equals(KantenList[i]))
                        return index;
                    index++;
                }
            }
            return 0;
        }
        
        public int GetAmountOfIndexOfKante(KanteUc sourceKante)
        {
            int index = 1;
            KanteUc.DockedStatus currentDockedStatus;

            if (sourceKante.SourceKnotenUc.Equals(this))
                currentDockedStatus = sourceKante.StatusSourceElement;
            else
                currentDockedStatus = sourceKante.StatusDestElement;
            

            for (int i = 0 ; i < KantenList.Count; i++)
            {
                if (KantenList[i].SourceKnotenUc.Equals(this) && KantenList[i].StatusSourceElement == currentDockedStatus ||
                    KantenList[i].DestKnotenUc.Equals(this) && KantenList[i].StatusDestElement == currentDockedStatus)
                {
                    index++;
                }                        
                
            }
            return index;
        }

        public readonly List<KanteUc> KantenList = new List<KanteUc>();
    }
}
