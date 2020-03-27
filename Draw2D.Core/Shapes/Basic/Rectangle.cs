using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Draw2D.Core.Layout;
using Draw2D.Core.Utlils;


namespace Draw2D.Core.Shapes.Basic
{
    public class Rectangle : VectorFigure
    {
        private VectorFigure _centerHandle;

        public Rectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            SnapTargets = SnapTargets.Center | SnapTargets.MidPoints | SnapTargets.Vertices;
        }

        public Rectangle(Geo.Rectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
        {

        }

        public override bool OnDragStart(Canvas canvas, float x, float y)
        {
            base.OnDragStart(canvas, x, y);

            if (_centerHandle == null)
            {
                _centerHandle = new Cross(BoundingBox.Center.X, BoundingBox.Center.Y, 10, 10)
                {
                    IsSelectable = false,
                    IsDragable = false,
                    FillColor = Colors.Transparent,
                    CanBeSnapTarget = false

                };
                canvas.AddFigure(_centerHandle);
            }
            _centerHandle.BringToFront();


            return true;
        }

        public override void OnDrag(Canvas canvas, float dxSum, float dySum, float dx, float dy, bool isShiftKey, bool isCtrlKey)
        {
            base.OnDrag(canvas, dxSum, dySum, dx, dy, isShiftKey, isCtrlKey);

            _centerHandle?.ForceSetPositionOfCenter(BoundingBox.Center);
        }

        public override void OnDragEnd(Canvas canvas, bool isShiftKey, bool isCtrlKey)
        {
            base.OnDragEnd(canvas, isShiftKey, isCtrlKey);
            canvas.RemoveFigure(_centerHandle);
            _centerHandle = null;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, w: {Width}, h: {Height}";
        }

        public override void Render(DrawingContext dc)
        {
            var screenPoint = Canvas.CoordinateSystem.ToScreenSpace(Position);
            var offset = new Point((float)screenPoint[0] - X, (float)screenPoint[1] - Y - Height);

            var strokeBrush = new SolidColorBrush(StrokeColor);
            strokeBrush.Freeze();

            var pen = new Pen(strokeBrush, StrokeThickness)
            {
                DashStyle = DashStyle
            };
            pen.Freeze();

            var fillBrush = new SolidColorBrush(FillColor);
            fillBrush.Freeze();

            dc.PushTransform(new TranslateTransform(offset.X, offset.Y));

            dc.DrawRectangle(fillBrush, pen,
                new Rect(new Point(X, Y), new Size(Width, Height)));

            dc.Pop();
        }
    }
}
