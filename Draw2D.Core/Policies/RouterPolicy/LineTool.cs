using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Draw2D.Core.Geo;
using Draw2D.Core.Handles;
using Draw2D.Core.Layout.Connection;
using Draw2D.Core.Shapes.Basic;
using Line = Draw2D.Core.Shapes.Basic.Line;

namespace Draw2D.Core.Policies.RouterPolicy
{
    public class LineTool : ToolBase
    {
        private Line _line;

        private bool _isLineAdded;

        private int _clickCount;
        private bool _isInitialized;
        private LineHandle _lineHandle;

        public override void OnMouseMove(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;

                _line = new Line(0, 0, 0, 0);
                _line.SetSnapTargets(SnapTargets.Vertices);
                _lineHandle = new LineHandle(new Ellipse(mouseX, mouseY, 10, 10), 0, _line)
                {
                    CanBeSnapTarget = false,
                    FillColor = Colors.Transparent
                };

                foreach (var snapPolicy in canvas.GetSnapPolicies())
                {
                    snapPolicy.InitSnap(canvas, _lineHandle.GetSnapPoints(), new []{_lineHandle});
                }

                canvas.Selection.Clear();

                canvas.AddFigure(_lineHandle);
            }

            var snapPoint = new Point(mouseX, mouseY);
            var isSnapped = false;

            foreach (var snapPolicy in canvas.GetSnapPolicies())
            {
                Point delta;
                isSnapped = snapPolicy.Snap(canvas, snapPoint, 0, 0, mouseX, mouseY, out snapPoint, out delta,
                    new[] { _lineHandle });

                if (isSnapped)
                    break;
            }

            if (isSnapped)
            {
                mouseX = snapPoint.X;
                mouseY = snapPoint.Y;
            }

            _lineHandle.ForceSetPositionCenter(mouseX, mouseY);

            if (_clickCount == 0)
            {
                _line[0] = new Point(mouseX, mouseY);
                _line[1] = new Point(mouseX, mouseY);
            }
            else
            {
                _line[1] = new Point(mouseX, mouseY);
            }
        }

        public override void OnMouseLeftDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey,
            bool isCtrlKey)
        {
            _clickCount++;

            if (_clickCount == 1)
            {
                var snapPoint = new Point(mouseX, mouseY);
                var isSnapped = false;

                foreach (var snapPolicy in canvas.GetSnapPolicies())
                {
                    Point delta;
                    isSnapped = snapPolicy.Snap(canvas, new Point(mouseX, mouseY), 0, 0, mouseX, mouseY, out snapPoint, out delta, new[] { _lineHandle });

                    if (isSnapped)
                        break;
                }

                if (isSnapped)
                {
                    mouseX = snapPoint.X;
                    mouseY = snapPoint.Y;
                }

                _line[0] = new Point(mouseX,mouseY);
            }

            if (_clickCount == 2)
            {
                ExecuteOnDone(canvas);
            }

            if (!_isLineAdded)
            {
                canvas.AddFigure(_line);
                _isLineAdded = true;
            }
        }

        private void ExecuteOnDone(Canvas canvas)
        {
            InstallHandles();
            _line.Select();

            canvas.RemoveFigure(_lineHandle.HandleShape);//Todo: Check this.
            canvas.RemoveFigure(_lineHandle);

            foreach (var snapPolicy in canvas.GetSnapPolicies())
            {
                snapPolicy.EndSnapping(canvas);
            }

            OnDone(this);
        }

        private void InstallHandles()
        {
            for (var i = 0; i < _line.PointCount; i++)
            {
                _line.InstallLineHandle(i);
            }
        }

        public override void OnMouseRightDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey,
            bool isCtrlKey)
        {
            _clickCount--;
            if (_clickCount == 0)
            {
                Cancel(canvas);
            }

        }

        public override void Cancel(Canvas canvas)
        {
            base.Cancel(canvas);

            canvas.RemoveFigure(_line);
            canvas.RemoveFigure(_lineHandle.HandleShape); //Todo: Check this
            canvas.RemoveFigure(_lineHandle);

            _isInitialized = false;
        }

        public override void Reroute(IReadOnlyList<Point> points)
        {

        }
    }
}