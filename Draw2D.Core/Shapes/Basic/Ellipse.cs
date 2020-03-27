using System;
using System.Windows.Media;
using Draw2D.Core.Geo;
using Draw2D.Core.Utlils;

namespace Draw2D.Core.Shapes.Basic
{
    public class Ellipse : Rectangle
    {
        public Ellipse(float x, float y, float width, float height) : base(x - width / 2.0f, y - height / 2.0f, width, height)
        {
            SetSnapTargets(SnapTargets.MidPoints | SnapTargets.Center);
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

            dc.DrawEllipse(fillBrush, pen, BoundingBox.Center, Width / 2, Height / 2);

            dc.Pop();
        }
    }
}