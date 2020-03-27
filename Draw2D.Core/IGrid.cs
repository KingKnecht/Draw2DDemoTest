using System;
using Draw2D.Core.Geo;

namespace Draw2D.Core
{
    public interface IGrid
    {
        Point GetNearestPoint(Point currentPos);
        int UnitX { get; set; }
        int UnitY { get; set; }
    }

    public class Grid : IGrid
    {
        public Point GetNearestPoint(Point currentPos)
        {
            var nextXPoint = Math.Round(currentPos.X / (double)UnitX) * UnitX;
            var nextYPoint = Math.Round(currentPos.Y / (double)UnitX) * UnitX;
            //return destinationPoint;
            return new Point((int)Math.Ceiling(nextXPoint), (int)Math.Ceiling(nextYPoint));
        }

        public int UnitX { get; set; }
        public int UnitY { get; set; }
    }
}