using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Draw2DControlLibrary
{
    /// <summary>
    /// A class that wraps up zooming and panning of it's content.
    /// </summary>
    public class ZoomAndPanViewBox : ContentControl
    {
        #region local fields
        /// <summary>
        /// The control for creating a drag border
        /// </summary>
        private Border _dragBorder;

        /// <summary>
        /// The control for creating a drag border
        /// </summary>
        private Border _sizingBorder;

        /// <summary>
        /// The control for containing a zoom border
        /// </summary>
        private Canvas _viewportCanvas;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode _mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point _origContentMouseDownPoint;
        #endregion

        #region constructor and overrides
        /// <summary>
        /// Static constructor to define metadata for the control (and link it to the style in Generic.xaml).
        /// </summary>
        static ZoomAndPanViewBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomAndPanViewBox), new FrameworkPropertyMetadata(typeof(ZoomAndPanViewBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _dragBorder = this.Template.FindName("PART_DraggingBorder", this) as Border;
            _sizingBorder = this.Template.FindName("PART_SizingBorder", this) as Border;
            _viewportCanvas = this.Template.FindName("PART_Content", this) as Canvas;
            SetBackground(Visual);
            if (_dragBorder != null && _viewportCanvas != null)
            {
                _viewportCanvas.MouseDown += ZoomAndPanControl_MouseDown;
                _viewportCanvas.MouseMove += ZoomAndPanControl_MouseMove;
                _viewportCanvas.MouseUp += ZoomAndPanControl_MouseUp;
                MouseDoubleClick += ZoomAndPanControl_MouseDoubleClick;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (ActualWidth > 0)
                _dragBorder.BorderThickness = new Thickness(_viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Left,
                    _viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Top,
                    _viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Right,
                    _viewportCanvas.ActualWidth / ActualWidth * BorderThickness.Bottom);
        }
        #endregion

        #region Mouse Event Handlers
        private void ZoomAndPanControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GetZoomAndPanControl().SaveZoom();
            _mouseHandlingMode = MouseHandlingMode.Panning;
            _origContentMouseDownPoint = e.GetPosition(_viewportCanvas);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                // Shift + left- or right-down initiates zooming mode.
                _mouseHandlingMode = MouseHandlingMode.DragZooming;
                _dragBorder.Visibility = Visibility.Hidden;
                _sizingBorder.Visibility = Visibility.Visible;
                Canvas.SetLeft(_sizingBorder, _origContentMouseDownPoint.X);
                Canvas.SetTop(_sizingBorder, _origContentMouseDownPoint.Y);
                _sizingBorder.Width = 0;
                _sizingBorder.Height = 0;
            }
            else
            {
                // Just a plain old left-down initiates panning mode.
                _mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (_mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                _viewportCanvas.CaptureMouse();
                e.Handled = true;
            }
        }

        private void ZoomAndPanControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                var zoomAndPanControl = GetZoomAndPanControl();
                var curContentPoint = e.GetPosition(_viewportCanvas);
                var rect = Helpers.Clamp(new Point(0, 0), new Point(_viewportCanvas.Width, _viewportCanvas.Height),
                    curContentPoint, _origContentMouseDownPoint);
                zoomAndPanControl.AnimatedZoomTo(rect);
                _dragBorder.Visibility = Visibility.Visible;
                _sizingBorder.Visibility = Visibility.Hidden;
            }
            _mouseHandlingMode = MouseHandlingMode.None;
            _viewportCanvas.ReleaseMouseCapture();
            e.Handled = true;
        }

        private void ZoomAndPanControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_mouseHandlingMode == MouseHandlingMode.Panning)
            {
                var curContentPoint = e.GetPosition(_viewportCanvas);
                var rectangleDragVector = curContentPoint - _origContentMouseDownPoint;
                //
                // When in 'dragging rectangles' mode update the position of the rectangle as the user drags it.
                //
                _origContentMouseDownPoint = Helpers.Clamp(e.GetPosition(_viewportCanvas));
                Canvas.SetLeft(_dragBorder, Canvas.GetLeft(_dragBorder) + rectangleDragVector.X);
                Canvas.SetTop(_dragBorder, Canvas.GetTop(_dragBorder) + rectangleDragVector.Y);
            }
            else if (_mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                var curContentPoint = e.GetPosition(_viewportCanvas);
                var rect = Helpers.Clamp(new Point(0, 0), new Point(_viewportCanvas.Width, _viewportCanvas.Height),
                    curContentPoint, _origContentMouseDownPoint);
                Helpers.PositionBorderOnCanvas(_sizingBorder, rect);
            }

            e.Handled = true;
        }

        private void ZoomAndPanControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                var zoomAndPanControl = GetZoomAndPanControl();
                zoomAndPanControl.SaveZoom();
                zoomAndPanControl.AnimatedSnapTo(e.GetPosition(_viewportCanvas));
            }
        }
        #endregion

        #region Background--Visual Brush
        /// <summary>
        /// The X coordinate of the content focus, this is the point that we are focusing on when zooming.
        /// </summary>
        public FrameworkElement Visual
        {
            get { return (FrameworkElement)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }
        public static readonly DependencyProperty VisualProperty = DependencyProperty.Register("Visual",
            typeof(FrameworkElement), typeof(ZoomAndPanViewBox), new FrameworkPropertyMetadata(null, OnVisualChanged));

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ZoomAndPanViewBox)d;
            c.SetBackground(e.NewValue as FrameworkElement);
        }

        private void SetBackground(FrameworkElement frameworkElement)
        {
            frameworkElement = frameworkElement ?? ((ContentControl)DataContext).Content as FrameworkElement;
            var visualBrush = new VisualBrush
            {
                Visual = frameworkElement,
                ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
                TileMode = TileMode.None,
                Stretch = Stretch.Fill
            };

            if (frameworkElement != null) frameworkElement.SizeChanged += (s, e) =>
            {
                _viewportCanvas.Height = frameworkElement.ActualHeight;
                _viewportCanvas.Width = frameworkElement.ActualWidth;
                _viewportCanvas.Background = visualBrush;
            };
        }
        #endregion

        private ZoomAndPanControl GetZoomAndPanControl()
        {
            var zoomAndPanControl = this.DataContext as ZoomAndPanControl;
            if (zoomAndPanControl == null) throw new NullReferenceException("DataContext is not of type ZoomAndPanControl");
            return zoomAndPanControl;
        }
    }
}
