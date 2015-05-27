#region Copyright information

// <summary>
// <copyright file="CappedLine.cs">Copyright (c) 2015</copyright>
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
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UseCaseAnalyser.GraphVisualiser.DrawingElements
{
    /// <summary>
    /// Class for displaying a capped line.
    /// Sources from http://blogs.msdn.com/b/mrochon/archive/2011/01/10/custom-line-caps-in-wpf.aspx
    /// </summary>
    internal class CappedLine : FrameworkElement
    {
        /// <summary>
        /// Property for setting and getting Stroke
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// Property for setting and getting StrokeThickness
        /// </summary>
        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Property for setting and getting LinePath
        /// </summary>
        public PathGeometry LinePath
        {
            get { return (PathGeometry) GetValue(LinePathProperty); }
            set { SetValue(LinePathProperty, value); }
        }

        /// <summary>
        /// Property for setting and getting cap at the start of the LinePath
        /// </summary>
        public Geometry BeginCap
        {
            get { return (Geometry) GetValue(BeginCapProperty); }
            set { SetValue(BeginCapProperty, value); }
        }

        /// <summary>
        /// Property for setting and getting cap at the end of the LinePath
        /// </summary>
        public Geometry EndCap
        {
            get { return (Geometry) GetValue(EndCapProperty); }
            set { SetValue(EndCapProperty, value); }
        }

        /// <summary>
        /// Logic for rendering a capped line
        /// </summary>
        /// <param name="dc">DrawingContex of the rendering capped line</param>
        protected override void OnRender(DrawingContext dc)
        {
            Point pos, tangent;
            double angleInRadians;
            double angleInDegrees;
            TransformGroup tg;
            Pen pen = new Pen(Stroke, StrokeThickness);
            dc.DrawGeometry(null, pen, LinePath);
            if (BeginCap != null)
            {
                LinePath.GetPointAtFractionLength(0.01d, out pos, out tangent);
                angleInRadians = Math.Atan2(tangent.Y, tangent.X) + Math.PI;
                angleInDegrees = angleInRadians*180/Math.PI + 180;
                tg = new TransformGroup();
                tg.Children.Add(new RotateTransform(angleInDegrees));
                LinePath.GetPointAtFractionLength(0.0d, out pos, out tangent);
                tg.Children.Add(new TranslateTransform(pos.X, pos.Y));
                dc.PushTransform(tg);
                dc.DrawGeometry(Stroke, pen, BeginCap);
                dc.Pop();
            }
            // ReSharper disable once InvertIf
            // [Mathias Schneider] keep original code
            if (EndCap != null)
            {
                LinePath.GetPointAtFractionLength(0.99, out pos, out tangent);
                angleInRadians = Math.Atan2(tangent.Y, tangent.X) + Math.PI;
                angleInDegrees = angleInRadians*180/Math.PI;
                tg = new TransformGroup();
                tg.Children.Add(new RotateTransform(angleInDegrees));
                LinePath.GetPointAtFractionLength(1, out pos, out tangent);
                tg.Children.Add(new TranslateTransform(pos.X, pos.Y));
                dc.PushTransform(tg);
                dc.DrawGeometry(Stroke, pen, EndCap);
            }
        }

        /// <summary>
        /// Overrides how size is measured
        /// </summary>
        /// <param name="availableSize">Size that is available</param>
        /// <returns>New measured size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            //TODO: Consider creating the Pen once when Stroke and StrokeThickness are set
            return LinePath.GetRenderBounds(new Pen(Stroke, StrokeThickness)).Size;
        }

        /// <summary>
        /// Callback handler used for LinePath dependency property
        /// </summary>
        /// <param name="sender">Dependency object that was changed</param>
        /// <param name="args">Event args containing information about the changes of the LinePath property</param>
        private static void LinePathChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CappedLine me = sender as CappedLine;
            if (null != me)
            {
                me.OnLinePathChanged((PathGeometry) args.NewValue);
            }
        }

        /// <summary>
        /// Property changed evented handler for LinePath property.
        /// </summary>
        /// <param name="value">new PathGeometry value</param>
        public virtual void OnLinePathChanged(PathGeometry value)
        {
            //removed implementation because it causes crashes
        }

        /// <summary>
        /// Callback handler used for BeginCap dependency property
        /// </summary>
        /// <param name="sender">Dependency object that was changed</param>
        /// <param name="args">Event args containing information about the changes of the BeginCap property</param>
        private static void BeginCapChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CappedLine me = sender as CappedLine;
            if (null != me)
            {
                me.OnBeginCapChanged((Geometry) args.NewValue);
            }
        }

        /// <summary>
        /// Property changed evented handler for capped line property at the beginning of the LinePath.
        /// </summary>
        /// <param name="value">new Geometry value</param>
        public virtual void OnBeginCapChanged(Geometry value)
        {
        }

        /// <summary>
        /// Callback handler used for EndCap dependency property
        /// </summary>
        /// <param name="sender">Dependency object that was changed</param>
        /// <param name="args">Event args containing information about the changes of the EndCap property</param>
        private static void EndCapChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CappedLine me = sender as CappedLine;
            if (null != me)
            {
                me.OnEndCapChanged((Geometry) args.NewValue);
            }
        }

        /// <summary>
        /// Property changed evented handler for capped line property at the end of the LinePath.
        /// </summary>
        /// <param name="value">new Geometry value</param>
        public virtual void OnEndCapChanged(Geometry value)
        {
        }

        /// <summary>
        /// Binding configuration for a dependecy property which is setting StrokeProperty
        /// </summary>
        public static readonly DependencyProperty StrokeProperty = Shape.StrokeProperty.AddOwner(typeof (CappedLine));

        /// <summary>
        /// Binding configuration for a dependecy property which is setting StrokeThicknessProperty
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            Shape.StrokeThicknessProperty.AddOwner(typeof (CappedLine));

        /// <summary>
        /// Binding configuration for a dependecy property which is setting LinePathProperty
        /// </summary>
        public static readonly DependencyProperty LinePathProperty =
            DependencyProperty.Register("LinePath", typeof (PathGeometry), typeof (CappedLine),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender, // choose appropriate flags here!!!
                    LinePathChangedCallback));

        /// <summary>
        /// Binding configuration for a dependecy property which is setting BeginCapProperty
        /// </summary>
        public static readonly DependencyProperty BeginCapProperty =
            DependencyProperty.Register("BeginCap", typeof (Geometry), typeof (CappedLine),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender, // choose appropriate flags here!!!
                    BeginCapChangedCallback));

        /// <summary>
        /// Binding configuration for a dependecy property which is setting EndCapProperty
        /// </summary>
        public static readonly DependencyProperty EndCapProperty =
            DependencyProperty.Register("EndCap", typeof (Geometry), typeof (CappedLine),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender, // choose appropriate flags here!!!
                    EndCapChangedCallback));
    }
}