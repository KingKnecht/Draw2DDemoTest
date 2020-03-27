using Draw2D.Core.Geo;

namespace Draw2D.Core.Policies.RouterPolicy
{
    internal class GuideLineFilterItem
    {

        public GuideLineFilterItem(Point from, Point to)
        {
            From = @from;
            To = to;
            var direction = to - from;
            SquareLength = direction.SquareLength;
            Direction = direction.Normalized();
        }

        public Point From { get; private set; }
        public Point To { get; private set; }
        public double SquareLength { get; set; }
        public Point Direction { get; }
    }
}