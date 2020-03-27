using System;
using System.Diagnostics;
using System.Xml.Schema;
using Draw2D.Core.Geo;
using Draw2D.Core.Policies.CanvasPolicy;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public class DragDropEditPolicy : PolicyBase, IAdjustPositionAndSize
    {
        public virtual Point AdjustPosition(Figure figure, float x, float y)
        {
            return new Point(x, y);
        }

        public virtual AdjustPositionResult AdjustPositionByDelta(Figure figure, float dx, float dy, AdjustPositionResult adjustmentResult)
        {
            return adjustmentResult;
        }

        public virtual AdjustSizeResult AdjustSizeByDelta(Figure figure, float dTop, float dRight, float dBottom, float dLeft,
            AdjustSizeResult adjustSizeResult)
        {
            return adjustSizeResult;
        }
    }
}