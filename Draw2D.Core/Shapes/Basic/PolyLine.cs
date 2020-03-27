using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Draw2D.Core.Geo;
using Draw2D.Core.Policies.RouterPolicy;
using Draw2D.Core.Utlils;
using QuickGraph;
using QuickGraph.Algorithms;

namespace Draw2D.Core.Shapes.Basic
{
    public class PolyLine : Line
    {
        public PolyLine(Point startPoint, Point endPoint) : base(startPoint, endPoint)
        {
            FillColor = Colors.Transparent;
        }

        public PolyLine(float x1, float y1, float x2, float y2) : base(new Point(x1,x2),new Point(x2,y2) )
        {
        }

        public virtual PolyLine AddToEnd(Point point)
        {
            this[PointCount] = point;

            return this;
        }

        public void AddToEnd(float x, float y)
        {
            AddToEnd(new Point(x, y));
        }

       
        public override IEnumerable<Point> GetSnapPoints()
        {
            if (SnapTargets.HasFlag(SnapTargets.Vertices))
            {
                foreach (var point in Points)
                {
                    yield return point;
                }
            }
        }
    

        public override bool HitTest(float x, float y)
        {
            for (int i = 0; i < PointCount - 1; i++)
            {
                var startOfLineSegment = this[i];
                var endOfLineSegment = this[i + 1];

                if (Hit(CoronaWidth + StrokeThickness, startOfLineSegment.X,
                    startOfLineSegment.Y, endOfLineSegment.X, endOfLineSegment.Y, x, y))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetPoints(List<Point> points)
        {
            ResetPoints();
            for (int i = 0; i < points.Count; i++)
            {
                this[i] = points[i];
            }

            
            Canvas?.NeedsRepaint(this);
        }

        public override void Render(DrawingContext dc)
        {
            var strokeBrush = new SolidColorBrush(StrokeColor);
            strokeBrush.Freeze();

            var pen = new Pen(strokeBrush, StrokeThickness);
            pen.Freeze();

            var geom = new StreamGeometry();
            using (StreamGeometryContext ctx = geom.Open())
            {
                var startVertex = Canvas.CoordinateSystem.ToScreenSpace(StartPoint);
                ctx.BeginFigure(new System.Windows.Point(startVertex[0], startVertex[1]), false, false);
                ctx.PolyLineTo(Points.Skip(1).Select(p =>
                        {
                            var vertex = Canvas.CoordinateSystem.ToScreenSpace(p);
                            return new System.Windows.Point(vertex[0], vertex[1]);
                        }).ToList(),

                    true/* is stroked */, true /* is smooth join */);
            }
            geom.Freeze();

            dc.DrawGeometry(null, pen, geom);
        }
    }
}