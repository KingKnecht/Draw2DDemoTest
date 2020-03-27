using System.Windows.Media;

namespace Draw2D.Core.Shapes.Basic
{
    public class Cross : Rectangle
    {
        public Cross(float x, float y, float width, float height) : base(x, y, width, height)
        {
        }

        public Cross(Geo.Rectangle rect) : base(rect)
        {
        }

        public override void Render(DrawingContext dc)
        {
            var strokeBrush = new SolidColorBrush(StrokeColor);
            strokeBrush.Freeze();

            var pen = new Pen(strokeBrush, StrokeThickness)
            {
                DashStyle = DashStyle
            };
            pen.Freeze();

            var fillBrush = new SolidColorBrush(FillColor);
            fillBrush.Freeze();
            var leftCenter = Canvas.CoordinateSystem.ToScreenSpace(BoundingBox.LeftCenter);
            var rightCenter = Canvas.CoordinateSystem.ToScreenSpace(BoundingBox.RightCenter);
            var bottomCenter = Canvas.CoordinateSystem.ToScreenSpace(BoundingBox.BottomCenter);
            var topCenter = Canvas.CoordinateSystem.ToScreenSpace(BoundingBox.TopCenter);

            dc.DrawLine(pen,new System.Windows.Point(leftCenter[0],leftCenter[1]) ,new  System.Windows.Point(rightCenter[0], rightCenter[1]));
            dc.DrawLine(pen, new System.Windows.Point(bottomCenter[0], bottomCenter[1]), new System.Windows.Point(topCenter[0], topCenter[1]));
        }
    }
}