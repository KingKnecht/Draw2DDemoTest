using System;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public class SlaveMasterDragDropFeedbackPolicy : DragDropEditPolicy
    {
        private readonly Figure _master;

        public SlaveMasterDragDropFeedbackPolicy(Figure master)
        {
            _master = master;
        }

        public override AdjustPositionResult AdjustPositionByDelta(Figure figure, float dx, float dy, AdjustPositionResult adjustmentResult)
        {
            return adjustmentResult;
        }
    }
}