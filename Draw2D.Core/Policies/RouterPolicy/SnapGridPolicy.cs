using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Draw2D.Core.Layout.Connection;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Utlils.Linq;
using Point = Draw2D.Core.Geo.Point;

namespace Draw2D.Core.Policies.RouterPolicy
{

    public interface ISnapPolicy
    {
        void InitSnap(Canvas canvas, IEnumerable<Point> snapPointsOfFigure, IEnumerable<Figure> blackList);
        bool Snap(Canvas canvas, Point figurePos, float dx, float dy, float dxSum, float dySum, out Point snapPosition, out Point delta, IEnumerable<Figure> blackList);
        void EndSnapping(Canvas canvas);

        int Priority { get; set; } //Higher value means snapping-policy will be executed first.
    }

    public class SnapGridPolicy : PolicyBase, ISnapPolicy
    {
        private List<Point> _startSnapPoints = new List<Point>();
        private List<Point> _ghostSnapPoints = new List<Point>();
        private Cross _snapPointCross;

        public void InitSnap(Canvas canvas, IEnumerable<Point> snapPointsOfFigure, IEnumerable<Figure> blackList)
        {
            _startSnapPoints = new List<Point>();
            _ghostSnapPoints = new List<Point>();

            foreach (var point in snapPointsOfFigure)
            {
                //We need the the fixed snap-points, 
                //because we are oprating with dxSum and dySum, 
                //which make only sense with the original starting positions.
                _startSnapPoints.Add(point.Clone());

                //We move ghost snap points around to the new position.
                _ghostSnapPoints.Add(point.Clone());
            }

            _snapPointCross = new Cross(0, 0, 10, 10)
            {
                IsSelectable = false,
                IsDragable = false,
                IsVisible = false,
                CanBeSnapTarget = false,
                StrokeColor = Colors.Red,
            };

            canvas.AddFigure(_snapPointCross);
        }

        /// <summary>
        /// Snap to grid points of the canvas grid.
        /// </summary>
        /// <returns>True, if there was a snap. Grid snap always finds a snap.</returns>
        /// <remarks>Do not rely on the delta and snapPoint results if there was no snap.</remarks>
        public bool Snap(Canvas canvas, Point figurePos, float dx, float dy, float dxSum, float dySum, out Point snapPosition, out Point delta, IEnumerable<Figure> blackList)
        {
       
            delta = new Point(dx, dy);
            snapPosition = figurePos + new Point(dx, dy);
            
            if (_startSnapPoints.Count == 0)
            {
                return false;
            }

            var isFirst = true;
            var dist = new Point(dxSum, dySum);

            for (int i = 0; i < _startSnapPoints.Count(); i++)
            {
                //Console.WriteLine("SnapPoint:" + _startSnapPoints[i]);

                var newPos = _startSnapPoints[i] + dist;
                var nearestGridPoint = canvas.Grid.GetNearestPoint(newPos);

                //Check for new minimum delta.
                if (isFirst)
                {
                    isFirst = false;
                    delta = nearestGridPoint - _ghostSnapPoints[i];
                    snapPosition = nearestGridPoint;
                }
                else if ((nearestGridPoint - _ghostSnapPoints[i]).SquareLength < delta.SquareLength)
                {
                    delta = nearestGridPoint - _ghostSnapPoints[i];
                    snapPosition = nearestGridPoint;
                }
            }

            _snapPointCross.ForceSetPositionOfCenter(snapPosition.X, snapPosition.Y);
            _snapPointCross.IsVisible = true;
            _snapPointCross.BringToFront();

            //Update the ghost snap points.
            foreach (var ghostSnapPoint in _ghostSnapPoints)
            {
                ghostSnapPoint.Translate(delta.X, delta.Y);
            }

            return true;
        }

        public void EndSnapping(Canvas canvas)
        {
            canvas.RemoveFigure(_snapPointCross);
        }

        public int Priority { get; set; } = 0;
    }
}