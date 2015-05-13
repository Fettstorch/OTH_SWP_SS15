using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GraphFramework.Interfaces;
using UseCaseAnalyer.Stubs;
using UseCaseAnalyser.GraphVisualiser.DrawingElements;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.GraphVisualiser
{
    /// <summary>
    ///     Interaction logic for GraphVisualiser.xaml
    /// </summary>
    public partial class GraphVisualiser : UserControl
    {
        private const double ElementWidth = 80;
        private const double ElementHeight = 65;
        private readonly List<UseCaseNode> mNodes = new List<UseCaseNode>();
        private Point mOffsetElementPosition;
        private FrameworkElement mSelectedElement;

        /// <summary>
        ///     GraphVisualiser default constructor
        /// </summary>
        public GraphVisualiser()
        {
            InitializeComponent();
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

        public UseCaseGraph UseCase
        {
            get { return GetValue(UseCaseProperty) as UseCaseGraph; }
            set { SetValue(UseCaseProperty, value); }
        }

        private static void Scenario_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //  MARK THE NODES WITHIN THE SCENARIO

            //  ACCESS MEMBER VIA DEPENDENCY OBJECT
            GraphVisualiser visualizer = (GraphVisualiser) d;
        }

        private static void UseCase_PropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            //  ACCESS MEMBER VIA DEPENDENCY OBJECT
            GraphVisualiser visualizer = (GraphVisualiser) d;
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

            visualizer.GraphElement = (IGraphElement) e.NewValue;
        }

        public void Clear()
        {
            mNodes.Clear();
            DrawingCanvas.Children.Clear();
        }

        public void VisualiseNodes()
        {
            //first add all nodes contained in UseCaseGraph to visualiser
            foreach (INode ucNode in UseCase.Nodes)
            {
                //get node's attribute named "Index"
                IAttribute ucNodeAttribut =
                    ucNode.Attributes.First(
                        a => a.Name == WordImporter.UseCaseNodeAttributes[(int) WordImporter.UseCaseNodeAttribute.Index]);

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
                            UseCaseGraphStub.GetInstance().mDummy1[0].Nodes.FirstOrDefault(
                                node =>
                                    node.Attributes.Any(
                                        attr =>
                                            attr.Name.Equals(
                                                WordImporter.UseCaseNodeAttributes[
                                                    (int) WordImporter.UseCaseNodeAttribute.Index]) &&
                                            ((string) attr.Value).Equals(results[0]))));
                        break;
                    default:
                        throw new InvalidOperationException("Extraction of index failed.");
                }
            }
        }

        public void VisualiseEdges()
        {
            foreach (IEdge ucEdge in UseCase.Edges)
            {
                UseCaseNode firstNode = null;
                UseCaseNode secondNode = null;
                foreach (UseCaseNode ucNode in mNodes)
                {
                    IAttribute ucNodeIndexAttr =
                        ucNode.Node.Attributes.First(
                            attr =>
                                attr.Name ==
                                WordImporter.UseCaseNodeAttributes[(int) WordImporter.UseCaseNodeAttribute.Index]);

                    if (firstNode == null &&
                        ucNodeIndexAttr.Value ==
                        ucEdge.Node1.Attributes.First(
                            attr =>
                                attr.Name ==
                                WordImporter.UseCaseNodeAttributes[(int) WordImporter.UseCaseNodeAttribute.Index]).Value)
                        firstNode = ucNode;
                    if (secondNode == null &&
                        ucNodeIndexAttr.Value ==
                        ucEdge.Node2.Attributes.First(
                            attr =>
                                attr.Name ==
                                WordImporter.UseCaseNodeAttributes[(int) WordImporter.UseCaseNodeAttribute.Index]).Value)
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

        public UseCaseNode AddNode(uint slotNumber, INode node, INode referenceUseCaseNode = null)
        {
            UseCaseNode useCaseNode = new UseCaseNode(slotNumber, node);
            useCaseNode.PreviewMouseLeftButtonDown += GraphVisualiser_OnMouseDown;
            DrawingCanvas.Children.Add(useCaseNode);


            if (referenceUseCaseNode != null)
            {
                //ToDo: Investigate why reference compare does not work.
                //referencenode=null = mNodes.Single(n => n.Node.Equals(referenceUseCaseNode));
                UseCaseNode referencenode = mNodes.Single(n => n.Node.Attributes.Single(a => a.Name == WordImporter.
                    UseCaseNodeAttributes[(int) WordImporter.UseCaseNodeAttribute.Index])
                    .Value.Equals(referenceUseCaseNode.Attributes.
                        Single(
                            a =>
                                a.Name ==
                                WordImporter.UseCaseNodeAttributes[(int) WordImporter.UseCaseNodeAttribute.Index]).Value));


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

            return useCaseNode;
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

        public UseCaseEdge AddEdge(UseCaseNode firstNode, UseCaseNode secondNode, IEdge edge)
        {
            if (firstNode == null || secondNode == null)
                return null;
            UseCaseEdge useCaseEdge = new UseCaseEdge(firstNode, secondNode, edge);
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
                if (element is ISelectableObject)
                {
                    ((ISelectableObject) element).ChangeSelection();
                    GraphElement = ((ISelectableObject) element).CurrentElement;

                    foreach (UIElement child in DrawingCanvas.Children)
                    {
                        if (child is ISelectableObject && (ISelectableObject)child != (ISelectableObject)element)
                            ((ISelectableObject)child).Unselect();
                    }
                }                
    
                mSelectedElement = element;
                mOffsetElementPosition = Mouse.GetPosition(DrawingCanvas);
                Point elementPoint = new Point(Canvas.GetLeft(mSelectedElement), Canvas.GetTop(mSelectedElement));
                mOffsetElementPosition.X -= elementPoint.X;
                mOffsetElementPosition.Y -= elementPoint.Y;
            }
            else
            {
                foreach (UIElement child in DrawingCanvas.Children)
                {
                    if (child is ISelectableObject)
                        ((ISelectableObject)child).Unselect();
                }
                GraphElement = UseCase;
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

        public static readonly DependencyProperty UseCaseProperty = DependencyProperty.Register("UseCase",
            typeof (UseCaseGraph), typeof (GraphVisualiser), new PropertyMetadata(UseCase_PropertyChanged));

        public static readonly DependencyProperty ScenarioProperty = DependencyProperty.Register("Scenario",
            typeof (IGraph), typeof (GraphVisualiser), new PropertyMetadata(Scenario_PropertyChanged));

        public static readonly DependencyProperty GraphElementProperty = DependencyProperty.Register("GraphElement",
            typeof (IGraphElement), typeof (GraphVisualiser));


    }
}