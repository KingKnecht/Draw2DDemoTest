using Draw2D.Core.Geo;

namespace Draw2D.Core
{
    public class EmptyGrid : IGrid
    {
        private static EmptyGrid _instance;

        public Point GetNearestPoint(Point currentPos)
        {
            return new Point(currentPos.X, currentPos.Y );
        }
        
        public static EmptyGrid Grid = _instance ?? (_instance = new EmptyGrid()) ;

        public int UnitX { get; set; }
        public int UnitY { get; set; }
    }
}