using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draw2D.Core.Geo;
using Draw2D.Core.Policies.CanvasPolicy;
using Draw2D.Core.Policies.RouterPolicy;
using Draw2D.Core.Shapes.Basic;

namespace Draw2D.Core.Layout.Connection
{
    public abstract class ToolBase : IMouseAware
    {
        internal Action<ToolBase> OnDone;

        public virtual void OnClick(Figure figure, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }

        public virtual void OnDoubleClick(Figure figure, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }

        public virtual void OnMouseLeftDoubleClick(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }

        public virtual void OnMouseMove(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }

        public virtual void OnMouseLeftDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }

        public virtual void OnMouseLeftUp(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }

        public virtual void OnMouseRightDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }

        public virtual void Cancel(Canvas canvas)
        {

        }

        public static Point SnapToAxis(Point startPoint, float endpointX, float endpointY)
        {
            var endPoint = new Point(endpointX, endpointY);

            if (Math.Abs(endpointX - startPoint.X) >= Math.Abs(endpointY - startPoint.Y))
            {
                endPoint = new Point(endpointX, startPoint.Y);
            }

            else if (Math.Abs(endpointX - startPoint.X) < Math.Abs(endpointY - startPoint.Y))
            {
                endPoint = new Point(startPoint.X, endpointY);
            }

            return endPoint;
        }

        public abstract void Reroute(IReadOnlyList<Point> points);
        //public virtual void Reroute(PolyLine polyLine)
        //{
        //    var points = new List<Point>
        //    {
        //        polyLine.StartPoint,
        //        polyLine.EndPoint
        //    };

        //    polyLine.SetPoints(points);
        //}
    }
}
