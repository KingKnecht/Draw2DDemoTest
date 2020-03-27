using System;
using System.Diagnostics;
using System.Xml.Schema;
using Draw2D.Core.Constants;
using Draw2D.Core.Utlils;

namespace Draw2D.Core.Geo
{
    [DebuggerDisplay("({X},{Y})")]
    public class Point : IEquatable<Point>
    {
        private float? _boundaryX;
        private float? _boundaryY;
        protected float? BoundaryWidth;
        protected float? BoundaryHeight;
        private float _x;
        private float _y;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point(Point point) : this(point.X, point.Y)
        {

        }

        public virtual float X
        {
            get { return _x; }
            set
            {
                _x = value;
                AdjustBoundary();
            }
        }

        public virtual float Y
        {
            get { return _y; }
            set
            {
                _y = value;
                AdjustBoundary();
            }
        }
              

        protected virtual void AdjustBoundary()
        {
            if (!_boundaryX.HasValue || !_boundaryY.HasValue || !BoundaryWidth.HasValue || !BoundaryHeight.HasValue)
                return;

            X = Math.Min(Math.Max(_boundaryX.Value, X), BoundaryWidth.Value);
            Y = Math.Min(Math.Max(_boundaryY.Value, Y), BoundaryHeight.Value);

        }

        public virtual Point Translate(float dx, float dy)
        {
            X += dx;
            Y += dy;
            AdjustBoundary();

            return this;
        }

        public virtual Point Translated(float x, float y)
        {
            var other = new Point(x, y);
            return new Point(X + other.X, Y + other.Y);
        }

        public Point SetPosition(float x, float y)
        {
            X = x;
            Y = y;
            AdjustBoundary();

            return this;
        }

        public PositionConstants GetPosition(Point other)
        {
            var dx = other.X - X;
            var dy = other.Y - Y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (dx < 0)
                {
                    return PositionConstants.West;
                }
                return PositionConstants.East;
            }
            if (dy < 0)
            {
                return PositionConstants.North;
            }
            return PositionConstants.South;
        }



        public double Distance(Point other)
        {
            return Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));
        }

        public float Length => Math.Sqrt(X * X + Y * Y).ToFloat();

        public float SquareLength => X * X + Y * Y;


        public virtual Point Clone()
        {
            return new Point(X, Y);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }
        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator *(Point p1, float scalar)
        {
            return new Point(Math.Round(p1.X * scalar,3).ToFloat(), Math.Round(p1.Y * scalar,3).ToFloat());
        }

        public static Point operator /(Point p1, float scalar)
        {
            return new Point(Math.Round(p1.X / scalar,3).ToFloat(), Math.Round(p1.Y / scalar,3).ToFloat());
        }

        public static implicit operator System.Windows.Point(Point p)
        {
            return new System.Windows.Point(p.X, p.Y);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        #region Equality

        public bool Equals(Point other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Math.Abs(_x - other.X).Same(0) && Math.Abs(_y - other.Y).Same(0);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_x.GetHashCode() * 397) ^ _y.GetHashCode();
            }
        }

        public static bool operator ==(Point left, Point right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !Equals(left, right);
        }

        #endregion

        public Point Normalized()
        {
            return new Point(this / Length);
        }
    }



}