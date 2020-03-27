using System;
using Draw2D.Core.Constants;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public class FollowMasterResizePolicy : PolicyBase
    {
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public ResizeDirections ResizeDirection { get; set; }
        private readonly Figure _master;

        public FollowMasterResizePolicy(Figure master, float offsetX, float offsetY, ResizeDirections resizeDirection)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            ResizeDirection = resizeDirection;
            _master = master;
        }

        public virtual void OnSizeChanged(Figure figure, Figure changedFigure, float dTop, float dRight, float dBottom, float dLeft)
        {
            ForceSetPosition(figure);
        }

        internal override void OnInstall(Figure hostFigure)
        {
            base.OnInstall(hostFigure);

            ForceSetPosition(hostFigure);
        }

        private void ForceSetPosition(Figure hostFigure)
        {
            switch (ResizeDirection)
            {
                case ResizeDirections.TopLeft:
                    hostFigure.ForceSetPositionCenter(_master.BoundingBox.TopLeft.X + OffsetX, _master.BoundingBox.TopLeft.Y + OffsetY);
                    break;
                case ResizeDirections.Top:
                    hostFigure.ForceSetPositionCenter(_master.BoundingBox.TopCenter.X + OffsetX, _master.BoundingBox.TopCenter.Y + OffsetY);
                    break;
                case ResizeDirections.TopRight:
                    hostFigure.ForceSetPositionCenter(_master.BoundingBox.TopRight.X + OffsetX, _master.BoundingBox.TopRight.Y + OffsetY);
                    break;
                case ResizeDirections.Right:
                    hostFigure.ForceSetPositionCenter(_master.BoundingBox.RightCenter.X + OffsetX, _master.BoundingBox.RightCenter.Y + OffsetY);
                    break;
                case ResizeDirections.BottomRight:
                    hostFigure.ForceSetPositionCenter(_master.BoundingBox.BottomRight.X + OffsetX, _master.BoundingBox.BottomRight.Y + OffsetY);
                    break;
                case ResizeDirections.Bottom:
                    hostFigure.ForceSetPositionCenter(_master.BoundingBox.BottomCenter.X + OffsetX, _master.BoundingBox.BottomCenter.Y + OffsetY);
                    break;
                case ResizeDirections.BottomLeft:
                    hostFigure.ForceSetPositionCenter(_master.BoundingBox.BottomLeft.X + OffsetX, _master.BoundingBox.BottomLeft.Y + OffsetY);
                    break;
                case ResizeDirections.Left:
                    hostFigure.ForceSetPositionCenter(_master.BoundingBox.LeftCenter.X + OffsetX, _master.BoundingBox.LeftCenter.Y + OffsetY);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}