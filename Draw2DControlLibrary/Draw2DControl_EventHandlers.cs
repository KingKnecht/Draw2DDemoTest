using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Draw2D.Core;
using Draw2D.Core.Geo;
using Draw2D.Core.Policies.RouterPolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2DControlLibrary.Commands;
using Point = System.Windows.Point;

namespace Draw2DControlLibrary
{
    partial class Draw2DControl
    {



        public static readonly DependencyProperty CanvasProperty = DependencyProperty.Register(
           "Canvas", typeof(ICanvas), typeof(Draw2DControl), new PropertyMetadata(default(ICanvas), OnCanvasChanged));

        public static readonly DependencyProperty WorldMousePosXProperty = DependencyProperty.Register(
            "WorldMousePosX", typeof(double), typeof(Draw2DControl), new PropertyMetadata(default(double)));

        public double WorldMousePosX
        {
            get { return (double)GetValue(WorldMousePosXProperty); }
            set { SetValue(WorldMousePosXProperty, value); }
        }

        public static readonly DependencyProperty WorldMousePosYProperty = DependencyProperty.Register(
            "WorldMousePosY", typeof(double), typeof(Draw2DControl), new PropertyMetadata(default(double)));

        public double WorldMousePosY
        {
            get { return (double)GetValue(WorldMousePosYProperty); }
            set { SetValue(WorldMousePosYProperty, value); }
        }

        public static readonly DependencyProperty RenderedItemsCountProperty = DependencyProperty.Register(
            "RenderedItemsCount", typeof(int), typeof(Draw2DControl), new PropertyMetadata(default(int)));

        public int RenderedItemsCount
        {
            get { return (int)GetValue(RenderedItemsCountProperty); }
            set { SetValue(RenderedItemsCountProperty, value); }
        }
        
        private static void OnCanvasChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as Draw2DControl;
            if (control == null)
                return;

            if (control._canvas != null)
            {
                control._canvas.SceneChanged -= control.CanvasOnSceneChanged;
                control._canvas.FigureRightClicked -= control.OnFigureRightClicked;
            }
            control._canvas = dependencyPropertyChangedEventArgs.NewValue as ICanvas;

            if (control._canvas != null)
            {
                control._canvas.SceneChanged += control.CanvasOnSceneChanged;
                control._canvas.FigureRightClicked += control.OnFigureRightClicked;
                control.Width = control._canvas.Width;
                control.Height = control._canvas.Height;
                
            }

            control.InvalidateVisual();
        }

        public static readonly DependencyProperty ViewportWidthProperty = DependencyProperty.Register(
            "ViewportWidth", typeof(double), typeof(Draw2DControl), new PropertyMetadata(default(double), ViewportWidthChanged));

        private static void ViewportWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as Draw2DControl;

