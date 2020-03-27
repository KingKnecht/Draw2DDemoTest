using System;
using Draw2D.Core.Constants;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public interface IAdjustPositionAndSize
    {
        AdjustPositionResult AdjustPositionByDelta(Figure figure, float dx, float dy,
            AdjustPositionResult adjustmentResult);

        AdjustSizeResult AdjustSizeByDelta(Figure figure, float dTop, float dRight, float dBottom, float dLeft,
            AdjustSizeResult adjustSizeResult);
    }

    public class ResizeMasterPolicy : ResizeMasterPolicyBase
    {
        public ResizeDirections ResizeDirection { get; set; }

        public ResizeMasterPolicy(Figure slave, ResizeDirections resizeDirection) : base(slave)
        {
            ResizeDirection = resizeDirection;
        }

        public override AdjustPositionResult AdjustPositionByDelta(Figure figure, float dx, float dy, AdjustPositionResult adjustmentResult)
        {
            var baseResult = base.AdjustPositionByDelta(figure, dx, dy, adjustmentResult);

            switch (ResizeDirection)
            {
                case ResizeDirections.TopLeft:
                    _slave.Resize(dy, 0, 0, dx);
                    break;
                case ResizeDirections.Top:
                    _slave.Resize(dy, 0, 0, 0);
                    break;
                case ResizeDirections.TopRight:
                    _slave.Resize(dy, dx, 0, 0);
                    break;
                case ResizeDirections.Right:
                    _slave.Resize(0, dx, 0, 0);
                    break;
                case ResizeDirections.BottomRight:
                    _slave.Resize(0, dx, dy, 0);
                    break;
                case ResizeDirections.Bottom:
                    _slave.Resize(0, 0, dy, 0);
                    break;
                case ResizeDirections.BottomLeft:
                    _slave.Resize(0, 0, dy, dx);
                    break;
                case ResizeDirections.Left:
                    _slave.Resize(0, 0, 0, dx);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return baseResult;
        }
    }
}