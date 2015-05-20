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
        private readonly Dictionary<INode, Point> mNodePosDict = new Dictionary<INode, Point>(); 
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
            set { SetValue(UseCaseProperty, value);}
        }
        /// <summary>
        /// Property changed evented handler for Scenerio property.
        /// If Scenario property is modified GraphVisualiser will highlight them within the view.
        /// </summary>
        /// <param name="d">Dependency object that was changed</param>
        /// <param name="e">Event args containing information about the changes of the Scenario property</param>
        private static void Scenario_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //  MARK THE NODES WITHIN THE SCENARIO

            //  ACCESS MEMBER VIA DEPENDENCY OBJECT
            GraphVisualiser visualizer = (GraphVisualiser)d;
        }

        /// <summary>
        /// Property changed evented handler for UseCase property.
        /// If UseCase property is modified GraphVisualiser will visualise the new UseCaseGraph. 
        /// Furthermore if this UseCaseGraph was already displayed the cached positions are used instead of 
        /// calculating them again from scretch.
        /// </summary>
        /// <param name="d">Dependency object that was changed</param>
        /// <param name="e">Event args containing information about the changes of the UseCase property</param>
        private static void UseCase_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        
        /// <summary>
        /// Creates cache entries (current position) for all INode objects within the UseCaseNodes.
        /// Furthermore clear Canvas and mNodes list.
        /// </summary>
        private void Clear()
        {
            //Save old Position in Dictionary
            foreach (UseCaseNode node in mNodes)
            {
                if (mNodePosDict.ContainsKey(node.Node))
                {
                    mNodePosDict[node.Node] = new Point(Canvas.GetLeft(node), Canvas.GetTop(node));
                }
            }
            mNodes.Clear();
            DrawingCanvas.Children.Clear();
        }

        /// <summary>
        /// Visualise all nodes in dependency property UseCaseGraph by using Index attributes to calculate their corresponding
        /// default slotNumber (node's X-Offset). If Index is corrupted an InvalidOperationException will be thrown.
        /// </summary>
        private void VisualiseNodes()
        {
            //first add all nodes contained in UseCaseGraph to visualiser
            foreach (INode ucNode in UseCase.Nodes)
            {
                //get node's attribute named "Index"
                IAttribute ucNodeAttribut =
                    ucNode.Attributes.First(
                        a => a.Name == UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.Index]);

                //parse Index value
                // Todo Use NodeAttributes NormalIndex, VariantIndex, VarSeqStep instead of IndexParser()
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
                                                UseCaseGraph.AttributeNames[
                                                    (int)UseCaseGraph.NodeAttributes.Index]) &&
                                            ((string) attr.Value).Equals(results[0]))));
                        break;
                    default:
                        throw new InvalidOperationException("Extraction of index failed.");
                }
            }
        }

        /// <summary>
        /// Visualise all edges by using Indices of start and end node to find corresponding UseCaseNodes.
        /// If UseCaseNodes are not contained in mNodes an InvalidOperationException will be thrown.
        /// </summary>
        private void VisualiseEdges()
        {
            foreach (IEdge ucEdge in UseCase.Edges)
            {
                UseCaseNode firstNode = null;
                UseCaseNode secondNode = null;
                foreach (UseCaseNode ucNode in mNodes)
                {
                    IAttribute ucNodeIndexAttr =
                        ucNode.Node.Attributes.First(attr => attr.Name == UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.Index]);

                    if (firstNode == null &&
                        ucNodeIndexAttr.Value == ucEdge.Node1.Attributes.First(
                        attr => attr.Name == UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.Index]).Value)
                        firstNode = ucNode;
                    if (secondNode == null && ucNodeIndexAttr.Value ==
                        ucEdge.Node2.Attributes.First(attr => attr.Name == UseCaseGraph.AttributeNames[(int)UseCaseGraph.NodeAttributes.Index]).Value)
                        secondNode = ucNode;
                }

                if (firstNode == null && secondNode == null)
                    throw new InvalidOperationException("Edge could not be added, because at least one node does not exist in GraphVisualiser.");

                AddEdge(firstNode, secondNode, ucEdge);
            }
        }

        /// <summary>
        /// Parse index by using a regex to split it in either one (NormalNodes) or three (VariantNodes) values.
        /// These values will be used for determine default position.
        /// </summary>
        /// <param name="index">Index string extracted by WordImporter containg NormalIndex, (VariantIndex and VarSeqStep) concatenated.</param>
        /// <returns>List values containing either one value (NormalIndex) for NormalNodes or three values (NormalIndex,VariantIndex,VarSeqStep) for VariantNodes)</returns>
        private List<string> IndexParser(string index)
        {
            Regex regex = new Regex(@"([0-9]+)([A-z]+)([0-9]+)");
            //split method adds empty entries in front and in the end of list - therefore remove empty entries
            return regex.Split(index).Where(s => s != String.Empty).ToList();
        }

        /// <summary>
        /// Adds a node to GraphVisualiser canvas and node list. Furthermore adjusts default position of this node if no cached value is given.
        /// </summary>
        /// <param name="slotNumber">Used to determine X-Offset for node (column).</param>
        /// <param name="node">INode object that should be wrapped within a UseCaseNode.</param>
        /// <param name="referenceUseCaseNode">Used if node is a variant node. Corrensponding reference node is normal node where the variant was branched. Used to determine Y-Offset.</param>
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
            double leftPos = ElementWidth*(slotNumber - 1) + 40;
            double topPos = useCaseNode.YOffset;
  
            // If node will be loaded the first time standard value will be used
            // If the node is already in the Dictionary the old value will be loaded
            if (!mNodePosDict.ContainsKey(node))
            {
                mNodePosDict.Add(node,new Point(leftPos,topPos));
                Canvas.SetTop(useCaseNode, topPos);
                Canvas.SetLeft(useCaseNode, leftPos);
            }
            else
            {
                Canvas.SetTop(useCaseNode, mNodePosDict[node].Y);
                Canvas.SetLeft(useCaseNode, mNodePosDict[node].X);
            }

            if (DrawingCanvas.Width < (ElementWidth)*(slotNumber) + 40)
                DrawingCanvas.Width = (ElementWidth)*(slotNumber) + 40;
            if (DrawingCanvas.Height < (useCaseNode.YOffset + ElementHeight))
                DrawingCanvas.Height = useCaseNode.YOffset + ElementHeight;

        }

        /// <summary>
        /// Adds an edge to Canvas and triggers start and ending node to render their edges.
        /// </summary>
        /// <param name="startNode">Node where edge starts.</param>
        /// <param name="endingNode">Node where edge ends.</param>
        /// <param name="edge">IEdge which will be wrapped within an UseCaseEdge.</param>
        private void AddEdge(UseCaseNode startNode, UseCaseNode endingNode, IEdge edge)
        {
            if (startNode == null || endingNode == null)
                return;
            UseCaseEdge useCaseEdge = new UseCaseEdge(startNode, endingNode, edge);
            useCaseEdge.PreviewMouseLeftButtonDown += GraphVisualiser_OnMouseDown;
            Panel.SetZIndex(useCaseEdge, 1);

            DrawingCanvas.Children.Add(useCaseEdge);

            startNode.RenderEdges();
            endingNode.RenderEdges();
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
        
        //Todo Is there another way to implement this behaviour? If a node is clicked currently this event will first Unselect the node and set UseCase as GraphElement and afterwards these values are overwritten by GraphVisualiser_OnMouseDown event.

        /// <summary>
        /// Event handler for canvas. Unselect all selectable elements in canvas and set dependency property GraphElement to UseCase.
        /// </summary>
        /// <param name="sender">Sender of Background_OnPreviewMouseLeftButtonDown event.</param>
        /// <param name="e">Background_OnPreviewMouseLeftButtonDown mouse button event arguments.</param>
        private void Background_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (UIElement child in DrawingCanvas.Children)
            {
                if (child is ISelectableObject)
                    ((ISelectableObject)child).Unselect();
            }
            GraphElement = UseCase;
        }

        /// <summary>
        /// Event handler for Graphvisualiser. Determines if a selectable UseCaseNode/Egde was pressed and updates GraphElement.
        /// </summary>
        /// <param name="sender">Sender of GraphVisualiser_OnMouseDown event.</param>
        /// <param name="e">GraphVisualiser_OnMouseDown mouse button event arguments.</param>
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

        /// <summary>
        /// Event handler for Graphvisualiser. Handles dragging state.
        /// </summary>
        /// <param name="sender">Sender of GraphVisualiser_OnMouseMove event.</param>
        /// <param name="e">GraphVisualiser_OnMouseMove mouse button event arguments.</param>
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

        /// <summary>
        /// Event handler for Graphvisualiser. Resets dragging state.
        /// </summary>
        /// <param name="sender">Sender of GraphVisualiser_OnMouseUp event.</param>
        /// <param name="e">GraphVisualiser_OnMouseUp mouse button event arguments.</param>
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