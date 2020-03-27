using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Draw2D.Core.Geo;
using Draw2D.Core.Layout.Connection;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Utlils;

namespace Draw2D.Core.Policies.RouterPolicy
{
    public class OrthogonalConnectionRouter : ConnectionRouter
    {
        private readonly Connection _connection;

        private bool _isLineAdded;
        private int _clickCount;

        public OrthogonalConnectionRouter(Action<Connection> onDone) : base(onDone)
        {
           
        }
        
        public override void OnMouseMove(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
          
            if (_clickCount == 0)
                return;

            var endPoint = new Point(mouseX, mouseY);
            _connection[1] = new Point(_connection[0].X + (mouseX - _connection[0].X) / 2, _connection[0].Y);
            _connection[2] = new Point(_connection[1].X, endPoint.Y);
            _connection[3] = endPoint;

            if (!_isLineAdded)
            {
                canvas.AddFigure(_connection);
                _isLineAdded = true;
            }
        }

        public override void OnMouseLeftDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
            _clickCount++;

            if (_clickCount == 1)
            {
                _connection[0] = new Point(mouseX, mouseY);
            }


            if (_clickCount == 2)
            {
                if (_connection[0].X == _connection[3].X || _connection[0].Y == _connection[3].Y)
                {
                    //Make a single line if x|y of start-point and end-point are equal.
                    _connection.RemoveAt(1);
                    _connection.RemoveAt(1);
                }
                OnDone(_connection);
            }


        }

        public override void OnMouseRightDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
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

            canvas.RemoveFigure(_connection);
        }

        public override void Reroute(Connection connection)
        {
            var points = new List<Point>();
            var startPoint = connection.StartPoint;
            var endPoint = connection.EndPoint;

            points.Add(startPoint);

            var diff =  endPoint - startPoint;
            points.Add(new Point(startPoint.X + diff.X / 2, startPoint.Y));
            points.Add(new Point(startPoint.X + diff.X / 2, endPoint.Y));

            points.Add(connection.EndPoint);

            connection.SetPoints(points);
        }
    }
}