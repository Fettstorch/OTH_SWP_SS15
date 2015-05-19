using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphFramework.Interfaces;
using UseCaseAnalyser.GraphVisualiser.DrawingElements;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.GraphVisualiser
{
    /// <summary>
    ///     Interaction logic for GraphVisualiser.xaml
    /// </summary>
    public partial class GraphVisualiser
    {
        #region properties
        private const double ElementWidth = 80;
        private const double ElementHeight = 65;
        private readonly List<UseCaseNode> mNodes = new List<UseCaseNode>();
        private Point mOffsetElementPosition;
        private FrameworkElement mSelectedElement;

        /// <summary>
        /// Binding configuration for a dependecy property which is  setting UseCaseGraph to display
        /// </summary>
        public static readonly DependencyProperty UseCaseProperty = DependencyProperty.Register("UseCase",
        typeof(UseCaseGraph), typeof(GraphVisualiser), new PropertyMetadata(UseCase_PropertyChanged));

        /// <summary>
        /// Binding configuration for a dependecy property which is setting a scenario graph (which will be highlighted by GraphVisualiser)
        /// </summary>
        public static readonly DependencyProperty ScenarioProperty = DependencyProperty.Register("Scenario",
            typeof(IGraph), typeof(GraphVisualiser), new PropertyMetadata(Scenario_PropertyChanged));

        /// <summary>
        /// Binding configuration for a dependecy property which is setting the currently selected IGraphElement (INode/IEdge/IGraph) in GraphVisualiser
        /// </summary>
        public static readonly DependencyProperty GraphElementProperty = DependencyProperty.Register("GraphElement",
            typeof(IGraphElement), typeof(GraphVisualiser));


        /// <summary>
        /// Dependency property for currently selected scenario graph
        /// </summary>
        public IGraph Scenario
        {
            get { return GetValue(ScenarioProperty) as IGraph; }
            set { SetValue(ScenarioProperty, value); }
        }

        /// <summary>
        /// Dependency property for currently selected IGraphElement
        /// </summary>
        public IGraphElement GraphElement
        {
            get { return GetValue(GraphElementProperty) as IGraphElement; }
            set { SetValue(GraphElementProperty, value); }
        }

        /// <summary>
        /// Dependency property for use case graph that should be visualised
        /// </summary>
        public UseCaseGraph UseCase
        {
            get { return GetValue(UseCaseProperty) as UseCaseGraph; }
            set { SetValue(UseCaseProperty, value); }
        }

        private static void Scenario_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //  MARK THE NODES WITHIN THE SCENARIO

            //  ACCESS MEMBER VIA DEPENDENCY OBJECT
            GraphVisualiser visualizer = (GraphVisualiser)d;
        }

        private static void UseCase_PropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            //  ACCESS MEMBER VIA DEPENDENCY OBJECT
            GraphVisualiser visualizer = (GraphVisualiser)d;
            visualizer.Clear();
            try
            {
                //  REDRAW (NEW GRAPH)
                visualizer.VisualiseNodes();
                visualizer.VisualiseEdges();
            }
            catch
            {
                //Todo Log
            }

            visualizer.GraphElement = (IGraphElement)e.NewValue;
        }

        #endregion
        
        /// <summary>
        /// GraphVisualiser default constructor
        /// </summary>
        public GraphVisualiser()
        {
            InitializeComponent();
        }
        
        private void Clear()
        {
            mNodes.Clear();
            DrawingCanvas.Children.Clear();
        }

        private void VisualiseNodes()
        {
            //first add all nodes contained in UseCaseGraph to visualiser
            foreach (INode ucNode in UseCase.Nodes)
            {
                //get node's attribute named "Index"
                IAttribute ucNodeAttribut =
                    ucNode.Attributes.First(
                        a => a.Name == WordImporter.UseCaseNodeAttributeNames[(int) WordImporter.UseCaseNodeAttributes.Index]);

                //parse Index value
                List<string> results = IndexParser(ucNodeAttribut.Value.ToString());
                switch (results.Count)
                {
                    case 1:
                        //if regex returns list with one element the node is a main sequence node - add it to first row
                        AddNode(1, ucNode);
                        break;
                    case 3:
                        //if regex returns list with three elements the node is a variant sequence node
                        //calculate row by converting character to int (only first char is converted at the moment)
                        //furthermore first entry in list marks reference node (node where variant sequences branches
                        AddNode((uint) (char.ToUpper(results[1][0]) - 63), ucNode,
                            UseCase.Nodes.FirstOrDefault(
                                node =>
                                    node.Attributes.Any(
                                        attr =>
                                            attr.Name.Equals(
                                                WordImporter.UseCaseNodeAttributeNames[
                                                    (int) WordImporter.UseCaseNodeAttributes.Index]) &&
                                            ((string) attr.Value).Equals(results[0]))));
                        break;
                    default:
                        throw new InvalidOperationException("Extraction of index failed.");
                }
            }
        }

        private void VisualiseEdges()
        {
            foreach (IEdge ucEdge in UseCase.Edges)
            {
                UseCaseNode firstNode = null;
                UseCaseNode secondNode = null;
                foreach (UseCaseNode ucNode in mNodes)
                {
                    IAttribute ucNodeIndexAttr =
                        ucNode.Node.Attributes.First(attr =>attr.Name == WordImporter.UseCaseNodeAttributeNames[(int)WordImporter.UseCaseNodeAttributes.Index]);

                    if (firstNode == null &&
                        ucNodeIndexAttr.Value == ucEdge.Node1.Attributes.First( 
                        attr =>attr.Name == WordImporter.UseCaseNodeAttributeNames[(int) WordImporter.UseCaseNodeAttributes.Index]).Value)
                        firstNode = ucNode;
                    if (secondNode == null && ucNodeIndexAttr.Value ==
                        ucEdge.Node2.Attributes.First( attr => attr.Name == WordImporter.UseCaseNodeAttributeNames[(int) WordImporter.UseCaseNodeAttributes.Index]).Value)
                        secondNode = ucNode;
                }

                if (firstNode == null && secondNode == null)
                    throw new InvalidOperationException("Edge could not be added, because at least one node does not exist in GraphVisualiser.");

                AddEdge(firstNode, secondNode, ucEdge);
            }
        }

        private List<string> IndexParser(string index)
        {
            Regex regex = new Regex(@"([0-9]+)([A-z]+)([0-9]+)");
            //split method adds empty entries in front and in the end of list - therefore remove empty entries
            return regex.Split(index).Where(s => s != String.Empty).ToList();
        }

        private void AddNode(uint slotNumber, INode node, INode referenceUseCaseNode = null)
        {
            UseCaseNode useCaseNode = new UseCaseNode(slotNumber, node);
            useCaseNode.PreviewMouseLeftButtonDown += GraphVisualiser_OnMouseDown;
            DrawingCanvas.Children.Add(useCaseNode);
            Panel.SetZIndex(useCaseNode, 10);
            
            if (referenceUseCaseNode != null)
            {
                UseCaseNode referencenode = mNodes.Single(n => n.Node.Equals(referenceUseCaseNode));
                useCaseNode.YOffset = referencenode.YOffset;
            }


            foreach (UseCaseNode ucNode in mNodes)
            {
                if (ucNode.SlotNumber == slotNumber && ucNode.YOffset > useCaseNode.YOffset)
                    useCaseNode.YOffset = ucNode.YOffset;
            }
            if (mNodes.Count > 0)
                useCaseNode.YOffset += ElementHeight;

            mNodes.Add(useCaseNode);
            Canvas.SetTop(useCaseNode, useCaseNode.YOffset);
            Canvas.SetLeft(useCaseNode, ElementWidth*(slotNumber - 1) + 40);
            
            if (DrawingCanvas.Width < (ElementWidth)*(slotNumber) + 40)
                DrawingCanvas.Width = (ElementWidth)*(slotNumber) + 40;
            if (DrawingCanvas.Height < (useCaseNode.YOffset + ElementHeight))
                DrawingCanvas.Height = useCaseNode.YOffset + ElementHeight;

        }

        private void AddEdge(UseCaseNode firstNode, UseCaseNode secondNode, IEdge edge)
        {
            if (firstNode == null || secondNode == null)
                return;
            UseCaseEdge useCaseEdge = new UseCaseEdge(firstNode, secondNode, edge);
            useCaseEdge.PreviewMouseLeftButtonDown += GraphVisualiser_OnMouseDown;
            Panel.SetZIndex(useCaseEdge, 1);

            DrawingCanvas.Children.Add(useCaseEdge);

            if (firstNode.SlotNumber < secondNode.SlotNumber)
                secondNode.YOffset = firstNode.YOffset + ElementHeight;

            firstNode.RenderEdges();
            secondNode.RenderEdges();
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

        
     

        #region events
        private void Background_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (UIElement child in DrawingCanvas.Children)
            {
                if (child is ISelectableObject)
                    ((ISelectableObject)child).Unselect();
            }
            GraphElement = UseCase;
        }

        private void GraphVisualiser_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                if (element is ISelectableObject)
                {
                    ((ISelectableObject)element).Select();
                    GraphElement = ((ISelectableObject)element).CurrentElement;

                    foreach (UIElement child in DrawingCanvas.Children)
                    {
                        if (child is ISelectableObject && (ISelectableObject)child != (ISelectableObject)element)
                            ((ISelectableObject)child).Unselect();
                    }

                    mSelectedElement = element;
                    mOffsetElementPosition = Mouse.GetPosition(DrawingCanvas);
                    Point elementPoint = new Point(Canvas.GetLeft(mSelectedElement), Canvas.GetTop(mSelectedElement));
                    mOffsetElementPosition.X -= elementPoint.X;
                    mOffsetElementPosition.Y -= elementPoint.Y;
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

        private void GraphVisualiser_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mSelectedElement != null)
            {
                foreach (FrameworkElement element in DrawingCanvas.Children)
                {
                    if (!element.Equals(mSelectedElement))
                        continue;
                    //Canvas.SetTop(fe, e.GetPosition(this).Y - offsetElementPosition.Y);
                    //Canvas.SetLeft(fe, e.GetPosition(this).X - offsetElementPosition.X);
                    UseCaseNode node = element as UseCaseNode;
                    if (node != null)
                        node.RenderEdges();
                    mSelectedElement = null;
                    break;
                }
            }
        }
        #endregion
       
    }
}