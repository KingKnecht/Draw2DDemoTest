using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZoomAndPan;

namespace Draw2DControlLibrary
{
    public partial class ZoomAndPanControl
    {
        private void ZoomAndPanControl_EventHandlers_OnApplyTemplate()
        {

            _partDragZoomBorder = this.Template.FindName("PART_DragZoomBorder", this) as Border;
            _partDragZoomCanvas = this.Template.FindName("PART_DragZoomCanvas", this) as Canvas;
            MouseDown += ZoomAndPanControl_MouseDown;
            MouseUp += ZoomAndPanControl_MouseUp;
            MouseMove += ZoomAndPanControl_MouseMove;
            MouseWheel += ZoomAndPanControl_MouseWheel;
            MouseDoubleClick += ZoomAndPanControl_MouseDoubleClick;

            FitCommandDepProp = FitCommand;
            FillCommandDepProp = FillCommand;
            OneHundredPercentCommandDepProp = OneHundredPercentCommand;
            ZoomInCommandDepProp = ZoomInCommand;
            ZoomOutCommandDepProp = ZoomOutCommand;
            UndoZoomCommandDepProp = UndoZoomCommand;
            RedoZoomCommandDepProp = RedoZoomCommand;
        }

        /// <summary>
        /// The control for creating a zoom border
        /// </summary>
        private Border _partDragZoomBorder;

        /// <summary>
        /// The control for containing a zoom border
        /// </summary>
        private Canvas _partDragZoomCanvas;

        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode _mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point _origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point _origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton _mouseButtonDown;

        /// <summary>
        /// Event raised on mouse down in the ZoomAndPanControl.
        /// </summary>
        private void ZoomAndPanControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveZoom();
            _content.Focus();
            Keyboard.Focus(_content);

            _mouseButtonDown = e.ChangedButton;
            _origZoomAndPanControlMouseDownPoint = e.GetPosition(this);
            _origContentMouseDownPoint = e.GetPosition(_content);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                _mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (_mouseButtonDown == MouseButton.Left)
            {
                // Just a plain old left-down initiates panning mode.
                _mouseHandlingMode = MouseHandlingMode.Panning;
            }

