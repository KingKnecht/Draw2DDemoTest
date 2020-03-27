using System;
using System.Windows;
using System.Windows.Controls;

namespace Draw2DControlLibrary
{
    public static class Helpers
    {
        public static Point Clamp(Point value)
        {
            return new Point(Math.Max(value.X, 0), Math.Max(value.Y, 0));
        }

        public static Point Clamp(Point topLeft, Point bottomRight, Point value)
        {
            return new Point(
                Math.Max(Math.Min(value.X, bottomRight.X), topLeft.X),
                Math.Max(Math.Min(value.Y, bottomRight.Y), topLeft.Y));
        }

        public static Rect Clamp(Point topLeft, Point bottomRight, Point value1, Point value2)
        {
            var point1 = Clamp(topLeft, bottomRight, value1);
            var point2 = Clamp(topLeft, bottomRight, value2);
            var newTopLeft = new Point(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            var size = new Size(Math.Abs(point1.X - point2.X), Math.Abs(point1.Y - point2.Y));
            return new Rect(newTopLeft, size);
        }

        public static void PositionBorderOnCanvas(Border border, Rect rect)
        {
            Canvas.SetLeft(border, rect.Left);
            Canvas.SetTop(border, rect.Top);
            border.Width = rect.Width;
            border.Height = rect.Height;
        }

        public static void PositionBorderOnCanvas(Border border, Point topLeft)
        {
            Canvas.SetLeft(border, topLeft.X);
            Canvas.SetTop(border, topLeft.Y);
        }

        public static void DragBorderOnCanvas(Border border, Point topLeft)
        {
            Canvas.SetLeft(border, Canvas.GetLeft(border) + topLeft.X);
            Canvas.SetTop(border, Canvas.GetTop(border) + topLeft.Y);
        }
    }
}