            if (control?._canvas != null)
            {
                control.Canvas.ViewportWidth = (double)e.NewValue;
            }
        }

        public double ViewportWidth
        {
            get { return (double)GetValue(ViewportWidthProperty); }
            set { SetValue(ViewportWidthProperty, value); }
        }

        public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register(
            "ViewportHeight", typeof(double), typeof(Draw2DControl), new PropertyMetadata(default(double), ViewportHeightChanged));

        private static void ViewportHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as Draw2DControl;

            if (control?._canvas != null)
            {
                control.Canvas.ViewportHeight = (double)e.NewValue;
            }
        }

        public double ViewportHeight
        {
            get { return (double)GetValue(ViewportHeightProperty); }
            set { SetValue(ViewportHeightProperty, value); }
        }

        public static readonly DependencyProperty ContentOffsetXProperty = DependencyProperty.Register(
            "ContentOffsetX", typeof(double), typeof(Draw2DControl), new PropertyMetadata(default(double), ContentOffsetXChanged));

        private static void ContentOffsetXChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as Draw2DControl;

            if (control?._canvas != null)
            {
                control.Canvas.ContentOffsetX = (double)e.NewValue;
            }
        }

        public double ContentOffsetX
        {
            get { return (double)GetValue(ContentOffsetXProperty); }
            set { SetValue(ContentOffsetXProperty, value); }
        }

        public static readonly DependencyProperty ContentOffsetYProperty = DependencyProperty.Register(
            "ContentOffsetY", typeof(double), typeof(Draw2DControl), new PropertyMetadata(default(double), ContentOffsetYChanged));

        private static void ContentOffsetYChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as Draw2DControl;

            if (control?._canvas != null)
            {
                control.Canvas.ContentOffsetY = (double)e.NewValue;
            }
        }

        public double ContentOffsetY
        {
            get { return (double) GetValue(ContentOffsetYProperty); }
            set { SetValue(ContentOffsetYProperty, value); }
        }
        private void OnFigureRightClicked(object sender, FigureClickEventArgs e)
        {
            Dispatcher.Invoke(() => ShowContextMenu(e.Sender, e.MousePosX, e.MousePosY));
        }


      

        private void ShowContextMenu(Figure figure, double mousePosX, double mousePosY)
        {
            var contextMenu = new ContextMenu();

            var menuItem = new MenuItem
            {
                Header = "Red"
            };
            menuItem.Click += (sender, args) => { ((VectorFigure)figure).StrokeColor = Colors.Red; };
            contextMenu.Items.Add(menuItem);

            menuItem = new MenuItem
            {
                Header = "Green"
            };
            menuItem.Click += (sender, args) => { ((VectorFigure)figure).StrokeColor = Colors.Green; };
            contextMenu.Items.Add(menuItem);

            var connection = figure as Connection;
            if (connection != null)
            {
                CreateContextMenu(connection, contextMenu);
            }

            //var dropShadowEffect = new DropShadowEffect();
            //dropShadowEffect.BlurRadius = 8;
            //dropShadowEffect.ShadowDepth = 5;
            //dropShadowEffect.Direction = 315;
            //dropShadowEffect.Opacity = 0.8;
            //contextMenu.Effect = dropShadowEffect;
            //contextMenu.HasDropShadow = true;

            ContextMenu = contextMenu;

            ContextMenu.IsOpen = true;
        }

        private static void CreateContextMenu(Connection connection, ContextMenu contextMenu)
        {
            MenuItem menuItem;
            contextMenu.Items.Add(new Separator());

            menuItem = new MenuItem
            {
                Header = "to Orthogonal"
            };
            menuItem.Click += (sender, args) =>
            {
                var router = new OrthogonalConnectionRouter((f) => { });
                router.Reroute(connection);
            };
            contextMenu.Items.Add(menuItem);

            //
            menuItem = new MenuItem
            {
                Header = "to Direct"
            };
            menuItem.Click += (sender, args) =>
            {
                var router = new LineTool();
                router.Reroute(connection.Points);
            };
            contextMenu.Items.Add(menuItem);

            //
            var radius = 5;
            menuItem = new MenuItem
            {
                Header = $"CornerRadius = {radius}"
            };
            menuItem.Click += (sender, args) =>
            {
                connection.CornerRadius = 5;
            };
            contextMenu.Items.Add(menuItem);

            //
            radius = 25;
            menuItem = new MenuItem
            {
                Header = $"CornerRadius = {radius}"
            };
            menuItem.Click += (sender, args) =>
            {
                connection.CornerRadius = 25;
            };
            contextMenu.Items.Add(menuItem);

            radius = 0;
            menuItem = new MenuItem
            {
                Header = $"CornerRadius = {radius}"
            };
            menuItem.Click += (sender, args) =>
            {
                connection.CornerRadius = 0;
            };
            contextMenu.Items.Add(menuItem);
        }

        private void CanvasOnSceneChanged(object sender, EventArgs eventArgs)
        {
            //Debug.WriteLine("CanvasOnSceneChanged");
            Dispatcher.Invoke(InvalidateVisual);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            this.CaptureMouse();
            var point = e.GetPosition(this);

            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount > 1)
                {
                    Canvas?.OnMouseLeftDoubleClick(point.X, point.Y, IsShifKeyDown(), IsControlKeyDown());
                }
                else
                {
                    Canvas?.OnMouseLeftDown(point.X, point.Y, IsShifKeyDown(), IsControlKeyDown());
                }

            }
            if (e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Pressed)
            {
                Canvas?.OnMouseRightDown(point.X, point.Y, IsShifKeyDown(), IsControlKeyDown());
            }
            e.Handled = true;
        }


        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            var point = e.GetPosition(this);
            
            if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
            {
                Canvas?.OnMouseLeftUp(point.X, point.Y, IsShifKeyDown(), IsControlKeyDown());
            }
            else if (e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Released)
            {
                Canvas?.OnMouseRightUp(point.X, point.Y, IsShifKeyDown(), IsControlKeyDown());
            }
            e.Handled = true;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            var screenPoint = e.GetPosition(this); 
            //Check if we can be informed from the canvas about world mouse pos.
            var point = Canvas.CoordinateSystem.ToWorldSpace(screenPoint.X,screenPoint.Y);

            WorldMousePosX = point.X;
            WorldMousePosY = point.Y;

            Canvas?.OnMouseMove(screenPoint.X, screenPoint.Y, IsShifKeyDown(), IsControlKeyDown());

            e.Handled = true;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            Keyboard.Focus(this); //WPF keyboard bullshit.
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Keyboard.ClearFocus();//WPF keyboard bullshit.
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            //Canvas?.OnKeyDown(e.Key);
            if (!e.IsRepeat)
            {
                Canvas?.OnKeyDown(e.Key);
            }

        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            //Canvas?.OnKeyDown(e.Key);
            if (!e.IsRepeat)
            {
                Canvas?.OnKeyUp(e.Key);
            }

        }

        private static bool IsControlKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl);
        }

        private static bool IsShifKeyDown()
        {
            return Keyboard.IsKeyDown(Key.LeftShift) | Keyboard.IsKeyDown(Key.RightShift);
        }
    }


}
