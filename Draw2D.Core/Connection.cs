using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Draw2D.Core.Layout.Connection;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Utlils;
using Point = Draw2D.Core.Geo.Point;

namespace Draw2D.Core
{
    public class Connection : PolyLine
    {
        private ConnectionRouter _router;
        private int _cornerRadius;

        public Connection(ConnectionRouter router, Point startPoint, Point endPoint) 
            : base(startPoint, endPoint)
        {
            Router = router;
            router.Reroute(this);
        }

        public Connection(ConnectionRouter router, int x1, int y1, int x2, int y2)
            : this(router,new Point(x1, y1),  new Point( x2, y2))
        {}

        public ConnectionRouter Router
        {
            get { return _router; }
            set
            {
                _router = value;
                _router.Reroute(this);
            }
        }
        
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                _cornerRadius = value;
                Canvas.NeedsRepaint(this);
            }
        }

        public override void Render(DrawingContext dc)
        {
            //var strokeBrush = new SolidColorBrush(StrokeColor);
            //strokeBrush.Freeze();

            //var pen = new Pen(strokeBrush, StrokeThickness);
            //pen.Freeze();

            //if (CornerRadius > 0)
            //{
            //    //taken from: http://stackoverflow.com/questions/3759880/customize-rounded-corner-radius-of-a-wpf-polyline
            //    PathFigure figure = new PathFigure();
            //    if (Points.Count > 0)
            //    {
            //        var vetices = Points.Select(Canvas.CoordinateSystem.ToScreenSpace).ToList();
            //        figure.StartPoint = new System.Windows.Point(vetices[0][0], vetices[0][1]);

            //        if (vetices.Count > 1)
            //        {
            //            // points between

            //            for (int i = 1; i < (vetices.Count - 1); i++)
            //            {
            //                // adjust radius if points are too close
            //                Vector v1 = vetices[i] - vetices[i - 1];
            //                Vector v2 = vetices[i + 1] - vetices[i];
            //                var radius = Math.Min(Math.Min(v1.Length, v2.Length) / 2, CornerRadius);

            //                // draw the line, and stop before the next point
            //                var len = v1.Length;
            //                v1.Normalize();
            //                v1 *= (len - radius);
            //                LineSegment line = new LineSegment(vetices[i - 1] + v1, true);
            //                figure.Segments.Add(line);

            //                // draw the arc to the next point
            //                v2.Normalize();
            //                v2 *= radius;
            //                SweepDirection direction = (Vector.AngleBetween(v1, v2) > 0) ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
            //                ArcSegment arc = new ArcSegment(vetices[i] + v2, new Size(radius, radius), 0, false, direction, true);
            //                figure.Segments.Add(arc);
            //            }

            //            // last point
            //            figure.Segments.Add(new LineSegment(vetices[vetices.Count - 1], true));
            //        }

            //        PathGeometry geometry = new PathGeometry();
            //        geometry.Figures.Add(figure);
            //        geometry.FillRule = FillRule.EvenOdd;

            //        geometry.Freeze();
            //        dc.DrawGeometry(null, pen, geometry);
            //    }
            //}
            //else
            //{
            //    var geom = new StreamGeometry();

            //    using (StreamGeometryContext ctx = geom.Open())
            //    {

            //        ctx.BeginFigure(toScreenSpace(StartPoint), false, false);

            //        ctx.PolyLineTo(Points.Skip(1).Select(toScreenSpace).ToList(),
            //            true/* is stroked */, true /* is smooth join */);

            //    }

            //    geom.Freeze();

            //    dc.DrawGeometry(null, pen, geom);
            //}


        }

        //private List<Point> CalculateBezierPoints(IReadOnlyList<Point> points)
        //{
        //    for (int i = 0; i < points.Count -1; i++)
        //    {
        //        var p1 = points[i + 1] - points[i] / 2;
        //        var p2 = 
        //    }
        //}
    }
}
