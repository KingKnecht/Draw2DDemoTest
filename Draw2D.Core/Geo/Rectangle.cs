using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Schema;
using Draw2D.Core.Utlils.Linq;

namespace Draw2D.Core.Geo
{
    [DebuggerDisplay("{X},{Y}, w:{Width}, h:{Height}")]
    public class Rectangle : Point, IEquatable<Rectangle>
    {
        private float _width;
        private float _height;

        public Rectangle(float x, float y, float width, float height) : base(x, y)
        {
            Width = width;
            Height = height;
        }

        public Rectangle(Point topLeft, Point bottomRight)
            : this(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y)
        { }

        public Rectangle(Rectangle rectangle) : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
        {

        }


        public virtual float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                AdjustBoundary();
            }
        }

        public virtual float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                AdjustBoundary();

            }
        }



        public float Left => X;
        public float Right => X + Width;
        public float Top => Y + Height;
        public float Bottom => Y;

        public Point BottomLeft => new Point(X, Y);
        public Point BottomRight => new Point(X + Width, Y);

        public Point BottomCenter => new Point(X + Width / 2, Y);
        public Point LeftCenter => new Point(X, Y + Height / 2);
        public Point RightCenter => new Point(X + Width, Y + Height / 2);
        public Point TopLeft => new Point(X, Y + Height);
        public Point TopCenter => new Point(X + Width / 2, Y + Height);
        public Point Center => new Point(X + Width / 2, Y + Height / 2);
        public Point TopRight => new Point(X + Width, Y + Height);

        public List<Point> GetVertices()
        {
            return new List<Point>()
            {
                TopLeft,
                TopRight,
                BottomRight,
                BottomLeft
            };
        }

        protected override void AdjustBoundary()
        {
            base.AdjustBoundary();

            if (!BoundaryWidth.HasValue || !BoundaryHeight.HasValue)
                return;

            Width = Math.Min(Width, BoundaryWidth.Value);
            Height = Math.Min(Height, BoundaryHeight.Value);
        }

        public Rectangle Resize(float dWidth, float dHeight)
        {
            Width += dWidth;
            Height += dHeight;

            AdjustBoundary();
            return this;
        }

        public Rectangle Scale(float dWidth, float dHeight)
        {
            Width += dWidth;
            Height += dHeight;
            X -= dWidth / 2;
            Y -= dHeight / 2;

            AdjustBoundary();

            return this;
        }

        public new Rectangle Translate(float dx, float dy)
        {
            //Todo: Point needed?
            var point = new Point(dx, dy);
            X += point.X;
            Y += point.Y;
            AdjustBoundary();

            return this;
        }

        public new Rectangle Translated(float dx, float dy)
        {
            return new Rectangle(X + dx, Y + dy, Width,  Height);
        }

        public Rectangle SetBounds(Rectangle bounds)
        {
            SetPosition(bounds.X, bounds.Y);
            Width = bounds.Width;
            Height = bounds.Height;

            return this;
        }

        public bool IsEmpty => Width <= 0 || Height <= 0;

        public bool HitTest(float iX, float iY)
        {
            var iX2 = X + Width;
            var iY2 = Y + Height;

            return (iX >= X && iX <= iX2 && iY >= Y && iY <= iY2);
        }

        public bool HitTest(Point point)
        {
            return HitTest(point.X, point.Y);
        }

        public new Rectangle Clone()
        {
            return new Rectangle(X, Y, Width, Height);
        }



        #region Equality

        public bool Equals(Rectangle other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && _width.Equals(other._width) && _height.Equals(other._height) &&
                   X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Rectangle)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ _width.GetHashCode();
                hashCode = (hashCode * 397) ^ _height.GetHashCode();
                hashCode = (hashCode * 397) ^ X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !Equals(left, right);
        }

        #endregion

        public Rectangle Union(Rectangle other)
        {
            var x1 = Math.Min(this.X, other.X);
            var x2 = Math.Max(this.X + this.Width, other.X + other.Width);
            var y1 = Math.Min(this.Y, other.Y);
            var y2 = Math.Max(this.Y + this.Height, other.Y + other.Height);

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static Rectangle GetBoundingBoxAroundPoints(IEnumerable<Point> points)
        {
            var pointList = points as IList<Point> ?? points.ToList();
            var xMax = pointList.MaxBy(p => p.X).X;
            var yMax = pointList.MaxBy(p => p.Y).Y;
            var xMin = pointList.MinBy(p => p.X).X;
            var yMin = pointList.MinBy(p => p.Y).Y;

            return new Rectangle(new Point(xMin, yMin), new Point(xMax, yMax));
        }

        public void AdjustDimensions(float dTop, float dRight, float dBottom, float dLeft)
        {
            Y += dBottom;
            X += dLeft;
            Height -= dBottom;
            Height += dTop;
            Width -= dLeft;
            Width += dRight;
        }

        public void AdjustWidth(float dx)
        {
            Width += dx;
        }

        public void AdjustHeight(float dy)
        {
            Height += dy;
        }

        public Rectangle Normalized()
        {
            var normalized = new Rectangle(this);
            if (normalized.Right < normalized.Left)
            {
                normalized.X = normalized.Right;
                normalized.Width = Math.Abs(Width);
            }

            if (normalized.Bottom > normalized.Top)
            {
                normalized.Y = normalized.Top;
                normalized.Height = Math.Abs(normalized.Height);
            }

            return normalized;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, w: {Width}, h: {Height}";
        }

        public Rectangle Extented(float extent)
        {
            return new Rectangle(this.X - extent, this.Y - extent, this.Width + 2 * extent, this.Height + 2 * extent);
        }

        public bool Intersects(Rectangle other)
        {

            return other.Left < Right &&
                   Left < other.Right &&
                   other.Bottom < Top &&
                   Bottom < other.Top;
        }

        //public bool Intersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        //{
        //    intersection = Vector2.Zero;

        //    Vector2 b = a2 - a1;
        //    Vector2 d = b2 - b1;
        //    float bDotDPerp = b.X * d.Y - b.Y * d.X;

        //    // if b dot d == 0, it means the lines are parallel so have infinite intersection points
        //    if (bDotDPerp == 0)
        //        return false;

        //    Vector2 c = b1 - a1;
        //    float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
        //    if (t < 0 || t > 1)
        //        return false;

        //    float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
        //    if (u < 0 || u > 1)
        //        return false;

        //    intersection = a1 + t * b;

        //    return true;
        //}

        public bool Contains(Rectangle other)
        {
            return Left <= other.Left
                   && Top >= other.Top
                   && Bottom <= other.Bottom
                   && Right >= other.Right;
        }
    }


}
