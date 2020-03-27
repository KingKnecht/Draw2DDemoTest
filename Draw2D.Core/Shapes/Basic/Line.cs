using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Draw2D.Core.Geo;
using Draw2D.Core.Handles;
using Draw2D.Core.Policies;
using Draw2D.Core.Policies.FigurePolicy;

namespace Draw2D.Core.Shapes.Basic
{
    public class Line : VectorFigure
    {
        private readonly List<Point> _points = new List<Point>() { new Point(0, 0), new Point(0, 0) };

        private Color _storedStrokeColor;
       
        public Line(Point startPoint, Point endPoint)
        {
            SnapTargets = SnapTargets.Vertices;
            FillColor = Colors.Transparent;
            StrokeThickness = 1;
            StartPoint = startPoint;
            EndPoint = endPoint;
            _storedStrokeColor = StrokeColor;

        }
        
        public void ResetPoints()
        {
            var startPoint = StartPoint;
            var endPoint = EndPoint;

            _points.Clear();
            this[0] = startPoint;
            this[1] = endPoint;
        } 

        public Line(float x1, float y1, float x2, float y2) : this(new Point(x1, y1), new Point(x2, y2))
        {

        }

        public double CoronaWidth { get; set; } = 5;

        public void InstallLineHandle(int pointIndex)
        {
            var handle = new LineHandle(new Ellipse(this[pointIndex].X, this[pointIndex].Y, 10, 10), pointIndex, this);
            handle.SetSnapTargets(SnapTargets.Center);
 
            AddHandle(handle);
        }

        public override IEnumerable<Point> GetSnapPoints()
        {
            if (SnapTargets.HasFlag(SnapTargets.Vertices))
            {
                yield return StartPoint;
                yield return EndPoint;
            }
        }

        public Geo.Point StartPoint
        {
            get { return _points[0]; }
            set
            {
                if (value == null)
                    return;

                this[0] = value.Clone();
                
                Canvas?.NeedsRepaint(this);
            }
        }



        public Geo.Point EndPoint
        {
            get { return _points.Last(); }
            set
            {
                if (value == null)
                    return;

                this[_points.Count - 1] = value.Clone();

                
                Canvas?.NeedsRepaint(this);
            }
        }

        /// <summary>
        /// Gets/Sets a point at index. If set-index is equal to size of collection the point is added.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Point this[int index]
        {
            get { return _points[index]; }
            set
            {
                if (value == null)
                    return;

                if (index > _points.Count - 1)
                {
                    _points.Add(value.Clone());
                }
                else
                {
                    _points[index] = value.Clone();
                }

                var bb = CalculateBoundingBox();

                Y = bb.Y;
                X = bb.X;
                Width = bb.Width;
                Height = bb.Height;

                BoundingBox = bb;

                Canvas?.OnFigureTranslated(this);

                
                Canvas?.NeedsRepaint(this);
            }
        }

        private Geo.Rectangle CalculateBoundingBox()
        {
            var bb = Geo.Rectangle.GetBoundingBoxAroundPoints(_points);

            bb.Y -= StrokeThickness/2;
            bb.X -= StrokeThickness/2;

            bb.Height += StrokeThickness;
            bb.Width += StrokeThickness;

            return bb;
        }

        public int PointCount => _points.Count;

        public void RemoveAt(int index)
        {
            _points.RemoveAt(index);

            
            Canvas?.NeedsRepaint(this);
        }

        public IReadOnlyList<Point> Points => _points;

        
        public override bool HitTest(float x, float y)
        {
            return Hit(CoronaWidth + StrokeThickness, StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y, x, y);
        }

        protected bool Hit(double coronaWidth, double x1, double y1, double x2, double y2, double x, double y)
        {
            return LineFunctions.Distance(x1, y1, x2, y2, x, y) < coronaWidth;
        }
         
        public override Figure Unselect()
        {
            base.Unselect();
            foreach (var lineHandle in Handles)
            {
                lineHandle.Hide(Canvas);
            }

            return this;
        }

        public override Figure EnableSelectionFeedback(bool isFeedbackEnabled)
        {
            if (isFeedbackEnabled)
            {
                _storedStrokeColor = StrokeColor;
                StrokeColor = GlowColor;
            }
            else
            {
                StrokeColor = _storedStrokeColor;
            }
            
            Canvas?.NeedsRepaint(this);

            return this;
        }

        public override void Translate(float dx, float dy)
        {
            if (!IsDragable)
                return;

            AdjustPositionResult adjustmentResult = new AdjustPositionResult(dx, dy);
            foreach (var policy in Policies)
            {
                if (policy is DragDropEditPolicy)
                {
                    var dragDropPolicy = (DragDropEditPolicy)policy;

                    adjustmentResult = dragDropPolicy.AdjustPositionByDelta(this, dx, dy, adjustmentResult);
                }
            }

            if (adjustmentResult.Dx != 0 || adjustmentResult.Dy != 0)
            {
                var offset = new Point(adjustmentResult.Dx, adjustmentResult.Dy);
                ForceTranslate(offset);
            }
        }
        public void ForceTranslate(Point offset)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                this[i] += offset;
            }

            foreach (var handle in Handles)
            {
                handle.Update();
            }

            BoundingBox = CalculateBoundingBox();
            Canvas?.OnFigureTranslated(this);

            
            Canvas?.NeedsRepaint(this);
        }

        public Color GlowColor { get; set; } = Colors.SlateGray;

        public void Translate(float dx, float dy, int pointIndex)
        {
            this[pointIndex].Translate(dx, dy);

            BoundingBox = CalculateBoundingBox();
            Canvas?.OnFigureTranslated(this);

            
            Canvas?.NeedsRepaint(this);
        }

        public override void Render(DrawingContext dc)
        {
          
            for (int i = 0; i < PointCount - 1; i++)
            {
                var p1 = Canvas.CoordinateSystem.ToScreenSpace(this[i]);
                var p2 = Canvas.CoordinateSystem.ToScreenSpace(this[i + 1]);

                var brush = new SolidColorBrush(StrokeColor);
                brush.Freeze();

                var pen = new Pen(brush, StrokeThickness)
                {
                    DashStyle = DashStyle
                };

                pen.Freeze();
                dc.DrawLine(pen, new System.Windows.Point(p1[0],p1[1]), new System.Windows.Point(p2[0], p2[1]));
            }
        }

        public override string ToString()
        {
            return $"[{StartPoint}]->[{EndPoint}]";
        }
    }

    public interface IEditTool
    {

    }
}