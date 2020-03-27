using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Draw2D.Core;

namespace Draw2DControlLibrary
{
   
    public partial class Draw2DControl : System.Windows.Controls.Canvas 
    {
        private ICanvas _canvas;
        private Pen _gridPen;

        static Draw2DControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Draw2DControl),
                new FrameworkPropertyMetadata(typeof(Draw2DControl), FrameworkPropertyMetadataOptions.AffectsRender));
        }

        public Draw2DControl()
        {
            _gridPen = new Pen(new SolidColorBrush(Colors.Gray), 1);
            _gridPen.Freeze();
            //Background = Brushes.Transparent;
            //RenderOptions.SetEdgeMode(this,EdgeMode.Aliased);
        }

        

        public ICanvas Canvas
        {
            get { return (ICanvas)GetValue(CanvasProperty); }
            set { SetValue(CanvasProperty, value); }
        }

        protected override void OnRender(DrawingContext dc)
        {
           
            if (Canvas == null)
                return;

            DrawBackground(dc);
            DrawGrid(dc);

            // var vectorFigures = Canvas.Figures.OfType<VectorFigure>().Where(f => f.IsVisible).ToList();
            List<VectorFigure> vectorFigures = Canvas.GetRenderableFigures();

            RenderedItemsCount = vectorFigures.Count;

            foreach (var figure in vectorFigures)
            {
                var vectorFigure = figure;
                //var handle = figure as IHandle;
                //if (handle != null)
                //{
                //    vectorFigure = handle.HandleShape;
                //}

                vectorFigure.Render(dc);
            }

           
        }

        private void DrawBackground(DrawingContext dc)
        {
            Brush background = Background;
            if (background != null)
            {
                // Using the Background brush, draw a rectangle that fills the
                // render bounds of the panel.
                Size renderSize = RenderSize;
                dc.DrawRectangle(background,
                                 null,
                                 new Rect(0.0, 0.0, renderSize.Width, renderSize.Height));
            }
        }

        private void DrawGrid(DrawingContext dc)
        {
            var xGridSize = Canvas.Grid.UnitX;
            var xAmount = (int)Canvas.Width / xGridSize;

            var yGridSize = Canvas.Grid.UnitY;
            var yAmount = (int)Canvas.Height / yGridSize;

            for (int i = 0; i <= xAmount; i++)
            {
                dc.DrawLine(_gridPen, new Point(i * xGridSize, 0), new Point(i * xGridSize, Canvas.Height));
            }

            for (int i = 0; i <= yAmount; i++)
            {
                dc.DrawLine(_gridPen, new Point(0, i * yGridSize), new Point(Canvas.Width, i * yGridSize));
            }

        }

        
        //private Draw2D.Core.Geo.Point ToWorldSpace(Point screenPoint)
        //{
        //    var factor = 1; //internally everthing is stored in 1/100mm.
        //    return new Draw2D.Core.Geo.Point((int) (screenPoint.X - Canvas.OriginX*Canvas.Width*factor),
        //        (int) (Canvas.Height*factor - screenPoint.Y - Canvas.OriginY *Canvas.Height*factor));
        //}

        //private Point ToScreenSpace(Draw2D.Core.Geo.Point point)
        //{
        //    var factor = 1; //internally everthing is stored in 1/100mm.
        //    return new Point(point.X + Canvas.OriginX * Canvas.Width / factor,
        //        -point.Y + Canvas.Height / factor - Canvas.OriginY * Canvas.Height / factor);
        //}
    }
}
