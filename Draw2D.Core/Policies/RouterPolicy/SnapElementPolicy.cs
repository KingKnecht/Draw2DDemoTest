using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Utlils;
using Draw2D.Core.Utlils.Linq;
using QuickGraph.Serialization;
using Supercluster.KDTree;
using Point = Draw2D.Core.Geo.Point;

namespace Draw2D.Core.Policies.RouterPolicy
{
    public class SnapElementPolicy : PolicyBase, ISnapPolicy
    {
        private readonly List<Line> _guideLines = new List<Line>();
        private List<Point> _startSnapPoints = new List<Point>();
        private List<Point> _ghostSnapPoints = new List<Point>();
        private IEnumerable<Figure> _blacklist;
        
        public void InitSnap(Canvas canvas, IEnumerable<Point> snapPointsOfFigure, IEnumerable<Figure> blackList)
        {
            _blacklist = blackList;
            _startSnapPoints = new List<Point>();
            _ghostSnapPoints = new List<Point>();

            foreach (var point in snapPointsOfFigure)
            {
                //We need the the fixed snap-points, 
                //because we are operating with dxSum and dySum, 
                //which make only sense with the original starting positions.
                _startSnapPoints.Add(point.Clone());

                //We move ghost snap points around to the new position.
                _ghostSnapPoints.Add(point.Clone());
            }


        }

        private Line CreateGuideLine()
        {
            return new Line(0, 0, 0, 0)
            {
                CanBeSnapTarget = false,
                StrokeColor = Colors.DarkMagenta,
                StrokeThickness = 2,
                DashStyle = DashStyles.Dash
            };
        }

        /// <summary>
        /// Snaps to Element snap points.
        /// </summary>
        /// <returns>True, if there was a snap.</returns>
        /// <remarks>Do not rely on the delta and snapPoint results if there was no snap.</remarks>
        public bool Snap(Canvas canvas, Point figurePos, float dx, float dy, float dxSum, float dySum, out Point snapPosition, out Point delta, IEnumerable<Figure> blackList)
        {
            DeleteGuideLines(canvas);
            snapPosition = new Point(dxSum, dySum);

            if (_startSnapPoints.Count == 0)
            {
                delta = new Point(dx, dy);
                return false;
            }

            var sumDelta = new Point(dxSum, dySum);
            delta = new Point(dx, dy);

            var targetInfos = new List<TargetInfo>();

            for (int i = 0; i < _startSnapPoints.Count; i++)
            {
                var newPosition = _startSnapPoints[i] + sumDelta;

                var nearestXNeighbour = canvas.SnapCluster.GetXNodes(newPosition.X, _blacklist)
                    .OrderBy(n => Math.Abs(newPosition.Y - n.Point.Y))
                    .FirstOrDefault();

                if (nearestXNeighbour != null)
                {
                    targetInfos.Add(new TargetInfo()
                    {
                        SnapPoint = new Point(nearestXNeighbour.Position, _ghostSnapPoints[i].Y),
                        SourcePoint = _ghostSnapPoints[i].Clone(),
                        TargetVertex = nearestXNeighbour.Point.Clone()
                    });
                }

                var nearestYNeighbour = canvas.SnapCluster.GetYNodes(newPosition.Y, _blacklist)
               .OrderBy(n => Math.Abs(newPosition.X - n.Point.X))
               .FirstOrDefault();

                if (nearestYNeighbour != null)
                {
                    targetInfos.Add(new TargetInfo()
                    {
                        SnapPoint = new Point(_ghostSnapPoints[i].X, nearestYNeighbour.Position),
                        SourcePoint = _ghostSnapPoints[i].Clone(),
                        TargetVertex = nearestYNeighbour.Point.Clone()
                    });
                }
            }

            if (targetInfos.Count == 0)
            {

                foreach (var ghostSnapPoint in _ghostSnapPoints)
                {
                    ghostSnapPoint.Translate(delta.X, delta.Y);
                }

                return false;
            }


            var orderedTargetsX =
                targetInfos.Where(ti => ti.IsInXDirection)
                    .OrderBy(ti => Math.Abs(ti.SourcePoint.X - ti.SnapPoint.X))
                    .ThenBy(ti => Math.Abs(ti.SourcePoint.X - ti.TargetVertex.X))
                    .ToList();

            var orderedTargetsY =
                targetInfos.Where(ti => !ti.IsInXDirection)
                   .OrderBy(ti => Math.Abs(ti.SourcePoint.Y - ti.SnapPoint.Y))
                   .ThenBy(ti => Math.Abs(ti.SourcePoint.Y - ti.TargetVertex.Y))
                   .ToList();

            TargetInfo minTargetX = null;
            if (orderedTargetsX.Any())
            {
                minTargetX = orderedTargetsX.First();
            }

            TargetInfo minTargetY = null;
            if (orderedTargetsY.Any())
            {
                minTargetY = orderedTargetsY.First();
                //Console.WriteLine(minY.SourcePoint);
            }


            if (minTargetX != null)
            {
                delta.Y = minTargetX.SnapPoint.Y - minTargetX.SourcePoint.Y;
                snapPosition.Y = minTargetX.SnapPoint.Y;
            }

            if (minTargetY != null)
            {
                delta.X = minTargetY.SnapPoint.X - minTargetY.SourcePoint.X;
                snapPosition.X = minTargetY.SnapPoint.X;
            }

            //Console.WriteLine("snapPosition: " + snapPosition);

            foreach (var ghostSnapPoint in _ghostSnapPoints)
            {
                ghostSnapPoint.Translate(delta.X, delta.Y);
            }

            if (minTargetX != null)
            {
                if (ShowAdditionalLines)
                {
                    ShowAdditionalLinesX(canvas, orderedTargetsX);
                }

                var guideLine = CreateGuideLine();
                //guideLine.StrokeColor = Colors.CornflowerBlue;
                guideLine.StartPoint = minTargetX.SnapPoint;
                guideLine.EndPoint = minTargetX.TargetVertex;
                guideLine.IsVisible = true;

                _guideLines.Add(guideLine);
                canvas.AddAdornerFigure(guideLine);
            }

            if (minTargetY != null)
            {
                if (ShowAdditionalLines)
                {
                    ShowAdditionalLinesY(canvas, orderedTargetsY);
                }

                var guideLine = CreateGuideLine();
                //guideLine.StrokeColor = Colors.CornflowerBlue;
                guideLine.StartPoint = minTargetY.SnapPoint;
                guideLine.EndPoint = minTargetY.TargetVertex;
                guideLine.IsVisible = true;

                //Console.WriteLine(guideLine);
                _guideLines.Add(guideLine);
                canvas.AddAdornerFigure(guideLine);

            }

            return true;
        }

