﻿#region Copyright information

// <summary>
// <copyright file="UseCaseGraphVisualiser.xaml.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>09/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphFramework.Interfaces;
using LogManager;
using UseCaseAnalyser.GraphVisualiser.DrawingElements;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.GraphVisualiser
{
    /// <summary>
    /// Class for displaying a UseCaseGraph element. It offers possibilities to select a single GraphElement 
    /// while setting a dependency property GraphElement. Furthermore the user has the option to move all nodes.
    /// </summary>
    public partial class UseCaseGraphVisualiser
    {
        #region properties

        private const double ElementWidth = 110;
        private const double ElementHeight = 70;
        private const double ScaleRateZoom = 1.05;
        private readonly List<UseCaseNode> mNodes = new List<UseCaseNode>();
        private readonly Dictionary<INode, Point> mNodePosDict = new Dictionary<INode, Point>();
        private Point mOffsetElementPosition;
        private FrameworkElement mSelectedElement;
        private DateTime mLastSizeUpdateTime = DateTime.Now;
        private const uint CanvasSizeUpdateTime = 20; // in Milliseconds


        /// <summary>
        /// Binding configuration for a dependecy property which is setting UseCaseGraph to display
        /// </summary>
        public static readonly DependencyProperty UseCaseProperty = DependencyProperty.Register("UseCase",
            typeof (UseCaseGraph), typeof (UseCaseGraphVisualiser), new PropertyMetadata(UseCasePropertyChanged));

        /// <summary>
        /// Binding configuration for a dependecy property which is setting a scenario graph (which will be highlighted by UseCaseGraphVisualiser)
        /// </summary>
        public static readonly DependencyProperty ScenarioProperty = DependencyProperty.Register("Scenario",
            typeof (IGraph), typeof (UseCaseGraphVisualiser), new PropertyMetadata(ScenarioPropertyChanged));

        /// <summary>
        /// Binding configuration for a dependecy property which is setting the currently selected IGraphElement (INode/IEdge/IGraph) in UseCaseGraphVisualiser
        /// </summary>
        public static readonly DependencyProperty GraphElementProperty = DependencyProperty.Register("GraphElement",
            typeof (IGraphElement), typeof (UseCaseGraphVisualiser));


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

        /// <summary>
        /// Property changed evented handler for Scenerio property.
        /// If Scenario property is modified UseCaseGraphVisualiser will highlight them within the view.
        /// </summary>
        /// <param name="d">Dependency object that was changed</param>
        /// <param name="e">Event args containing information about the changes of the Scenario property</param>
        private static void ScenarioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //  MARK THE NODES WITHIN THE SCENARIO

            //  ACCESS MEMBER VIA DEPENDENCY OBJECT
            UseCaseGraphVisualiser visualizer = (UseCaseGraphVisualiser) d;

            if (visualizer.Scenario == null)
            {
                LoggingFunctions.Trace("Scenario unselected.");
                return;
            }
            IAttribute nameAttribute = visualizer.Scenario.GetAttributeByName("Name");
            if (nameAttribute != null)
                LoggingFunctions.Trace("Scenario : " + nameAttribute.Value + " was selected.");
            
            visualizer.SetBrushForScenario(visualizer.Scenario, Brushes.Red);
            visualizer.GraphElement = visualizer.Scenario;
        }

        /// <summary>
        /// Property changed evented handler for UseCase property.
        /// If UseCase property is modified UseCaseGraphVisualiser will visualise the new UseCaseGraph. 
        /// Furthermore if this UseCaseGraph was already displayed the cached positions are used instead of 
        /// calculating them again from scratch.
        /// </summary>
        /// <param name="d">Dependency object that was changed</param>
        /// <param name="e">Event args containing information about the changes of the UseCase property</param>
        private static void UseCasePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //  ACCESS MEMBER VIA DEPENDENCY OBJECT
            UseCaseGraphVisualiser visualizer = (UseCaseGraphVisualiser) d;
            visualizer.Clear();
            visualizer.GraphElement = (IGraphElement) e.NewValue;
            if (visualizer.UseCase == null)
            {
                LoggingFunctions.Trace("UseCase unselected.");
                return;
            }
            IAttribute nameAttribute = visualizer.UseCase.GetAttributeByName("Name");
            if (nameAttribute != null)
                LoggingFunctions.Trace("UseCase : " + nameAttribute.Value + " was selected.");

            //  REDRAW (NEW GRAPH)
            visualizer.VisualiseGraph();
        }

        #endregion

        /// <summary>
        /// UseCaseGraphVisualiser default constructor
        /// </summary>
        public UseCaseGraphVisualiser()
        {
            InitializeComponent();
        }


        /// <summary>
        /// redraws the current usecasegraph --> nodes + edges are redrawn
        /// and the cache positon will be deleted
        /// </summary>
        public void RedrawGraph()
        {
            Clear(true);
            VisualiseGraph();
        }

        /// <summary>
        /// Creates cache entries (current position) for all INode objects within the UseCaseNodes.
        /// Furthermore clear Canvas and mNodes list.
        /// </summary>
        /// <param name="clearCache">True:clear cache of nodes | False: save old postion of nodes</param>
        private void Clear(bool clearCache = false)
        {
            if (clearCache)
            {
                LoggingFunctions.Trace("Clear Cache of positions in the canvas of all nodes");
                
                foreach (UseCaseNode node in mNodes)
                {
                    if (mNodePosDict.ContainsKey(node.Node))
                        mNodePosDict.Remove(node.Node);
                }
            }
            // if the cache will not be cleared the position will be saved
            else
            {
                LoggingFunctions.Trace("Save positions in the canvas of all nodes into the cache");
                //Save old Position in Dictionary
                foreach (UseCaseNode node in mNodes)
                {
                    if (mNodePosDict.ContainsKey(node.Node))
                    {
                        mNodePosDict[node.Node] = new Point(Canvas.GetLeft(node), Canvas.GetTop(node));
                    }
                }     
            }
           
            mNodes.Clear();
            DrawingCanvas.Children.Clear();
            DrawingCanvas.Height = DrawingCanvas.Width = 100;
            CanvaScaleTransform.ScaleX = CanvaScaleTransform.ScaleY = 1;
            CanvasScrollViewer.ScrollToLeftEnd();
            CanvasScrollViewer.ScrollToTop();

        }

        /// <summary>
        /// Visiualise all Nodes in Standard Position and redraw edges 
        /// </summary>
        private void VisualiseGraph()
        {
            try
            {
                VisualiseNodes();
            }
            catch
            {
                LoggingFunctions.Error("Error while calculating visualisation of use case nodes occured.");
                throw;
            }

            try
            {
                VisualiseEdges();
            }
            catch
            {
                LoggingFunctions.Error("Error while calculating visualisation of use case egdes occured.");
                throw;
            }
        }

        /// <summary>
        /// Visualise all nodes in dependency property UseCaseGraph.
        /// </summary>
        private void VisualiseNodes()
        {
            //add all nodes contained in UseCaseGraph to visualiser
            foreach (INode ucNode in UseCase.Nodes)
            {
                AddNode(ucNode);
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
                    if (firstNode == null && ucNode.Node.Equals(ucEdge.Node1))
                        firstNode = ucNode;
                    if (secondNode == null && ucNode.Node.Equals(ucEdge.Node2))
                        secondNode = ucNode;
                }

                if (firstNode == null || secondNode == null)
                {
                    InvalidOperationException invalidOperationException =
                        new InvalidOperationException(
                            @"Edge could not be added, because at least one node does not exist in UseCaseGraphVisualiser.");
                    LoggingFunctions.Exception(invalidOperationException);
                    throw invalidOperationException;
                }


                AddEdge(firstNode, secondNode, ucEdge);
            }
        }

        /// <summary>
        /// Adds a node to UseCaseGraphVisualiser canvas and node list. Furthermore adjusts default position of this node using 
        /// NormalIndex/VariantIndex attributes if no cached value is given.
        /// </summary>
        /// <param name="node">INode object that should be wrapped within a UseCaseNode.</param>
        private void AddNode(INode node)
        {
            UseCaseNode useCaseNode = new UseCaseNode(node);
            useCaseNode.PreviewMouseLeftButtonDown += GraphVisualiser_OnMouseDown;
            DrawingCanvas.Children.Add(useCaseNode);
            Panel.SetZIndex(useCaseNode, 10);

            // If the node is already in the Dictionary the old value will be loaded
            // otherwise default value will be calculated
            if (mNodePosDict.ContainsKey(node))
            {
                Canvas.SetTop(useCaseNode, mNodePosDict[node].Y);
                Canvas.SetLeft(useCaseNode, mNodePosDict[node].X);
            }
            else
            {
                double leftPos = 40;
                double topPos = 0;
                uint yOffsetMultiplier;

                // get node's attribute named "NormalIndex"
                IAttribute normalAttribute = node.GetAttributeByName(NodeAttributes.NormalIndex.AttributeName());

                // determine if node is a variant node (node type cannot be used)
                IAttribute variantIndexAttribute = node.GetAttributeByName(NodeAttributes.VariantIndex.AttributeName());

                // check if node is a variant sequence node
                if (variantIndexAttribute != null)
                {
                    // determine y-Offset using variant index (character) which is
                    // "casted" to an integer with offset
                    yOffsetMultiplier = (uint) (char.ToUpper(variantIndexAttribute.Value.ToString()[0]) - 64);

                    // determine reference use case node 
                    INode referenceUseCaseNode = UseCase.Nodes.FirstOrDefault(
                        ucNode => ucNode.Attributes.Any(attr =>
                            attr.Name.Equals(NodeAttributes.NormalIndex.AttributeName()) &&
                            ((string) attr.Value).Equals(normalAttribute.Value)));
                    // get correspronding UseCaseNode
                    UseCaseNode referencenode = mNodes.Single(n => n.Node.Equals(referenceUseCaseNode));

                    // determine x-Offset using variant sequence step
                    uint xOffsetMultiplier;
                    IAttribute varSeqStepAttribute = node.GetAttributeByName(NodeAttributes.VarSeqStep.AttributeName());
                    uint.TryParse(varSeqStepAttribute.Value.ToString(), out xOffsetMultiplier);

                    //top is calculated by using reference node y-offset added to y-offset corresponding to its variant number
                    topPos = Canvas.GetTop(referencenode) + yOffsetMultiplier*ElementHeight;
                    //left only depends on variant sequence number
                    leftPos += xOffsetMultiplier*ElementWidth;
                }
                //otherwise it's a normal sequence node
                else
                {
                    // normal index is used to determine default y-Position
                    uint.TryParse(normalAttribute.Value.ToString(), out yOffsetMultiplier);
                    topPos += (yOffsetMultiplier - 1) * ElementHeight;

                    //skip calculation of variant sequence offset for first node (with index 1)
                    //note: if node are inserted in an wrong order rendering would become corrupted!
                    if (mNodes.Count > 0)
                    {
                        //determine index of previous normal node
                        int ucNodeIndexPrevious;
                        int.TryParse(normalAttribute.Value.ToString(), out ucNodeIndexPrevious);
                        ucNodeIndexPrevious--;

                        //determine reference use case node (previous normal node)
                        INode previousNode = UseCase.Nodes.FirstOrDefault(n => n.GetAttributeByName(NodeAttributes.NormalIndex.AttributeName()).Value.Equals(ucNodeIndexPrevious.ToString()));
                        UseCaseNode referencenode = mNodes.Single(n => n.Node.Equals(previousNode));
                        topPos = Canvas.GetTop(referencenode) + ElementHeight;
                    }
                    //get previous normal node's variant count used to calculate additional offset
                    int variantCount = GetPreviousNodeVariantCount(useCaseNode);

                    //add element height for all elements except first (otherwise margin would be too large)
                    if (mNodes.Count > 0)
                        topPos += (ElementHeight * (variantCount));
                }

                // finally add calculated position to cache
                mNodePosDict.Add(node, new Point(leftPos, topPos));

                //assign calculated position
             Canvas.SetTop(useCaseNode, topPos);
             Canvas.SetLeft(useCaseNode, leftPos);
            }

            //add node to graph visualiser usecase node list
            mNodes.Add(useCaseNode);

            ////resize canvas size (for scrollviewer)
            if (DrawingCanvas.Width < Canvas.GetLeft(useCaseNode) + ElementWidth)
                DrawingCanvas.Width = Canvas.GetLeft(useCaseNode) + ElementWidth;
            if (DrawingCanvas.Height < (Canvas.GetTop(useCaseNode) + ElementHeight))
                DrawingCanvas.Height = Canvas.GetTop(useCaseNode) + ElementHeight;
        }

        /// <summary>
        /// Calculates additional offset corresponding to previous node's variant sequence count.
        /// </summary>
        /// <param name="ucNode">UseCaseNode which predecessor's variant sequence count should be determined.</param>
        /// <returns>Count of previous normal node - if ucNode is not a normal node itself or previous could not be determined count is 0.</returns>
        private int GetPreviousNodeVariantCount(UseCaseNode ucNode)
        {
            // get node's attribute named "NormalIndex"
            IAttribute normalIndexAttribute = ucNode.Node.GetAttributeByName(NodeAttributes.NormalIndex.AttributeName());

            //determine if node is a variant node (node type cannot be used)
            IAttribute variantIndexAttribute = ucNode.Node.GetAttributeByName(NodeAttributes.VariantIndex.AttributeName());

            //if a variant node was specified return 0
            if (variantIndexAttribute != null) 
                return 0;

            //determine index of previous normal node
            int ucNodeIndexPrevious;
            int.TryParse(normalIndexAttribute.Value.ToString(), out ucNodeIndexPrevious);
            ucNodeIndexPrevious--;

            //get previous node
            INode previousNode = UseCase.Nodes.FirstOrDefault(n => n.GetAttributeByName(NodeAttributes.NormalIndex.AttributeName()).Value.Equals(ucNodeIndexPrevious.ToString()));
            if (previousNode == null) return 0;

            //calculate previous node's variant count
            int variantCount = 0;
            foreach (INode orgNode in UseCase.Nodes)
            {
                IAttribute ucNodeVarSeqStepIndexAttribute = orgNode.GetAttributeByName(NodeAttributes.VarSeqStep.AttributeName());

                //check if there are variant by using normal index as compare and only increase count if there exist an edge between normal node and variant node
                //otherwise all variant sequence node would increase amount of variants
                if (ucNodeVarSeqStepIndexAttribute == null) 
                    continue;
                
                IAttribute ucNodeNormalIndexAttribute = orgNode.GetAttributeByName(NodeAttributes.NormalIndex.AttributeName());

                int nodeIndex;
                int.TryParse(ucNodeNormalIndexAttribute.Value.ToString(), out nodeIndex);

                //only check nodes with same index and egdes from normal node to its variant (return jumps should not be counted)
                if (ucNodeIndexPrevious == nodeIndex && UseCase.GetEdges(previousNode, orgNode).ToList().Any(edge => edge.Node1 == previousNode && edge.Node2 == orgNode))
                    variantCount++;
            }

            return variantCount;
        }

        /// <summary>
        /// Color for specific Scenario will be set
        /// </summary>
        /// <param name="sourceGraph">Scenario which will be highlited</param>
        /// <param name="futureBrush">Brush which will be used to highlite the specific scenario</param>
        private void SetBrushForScenario(IGraph sourceGraph, Brush futureBrush)
        {
            foreach (UseCaseNode useCaseNode in mNodes)
            {
                useCaseNode.SetDrawingBrush(sourceGraph.Edges,
                    sourceGraph.Nodes.Contains(useCaseNode.Node) ? futureBrush : Brushes.Black);
                useCaseNode.LblIndex.Foreground=
                     sourceGraph.Nodes.Contains(useCaseNode.Node) ? futureBrush : Brushes.Black;
            }
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

        #region events

        /// <summary>
        /// Event handler for canvas. Unselect all selectable elements in canvas and set dependency property GraphElement to UseCase.
        /// </summary>
        /// <param name="sender">Sender of Background_OnPreviewMouseLeftButtonDown event.</param>
        /// <param name="e">Background_OnPreviewMouseLeftButtonDown mouse button event arguments.</param>
        private void Background_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GraphElement == UseCase)
            return;

            foreach (UIElement child in DrawingCanvas.Children)
            {
                ISelectableGraphElement selectableChild = child as ISelectableGraphElement;
                if (selectableChild != null)
                    selectableChild.Unselect();
            }
            LoggingFunctions.Trace("Use Case Graph selected");
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
            ISelectableGraphElement selectableElement = element as ISelectableGraphElement;
            if (selectableElement == null)
                return;

            selectableElement.Select();
            GraphElement = selectableElement.CurrentElement;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            //[Mathias Schneider, Patrick Schießl] - keep more readable foreach loop instead of using LINQ
            foreach (UIElement child in DrawingCanvas.Children)
            {
                ISelectableGraphElement selectableChild = child as ISelectableGraphElement;
                if (selectableChild != null && selectableChild != selectableElement)
                    selectableChild.Unselect();
            }

            mSelectedElement = element;
            mOffsetElementPosition = Mouse.GetPosition(DrawingCanvas);
            Point elementPoint = new Point(Canvas.GetLeft(mSelectedElement), Canvas.GetTop(mSelectedElement));
            mOffsetElementPosition.X -= elementPoint.X;
            mOffsetElementPosition.Y -= elementPoint.Y;

            e.Handled = true;
        }
    
        /// <summary>
        /// Event handler for Graphvisualiser. Handles dragging state.
        /// </summary>
        /// <param name="sender">Sender of GraphVisualiser_OnMouseMove event.</param>
        /// <param name="e">GraphVisualiser_OnMouseMove mouse button event arguments.</param>
        private void GraphVisualiser_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (mSelectedElement == null)
                return;
            
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            //[Mathias Schneider, Patrick Schießl] - keep more readable foreach loop instead of using LINQ
            double maxHeight = 0, maxWidth = 0;
            foreach (FrameworkElement frameworkElement in DrawingCanvas.Children)
            {
                if (maxHeight < frameworkElement.Height + Canvas.GetTop(frameworkElement) + 20)
                    maxHeight = frameworkElement.Height + Canvas.GetTop(frameworkElement) + 20;

                if (maxWidth < frameworkElement.Width + Canvas.GetLeft(frameworkElement) + 20)
                    maxWidth = frameworkElement.Width + Canvas.GetLeft(frameworkElement) + 20;


                if (!frameworkElement.Equals(mSelectedElement))
                    continue;

                Canvas.SetTop(frameworkElement, e.GetPosition(DrawingCanvas).Y - mOffsetElementPosition.Y);
                Canvas.SetLeft(frameworkElement, e.GetPosition(DrawingCanvas).X - mOffsetElementPosition.X);
                UseCaseNode node = frameworkElement as UseCaseNode;
                if (node != null)
                    node.RenderEdges();
            }


            if ((DateTime.Now - mLastSizeUpdateTime).Milliseconds < CanvasSizeUpdateTime)  
                return;
            
            mLastSizeUpdateTime = DateTime.Now;
            const double borderToStartScroll = 100;
            Point currMousePos = Mouse.GetPosition(CanvasScrollViewer);

            //Mouse is moving with an element on the right side of the canvas -> Increase canvas size and scroll right
            if (currMousePos.X > CanvasScrollViewer.ActualWidth - borderToStartScroll)
            {
                DrawingCanvas.Width = maxWidth + CanvasScrollViewer.ActualWidth - Mouse.GetPosition(CanvasScrollViewer).X;
                CanvasScrollViewer.LineRight();
            }
            // Is Mouse pos in the left side of the canvas -> decrease Canvas size
            else if (currMousePos.X < borderToStartScroll)
            {
                //Scroll viewer is completly left -> set standard value of drawing canvas size
                if (CanvasScrollViewer.HorizontalOffset< 0.1)
                    DrawingCanvas.Width = maxWidth;
                //set max pos of element as canvas size and add Mouse Offset
                else
                    DrawingCanvas.Width = maxWidth + CanvasScrollViewer.ActualWidth - Mouse.GetPosition(CanvasScrollViewer).X;
                CanvasScrollViewer.LineLeft();
            }
            //Mouse is moving with an element on the bottom side of the canvas -> Increase canvas size and scroll down
            if (currMousePos.Y > CanvasScrollViewer.ActualHeight - borderToStartScroll)
            {
                DrawingCanvas.Height = maxHeight + CanvasScrollViewer.ActualHeight - Mouse.GetPosition(CanvasScrollViewer).Y;
                CanvasScrollViewer.LineDown();
            }
            // Is Mouse pos in the up side of the canvas -> decrease Canvas size
            else if (currMousePos.Y < borderToStartScroll)
            {
                //Scroll viewer is completly up -> set standard value of drawing canvas size
                if (CanvasScrollViewer.VerticalOffset < 0.1)
                    DrawingCanvas.Height = maxHeight;
                //set max pos of element as canvas size and add Mouse Offset
                else
                    DrawingCanvas.Height = maxHeight + CanvasScrollViewer.ActualHeight - Mouse.GetPosition(CanvasScrollViewer).Y;
                CanvasScrollViewer.LineUp();
            }
        }


        /// <summary>
        /// Event handler for Graphvisualiser. Resets dragging state.
        /// </summary>
        /// <param name="sender">Sender of GraphVisualiser_OnMouseUp event.</param>
        /// <param name="e">GraphVisualiser_OnMouseUp mouse button event arguments.</param>
        private void GraphVisualiser_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mSelectedElement == null)
                return;
            
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            //[Mathias Schneider, Patrick Schießl] - keep more readable foreach loop instead of using LINQ
            foreach (FrameworkElement element in DrawingCanvas.Children)
            {
                if (!element.Equals(mSelectedElement))
                    continue;
                UseCaseNode node = element as UseCaseNode;
                if (node != null)
                    node.RenderEdges();
                mSelectedElement = null;
                break;
            }
        }

        /// <summary>
        /// Event handler for CanvasScrollViewer. If left Ctrl Key is pressed Canvas Zoom starts
        /// </summary>
        /// <param name="sender">Sender of CanvasScrollViewer_OnMouseWheel event</param>
        /// <param name="e">CanvasScrollViewer_OnMouseWheel mouse wheel event arguments</param>
        private void CanvasScrollViewer_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                return;

            if (e.Delta > 0)
            {
                CanvaScaleTransform.ScaleX *= ScaleRateZoom;
                CanvaScaleTransform.ScaleY *= ScaleRateZoom;
            }
            else
            {
                CanvaScaleTransform.ScaleX /= ScaleRateZoom;
                CanvaScaleTransform.ScaleY /= ScaleRateZoom;
            }
            e.Handled = true;
        }

        #endregion


    }
}