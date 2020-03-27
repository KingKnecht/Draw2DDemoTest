using System;
using System.CodeDom;
using Draw2D.Core.Constants;
using Draw2D.Core.Geo;
using Rectangle = Draw2D.Core.Shapes.Basic.Rectangle;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public class DirectionDragDropPolicy : DragDropEditPolicy
    {
        public DragDropDirections Direction { get; set; }

        public DirectionDragDropPolicy(DragDropDirections direction)
        {
            Direction = direction;
        }

        public override Point AdjustPosition(Figure figure, float x, float y)
        {
            switch (Direction)
            {
                case DragDropDirections.All:
                    return new Point(x, y);
                case DragDropDirections.Horizontal:
                    return new Point(x, figure.Y);
                case DragDropDirections.Vertical:
                    return new Point(figure.X, y);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override AdjustPositionResult AdjustPositionByDelta(Figure figure, float dx, float dy, AdjustPositionResult adjustmentResult)
        {

            switch (Direction)
            {
                case DragDropDirections.All:
                    adjustmentResult.Dx = dx;
                    adjustmentResult.Dy = dy;
                    break;
                case DragDropDirections.Horizontal:
                    adjustmentResult.Dx = dx;
                    adjustmentResult.Dy = 0;
                    adjustmentResult.LockDy();
                    break;
                case DragDropDirections.Vertical:
                    adjustmentResult.Dx = 0;
                    adjustmentResult.Dy = dy;
                    adjustmentResult.LockDx();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return adjustmentResult;
        }
    }
}