        private void ShowAdditionalLinesX(Canvas canvas, List<TargetInfo> allXTargetInfos)
        {
            if (allXTargetInfos.Count > 1)
            {
                var additionalAlignments = allXTargetInfos
                    .GroupBy(ti => ti.TargetVertex)
                    .Select(g => g.MinBy(ti => Math.Abs(ti.TargetVertex.X - ti.SnapPoint.X)))
                    .Skip(1);

                foreach (var source in additionalAlignments)
                {
                    var guideLine = CreateGuideLine();
                    guideLine.StartPoint = source.SnapPoint;
                    guideLine.EndPoint = source.TargetVertex;
                    guideLine.IsVisible = true;

                    _guideLines.Add(guideLine);
                    canvas.AddAdornerFigure(guideLine);
                }
            }
        }

        private void ShowAdditionalLinesY(Canvas canvas, List<TargetInfo> allYTargetInfos)
        {
            if (allYTargetInfos.Count > 1)
            {
                var additionalAlignments = allYTargetInfos
                    .GroupBy(ti => ti.TargetVertex)
                    .Select(g => g.MinBy(ti => Math.Abs(ti.TargetVertex.Y - ti.SnapPoint.Y)))
                    .Skip(1);

                foreach (var source in additionalAlignments)
                {
                    var guideLine = CreateGuideLine();
                    guideLine.StartPoint = source.SnapPoint;
                    guideLine.EndPoint = source.TargetVertex;
                    guideLine.IsVisible = true;

                    _guideLines.Add(guideLine);
                    canvas.AddAdornerFigure(guideLine);
                }
            }
        }

        public bool ShowAdditionalLines { get; set; } = false;

        private void DeleteGuideLines(Canvas canvas)
        {
            foreach (var guideLine in _guideLines)
            {
                canvas.RemoveAdornerFigure(guideLine);
            }
            _guideLines.Clear();

        }


        public void EndSnapping(Canvas canvas)
        {
            foreach (var guideLine in _guideLines)
            {
                canvas.RemoveAdornerFigure(guideLine);
            }
        }

        public int Priority { get; set; } = 1;
    }
}