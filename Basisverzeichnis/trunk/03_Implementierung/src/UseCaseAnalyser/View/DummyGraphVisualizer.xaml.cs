using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
using GraphFramework.Interfaces;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.View
{
    /// <summary>
    /// Interaction logic for DummyGraphVisualizer.xaml
    /// </summary>
    public partial class DummyGraphVisualizer : UserControl
    {
        public DummyGraphVisualizer()
        {
            InitializeComponent();
        }
        
        public static readonly DependencyProperty UseCaseProperty = DependencyProperty.Register("UseCase",
            typeof (UseCaseGraph), typeof (DummyGraphVisualizer), new PropertyMetadata(UseCase_PropertyChanged));

        public static readonly DependencyProperty ScenarioProperty = DependencyProperty.Register("Scenario",
            typeof(IGraph), typeof(DummyGraphVisualizer), new PropertyMetadata(Scenario_PropertyChanged));

        public static readonly DependencyProperty GraphElementProperty = DependencyProperty.Register("GraphElement",
            typeof(IGraphElement), typeof(DummyGraphVisualizer));

        public UseCaseGraph UseCase
        {
            get { return GetValue(UseCaseProperty) as UseCaseGraph; }
            set { SetValue(UseCaseProperty, value); }
        }

        public IGraph Scenario
        {
            get { return GetValue(ScenarioProperty) as IGraph; }
            set { SetValue(ScenarioProperty, value); }
        }

        public IGraphElement GraphElement
        {
            get { return GetValue(GraphElementProperty) as IGraphElement; }
            set { SetValue(GraphElementProperty, value); }
        }


        private static void UseCase_PropertyChanged(DependencyObject d, 
        DependencyPropertyChangedEventArgs e)
        {
            //  REDRAW (NEW GRAPH)

            //  ACCESS MEMBER VIA DEPENDENCY OBJECT
            DummyGraphVisualizer visualizer = (DummyGraphVisualizer)d;
            visualizer.GraphElement = null;

        }

        private static void Scenario_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //  MARK THE NODES WITHIN THE SCENARIO

            //  ACCESS MEMBER VIA DEPENDENCY OBJECT
            DummyGraphVisualizer visualizer = (DummyGraphVisualizer)d;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //  ANYWHERE --> CHANGE THE GRAPH ELEMENT --> WHEN USER CLICKS A NODE, EDGE

            if (UseCase != null)
            {
                GraphElement = UseCase.Nodes.First();
            }
            else
            {
                MessageBox.Show("No use case selected!");
            }
        }
    }
}
