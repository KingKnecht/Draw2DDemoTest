using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Draw2D.Core.Geo;
using Draw2D.Core.Handles;
using Draw2D.Core.Layout.Connection;
using Draw2D.Core.Shapes.Basic;
using QuickGraph;

namespace Draw2D.Core.Policies.RouterPolicy
{
    public class PolylineTool : ToolBase
    {
        private PolyLine _polyline;
        private bool _isLineAdded;
        private int _clickCount;
        private bool _isInitialized;
        private LineHandle _lineHandle;

        public override void OnMouseMove(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _polyline = new PolyLine(0, 0, 0, 0);
                _polyline.SetSnapTargets(SnapTargets.Vertices);
                _lineHandle = new LineHandle(new Ellipse(mouseX, mouseY, 10, 10), 0, _polyline);


                foreach (var snapPolicy in canvas.GetSnapPolicies())
                {
                    snapPolicy.InitSnap(canvas, _lineHandle.GetSnapPoints(), new[] { _lineHandle });
                }

                canvas.Selection.Clear();

                canvas.AddFigure(_lineHandle);
            }

            Point snapPoint = new Point(mouseX, mouseY);
            bool isSnapped = false;
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
            _lineHandle.ForceSetPositionCenter(mouseX, mouseY);

            if (_clickCount == 0)
            {
                _polyline[0] = new Point(mouseX, mouseY);
                _polyline[1] = new Point(mouseX, mouseY);
            }
            else
            {
                _polyline[_clickCount] = new Point(mouseX, mouseY);
            }
        }

        public override void OnMouseLeftDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
            _clickCount++;

            if (_clickCount == 1)
            {
                Point snapPoint = new Point(mouseX, mouseY);
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

                _polyline[0] = new Point(mouseX, mouseY);
            }


            if (!_isLineAdded)
            {
                canvas.AddFigure(_polyline);
                _isLineAdded = true;
            }
        }

        private void ExecuteOnDone(Canvas canvas)
        {

            canvas.RemoveFigure(_lineHandle.HandleShape);//Todo: Check this.
            canvas.RemoveFigure(_lineHandle);

            InstallHandles();
            _polyline.Select();


            foreach (var snapPolicy in canvas.GetSnapPolicies())
            {
                snapPolicy.EndSnapping(canvas);
            }

            OnDone(this);
        }

        public override void OnMouseRightDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
            _clickCount--;

            if (_clickCount == 0)
            {
                Cancel(canvas);
            }
            else
            {
                _polyline.RemoveAt(_polyline.Points.Count - 1);
            }
        }

        public override void Cancel(Canvas canvas)
        {
            base.Cancel(canvas);

            foreach (var snapPolicy in canvas.GetSnapPolicies())
            {
                snapPolicy.EndSnapping(canvas);
            }

            canvas.RemoveFigure(_polyline);
            canvas.RemoveFigure(_lineHandle);
        }

        public override void Reroute(IReadOnlyList<Point> points)
        {
            throw new NotImplementedException();
        }

        public override void OnMouseLeftDoubleClick(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
            ExecuteOnDone(canvas);
        }
        private void InstallHandles()
        {
            for (var i = 0; i < _polyline.PointCount; i++)
            {
                _polyline.InstallLineHandle(i);
            }
        }
    }
}