using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Draw2D.Core.Geo;

namespace Draw2D.Core.Shapes.Basic
{
    public class Path : VectorFigure
    {
        private Geometry _geometry;

        public void SetGeometry(Geometry geometry)
        {
            _geometry = geometry;
            X = (int)Math.Ceiling(_geometry.Bounds.Left);
            Y = (int)Math.Ceiling(_geometry.Bounds.Bottom);
            Width = (int)Math.Ceiling(_geometry.Bounds.Width);
            Height = (int)Math.Ceiling(_geometry.Bounds.Height);
        }
        
        public override void Render(DrawingContext dc)
        {
            var screenPoint = Canvas.CoordinateSystem.ToScreenSpace(Position);
            var offset = new Point((int)screenPoint[0] - X, (int)screenPoint[1] - Y);

            //StreamGeometry streamGeometry = new StreamGeometry();
            //using (StreamGeometryContext ctx = streamGeometry.Open())
            //{
            //    //_geometry.Transform = new TranslateTransform(offset.X, offset.Y);
            //    ctx.DrawGeometry(_geometry);
            //}

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
            //streamGeometry.Transform = new TranslateTransform(offset.X,offset.Y);
            dc.DrawGeometry(fillBrush, pen, _geometry);
            dc.Pop();
        }

    }

    public static class GeometryExtensions
    {
        public static void DrawGeometry(this StreamGeometryContext ctx, Geometry geo)
        {
            var pathGeometry = geo as PathGeometry ?? PathGeometry.CreateFromGeometry(geo);
            foreach (var figure in pathGeometry.Figures)
                ctx.DrawFigure(figure);
        }

        public static void DrawFigure(this StreamGeometryContext ctx, PathFigure figure)
        {
            ctx.BeginFigure(figure.StartPoint, figure.IsFilled, figure.IsClosed);
            foreach (var segment in figure.Segments)
            {
                var lineSegment = segment as LineSegment;
                if (lineSegment != null) { ctx.LineTo(lineSegment.Point, lineSegment.IsStroked, lineSegment.IsSmoothJoin); continue; }

                var bezierSegment = segment as BezierSegment;
                if (bezierSegment != null) { ctx.BezierTo(bezierSegment.Point1, bezierSegment.Point2, bezierSegment.Point3, bezierSegment.IsStroked, bezierSegment.IsSmoothJoin); continue; }

                var quadraticSegment = segment as QuadraticBezierSegment;
                if (quadraticSegment != null) { ctx.QuadraticBezierTo(quadraticSegment.Point1, quadraticSegment.Point2, quadraticSegment.IsStroked, quadraticSegment.IsSmoothJoin); continue; }

                var polyLineSegment = segment as PolyLineSegment;
                if (polyLineSegment != null) { ctx.PolyLineTo(polyLineSegment.Points, polyLineSegment.IsStroked, polyLineSegment.IsSmoothJoin); continue; }

                var polyBezierSegment = segment as PolyBezierSegment;
                if (polyBezierSegment != null) { ctx.PolyBezierTo(polyBezierSegment.Points, polyBezierSegment.IsStroked, polyBezierSegment.IsSmoothJoin); continue; }

                var polyQuadraticSegment = segment as PolyQuadraticBezierSegment;
                if (polyQuadraticSegment != null) { ctx.PolyQuadraticBezierTo(polyQuadraticSegment.Points, polyQuadraticSegment.IsStroked, polyQuadraticSegment.IsSmoothJoin); continue; }

                var arcSegment = segment as ArcSegment;
                if (arcSegment != null) { ctx.ArcTo(arcSegment.Point, arcSegment.Size, arcSegment.RotationAngle, arcSegment.IsLargeArc, arcSegment.SweepDirection, arcSegment.IsStroked, arcSegment.IsSmoothJoin); continue; }
            }
        }
    }
}
