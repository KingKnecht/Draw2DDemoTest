using System;
using Draw2D.Core.Geo;
using Draw2D.Core.Utlils;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public class RegionDragDropEditPolicy : DragDropEditPolicy, IRegionAwarePolicy
    {
        public RegionDragDropEditPolicy(Rectangle region)
        {
            Region = region;
        }

        public Rectangle Region { get; set; }

        public override AdjustSizeResult AdjustSizeByDelta(Figure figure, float dTop, float dRight, float dBottom, float dLeft,
            AdjustSizeResult adjustSizeResult)
        {
            var bb = figure.GetBoundingBoxIncludeLinked();
            bb.AdjustDimensions(dTop, dRight, dBottom, dLeft);
            

            if (Region.Contains(bb))
            {
               return adjustSizeResult;
            }

            if (bb.Top > Region.Top)
            {
                adjustSizeResult.DeltaTop = 0;
                adjustSizeResult.LockDeltaTop();
            }

            if (bb.Right > Region.Right)
            {
                adjustSizeResult.DeltaRight = 0;
                adjustSizeResult.LockDeltaRight();
            }

            if (bb.Bottom < Region.Bottom)
            {
                adjustSizeResult.DeltaBottom = 0;
                adjustSizeResult.LockDeltaBottom();
            }

            if (bb.Left < Region.Left)
            {
                adjustSizeResult.DeltaLeft = 0;
                adjustSizeResult.LockDeltaLeft();
            }

            return adjustSizeResult;
        }

        public override AdjustPositionResult AdjustPositionByDelta(Figure figure, float dx, float dy, AdjustPositionResult adjustmentResult)
        {
            var bb = figure.GetBoundingBoxIncludeLinked();
            bb.X += dx;
            bb.Y += dy;

            if (Region.Contains(bb))
            {
                return adjustmentResult;
            }

            if (bb.Top > Region.Top)
            {
                adjustmentResult.Dy = 0;
                adjustmentResult.LockDy();
                
            }
            if (bb.Left < Region.Left)
            {
                adjustmentResult.Dx = 0;
                adjustmentResult.LockDx();
            }
            if (bb.Right > Region.Right)
            {
                adjustmentResult.Dx = 0;
                adjustmentResult.LockDx();

            }
            if (bb.Bottom < Region.Bottom)
            {
                adjustmentResult.Dy = 0;
                adjustmentResult.LockDy();
            }

            return adjustmentResult;

        }
    }

    public class FigureRegionDragDropEditPolicy : DragDropEditPolicy, IRegionAwarePolicy
    {
        private readonly Figure _regionFigure;

        public FigureRegionDragDropEditPolicy(Figure regionFigure)
        {
            _regionFigure = regionFigure;
        }

        public Rectangle Region
        {
            get => _regionFigure.BoundingBox;
            set { ; }
        }

        public override AdjustSizeResult AdjustSizeByDelta(Figure figure, float dTop, float dRight, float dBottom, float dLeft,
            AdjustSizeResult adjustSizeResult)
        {
            var bb = figure.GetBoundingBoxIncludeLinked();
            bb.AdjustDimensions(dTop, dRight, dBottom, dLeft);


            if (Region.Contains(bb))
            {
                return adjustSizeResult;
            }

            if (bb.Top > Region.Top)
            {
                adjustSizeResult.DeltaTop = 0;
                adjustSizeResult.LockDeltaTop();
            }

            if (bb.Right > Region.Right)
            {
                adjustSizeResult.DeltaRight = 0;
                adjustSizeResult.LockDeltaRight();
            }

            if (bb.Bottom < Region.Bottom)
            {
                adjustSizeResult.DeltaBottom = 0;
                adjustSizeResult.LockDeltaBottom();
            }

            if (bb.Left < Region.Left)
            {
                adjustSizeResult.DeltaLeft = 0;
                adjustSizeResult.LockDeltaLeft();
            }

            return adjustSizeResult;
        }

        public override AdjustPositionResult AdjustPositionByDelta(Figure figure, float dx, float dy, AdjustPositionResult adjustmentResult)
        {
            var bb = figure.GetBoundingBoxIncludeLinked();
            bb.X += dx;
            bb.Y += dy;

            if (Region.Contains(bb))
            {
                return adjustmentResult;
            }

            if (bb.Top > Region.Top)
            {
                adjustmentResult.Dy = 0;
                adjustmentResult.LockDy();

            }
            if (bb.Left < Region.Left)
            {
                adjustmentResult.Dx = 0;
                adjustmentResult.LockDx();
            }
            if (bb.Right > Region.Right)
            {
                adjustmentResult.Dx = 0;
                adjustmentResult.LockDx();

            }
            if (bb.Bottom < Region.Bottom)
            {
                adjustmentResult.Dy = 0;
                adjustmentResult.LockDy();
            }

            return adjustmentResult;

        }
    }
}