            if (_mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                this.CaptureMouse();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse up in the ZoomAndPanControl.
        /// </summary>
        private void ZoomAndPanControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseHandlingMode != MouseHandlingMode.None)
            {
                if (_mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (_mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn(_origContentMouseDownPoint);
                    }
                    else if (_mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut(_origContentMouseDownPoint);
                    }
                }
                else if (_mouseHandlingMode == MouseHandlingMode.DragZooming)
                {
                    var finalContentMousePoint = e.GetPosition(_content);
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    ApplyDragZoomRect(finalContentMousePoint);
                }

                this.ReleaseMouseCapture();
                _mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the ZoomAndPanControl.
        /// </summary>
        private void ZoomAndPanControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseHandlingMode == MouseHandlingMode.Panning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                var curContentMousePoint = e.GetPosition(_content);
                var dragOffset = curContentMousePoint - _origContentMouseDownPoint;

                this.ContentOffsetX -= dragOffset.X;
                this.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (_mouseHandlingMode == MouseHandlingMode.Zooming)
            {
                var curZoomAndPanControlMousePoint = e.GetPosition(this);
                var dragOffset = curZoomAndPanControlMousePoint - _origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (_mouseButtonDown == MouseButton.Left &&
                    (Math.Abs(dragOffset.X) > dragThreshold ||
                     Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    //
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a rectangle to select the area
                    // to zoom in on.
                    //
                    _mouseHandlingMode = MouseHandlingMode.DragZooming;
                    var curContentMousePoint = e.GetPosition(_content);
                    InitDragZoomRect(_origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (_mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                //
                // When in drag zooming mode continously update the position of the rectangle
                // that the user is dragging out.
                //
                var curContentMousePoint = e.GetPosition(this);
                SetDragZoomRect(_origZoomAndPanControlMouseDownPoint, curContentMousePoint);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel
        /// </summary>
        private void ZoomAndPanControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            DelayedSaveZoom750Miliseconds();
            e.Handled = true;

            if (e.Delta > 0)
                ZoomIn(e.GetPosition(_content));
            else if (e.Delta < 0)
                ZoomOut(e.GetPosition(_content));
        }

        /// <summary>
        /// Event raised with the double click command
        /// </summary>
        private void ZoomAndPanControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                SaveZoom();
                this.AnimatedSnapTo(e.GetPosition(_content));
            }
        }

        #region private Zoom methods
        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            this.ZoomAboutPoint(this.ViewportZoom * 0.90909090909, contentZoomCenter);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            this.ZoomAboutPoint(this.ViewportZoom * 1.1, contentZoomCenter);
        }

        /// <summary>
        /// Initialise the rectangle that the use is dragging out.
        /// </summary>
        private void InitDragZoomRect(Point pt1, Point pt2)
        {
            _partDragZoomCanvas.Visibility = Visibility.Visible;
            _partDragZoomBorder.Opacity = 1;
            SetDragZoomRect(pt1, pt2);
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from content coordinates.
            //
            var rect = Helpers.Clamp(new Point(0, 0), new Point(_partDragZoomCanvas.ActualWidth, _partDragZoomCanvas.ActualHeight),
                 pt1, pt2);
            Helpers.PositionBorderOnCanvas(_partDragZoomBorder, rect);
        }

        /// <summary>
        /// When the user has finished dragging out the rectangle the zoom operation is applied.
        /// </summary>
        private void ApplyDragZoomRect(Point finalContentMousePoint)
        {
            var rect = Helpers.Clamp(new Point(0, 0), new Point(_partDragZoomCanvas.ActualWidth, _partDragZoomCanvas.ActualHeight),
                 finalContentMousePoint, _origContentMouseDownPoint);
            this.AnimatedZoomTo(rect);
            // new Rect(contentX, contentY, contentWidth, contentHeight));
            FadeOutDragZoomRect();
        }

        //
        // Fade out the drag zoom rectangle.
        //
        private void FadeOutDragZoomRect()
        {
            AnimationHelper.StartAnimation(_partDragZoomBorder, OpacityProperty, 0.0, 0.1,
                delegate { _partDragZoomCanvas.Visibility = Visibility.Collapsed; });
        }

        #endregion

        #region Command DependencyProperties

        public static readonly DependencyProperty FitCommandDepPropProperty = DependencyProperty.Register(
            "FitCommandDepProp", typeof (ICommand), typeof (ZoomAndPanControl), new PropertyMetadata(default(ICommand)));

        public ICommand FitCommandDepProp
        {
            get { return (ICommand) GetValue(FitCommandDepPropProperty); }
            set { SetValue(FitCommandDepPropProperty, value); }
        }

        public static readonly DependencyProperty FillCommandDepPropProperty = DependencyProperty.Register(
            "FillCommandDepProp", typeof (ICommand), typeof (ZoomAndPanControl), new PropertyMetadata(default(ICommand)));

        public ICommand FillCommandDepProp
        {
            get { return (ICommand) GetValue(FillCommandDepPropProperty); }
            set { SetValue(FillCommandDepPropProperty, value); }
        }


        public static readonly DependencyProperty OneHundredPercentCommandDepPropProperty = DependencyProperty.Register(
            "OneHundredPercentCommandDepProp", typeof (ICommand), typeof (ZoomAndPanControl), new PropertyMetadata(default(ICommand)));

        public ICommand OneHundredPercentCommandDepProp
        {
            get { return (ICommand) GetValue(OneHundredPercentCommandDepPropProperty); }
            set { SetValue(OneHundredPercentCommandDepPropProperty, value); }
        }

        public static readonly DependencyProperty ZoomInCommandDepPropProperty = DependencyProperty.Register(
            "ZoomInCommandDepProp", typeof (ICommand), typeof (ZoomAndPanControl), new PropertyMetadata(default(ICommand)));

        public ICommand ZoomInCommandDepProp
        {
            get { return (ICommand) GetValue(ZoomInCommandDepPropProperty); }
            set { SetValue(ZoomInCommandDepPropProperty, value); }
        }

        public static readonly DependencyProperty ZoomOutCommandDepPropProperty = DependencyProperty.Register(
            "ZoomOutCommandDepProp", typeof (ICommand), typeof (ZoomAndPanControl), new PropertyMetadata(default(ICommand)));

        public ICommand ZoomOutCommandDepProp
        {
            get { return (ICommand) GetValue(ZoomOutCommandDepPropProperty); }
            set { SetValue(ZoomOutCommandDepPropProperty, value); }
        }

        public static readonly DependencyProperty UndoZoomCommandDepPropProperty = DependencyProperty.Register(
            "UndoZoomCommandDepProp", typeof (ICommand), typeof (ZoomAndPanControl), new PropertyMetadata(default(ICommand)));

        public ICommand UndoZoomCommandDepProp
        {
            get { return (ICommand) GetValue(UndoZoomCommandDepPropProperty); }
            set { SetValue(UndoZoomCommandDepPropProperty, value); }
        }

        public static readonly DependencyProperty RedoZoomCommandDepPropProperty = DependencyProperty.Register(
            "RedoZoomCommandDepProp", typeof (ICommand), typeof (ZoomAndPanControl), new PropertyMetadata(default(ICommand)));

        public ICommand RedoZoomCommandDepProp
        {
            get { return (ICommand) GetValue(RedoZoomCommandDepPropProperty); }
            set { SetValue(RedoZoomCommandDepPropProperty, value); }
        }

        #endregion


        #region Commands

        /// <summary>
        ///     Command to implement the zoom to fill 
        /// </summary>
        public ICommand FillCommand => _fillCommand ?? (_fillCommand = new RelayCommand(() =>
            {
                SaveZoom();
                ZoomTo(ContentFillZoom);
                RaiseCanExecuteChanged();
            }, () => Math.Abs(ViewportZoom - ContentFillZoom) > .01 * ContentFillZoom && ContentFillZoom >= ContentMinZoom));

        private RelayCommand _fillCommand;

        /// <summary>
        ///     Command to implement the zoom to fit 
        /// </summary>
        public ICommand FitCommand => _fitCommand ?? (_fitCommand = new RelayCommand(() =>
            {
                SaveZoom();
                AnimatedZoomTo(ContentFitZoom);
                RaiseCanExecuteChanged();
            }, () => Math.Abs(ViewportZoom - ContentFitZoom) > .01 * ContentFitZoom && ContentFitZoom >= ContentMinZoom));

        private RelayCommand _fitCommand;

        /// <summary>
        ///     Command to implement the zoom to 100% 
        /// </summary>
        public ICommand OneHundredPercentCommand => _oneHundredPercentCommand ?? (_oneHundredPercentCommand = new RelayCommand(() =>
            {
                SaveZoom();
                AnimatedZoomTo(1.0);
                RaiseCanExecuteChanged();
            }, () => Math.Abs(ViewportZoom - 1.0) > .01 && 1 >= ContentMinZoom));

        private RelayCommand _oneHundredPercentCommand;

        /// <summary>
        ///     Command to implement the zoom out by 110% 
        /// </summary>
        public ICommand ZoomOutCommand => _zoomOutCommand ?? (_zoomOutCommand = new RelayCommand(() =>
             {
                 DelayedSaveZoom1500Miliseconds();
                 ZoomOut(new Point(ContentZoomFocusX, ContentZoomFocusY));
             }, () => ViewportZoom > MinimumZoom));

        private RelayCommand _zoomOutCommand;

        /// <summary>
        ///     Command to implement the zoom in by 91% 
        /// </summary>
        public ICommand ZoomInCommand => _zoomInCommand ?? (_zoomInCommand = new RelayCommand(() =>
            {
                DelayedSaveZoom1500Miliseconds();
                ZoomIn(new Point(ContentZoomFocusX, ContentZoomFocusY));
            }, () => ViewportZoom < MaximumZoom));
        private RelayCommand _zoomInCommand;

        private void RaiseCanExecuteChanged()
        {
            _oneHundredPercentCommand?.RaiseCanExecuteChanged();
            _zoomOutCommand?.RaiseCanExecuteChanged();
            _zoomInCommand?.RaiseCanExecuteChanged();
            _fitCommand?.RaiseCanExecuteChanged();
            _fillCommand?.RaiseCanExecuteChanged();
        }
        #endregion
    }
}