using System;
using System.Windows.Media;
using Draw2D.Core.Constants;
using Draw2D.Core.Geo;

namespace Draw2D.Core.Handles
{

    public class ResizeHandle : VectorFigure, IHandle
    {
        private readonly float _offsetX;
        private readonly float _offsetY;
        public ResizeDirections Direction { get; set; }

        public ResizeHandle(Figure owner, VectorFigure handleShape, float offsetX, float offsetY, ResizeDirections direction)
        {
            HandleShape = handleShape;
            
            Width = handleShape.Width;
            Height = handleShape.Height;

            _offsetX = offsetX;
            _offsetY = offsetY;
            handleShape.IsDragable = false;
            handleShape.IsVisible = false;
            handleShape.IsSelectable = false;
            handleShape.CanBeSnapTarget = false;
            
            Direction = direction;

            IsDragable = true;
            IsVisible = true;
            IsSelectable = true;
            CanBeSnapTarget = false;
            SetSnapTargets(SnapTargets.Center);

            Owner = owner;

            Update();
        }

        public VectorFigure HandleShape { get; set; }

        //public override bool OnDragStart(Canvas canvas, float x, float y)
        //{
        //    base.OnDragStart(canvas, x, y);

        //    foreach (var policy in canvas.GetSnapPolicies())
        //    {
        //        policy.InitSnap(canvas, GetSnapPoints());
        //    }

        //    return true;
        //}

        public override void OnDrag(Canvas canvas, float dxSum, float dySum, float dx, float dy, bool isShiftKey, bool isCtrlKey)
        {
            if (!IsDragable)
                return;

            var delta = new Point(0, 0);
            var isSnapped = false;

            foreach (var policy in canvas.GetSnapPolicies())
            {
                Point snapPoint;
                isSnapped = policy.Snap(canvas, HandleShape.Position, dx, dy, dxSum, dySum, out snapPoint,out delta, new []{this});

                if (isSnapped)
                    break;
            }

            if (isSnapped)
            {
                dx = delta.X;
                dy = delta.Y;
            }
            
            switch (Direction)
            {
                case ResizeDirections.TopLeft:
                    Owner.Resize(dy, 0, 0, dx);
                    break;
                case ResizeDirections.Top:
                    Owner.Resize(dy, 0, 0, 0);
                    break;
                case ResizeDirections.TopRight:
                    Owner.Resize(dy, dx, 0, 0);
                    break;
                case ResizeDirections.Right:
                    Owner.Resize(0, dx, 0, 0);
                    break;
                case ResizeDirections.BottomRight:
                    Owner.Resize(0, dx, dy, 0);
                    break;
                case ResizeDirections.Bottom:
                    Owner.Resize(0, 0, dy, 0);
                    break;
                case ResizeDirections.BottomLeft:
                    Owner.Resize(0, 0, dy, dx);
                    break;
                case ResizeDirections.Left:
                    Owner.Resize(0, 0, 0, dx);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public override void OnDragEnd(Canvas canvas, bool isShiftKey, bool isCtrlKey)
        {
            base.OnDragEnd(canvas, isShiftKey, isCtrlKey);

            canvas.SnapCluster.Remove(Owner);
            canvas.SnapCluster.Add(Owner.GetSnapPoints(), Owner);
        }

        public void Show(Canvas canvas)
        {
            canvas.AddAdornerFigure(this);

            BringToFront();
        }

        public void Hide(Canvas canvas)
        {
            canvas.RemoveAdornerFigure(this);
        }

        public void Update()
        {
            var bb = Owner.BoundingBox;

            switch (Direction)
            {
                case ResizeDirections.TopLeft:
                    ForceSetPositionOfCenter(bb.TopLeft);
                    HandleShape.ForceSetPositionOfCenter(bb.TopLeft);
                    break;
                case ResizeDirections.Top:
                    ForceSetPositionOfCenter(bb.TopCenter);
                    HandleShape.ForceSetPositionOfCenter(bb.TopCenter);
                    break;
                case ResizeDirections.TopRight:
                    ForceSetPositionOfCenter(bb.TopRight);
                    HandleShape.ForceSetPositionOfCenter(bb.TopRight);
                    break;
                case ResizeDirections.Right:
                    ForceSetPositionOfCenter(bb.RightCenter);
                    HandleShape.ForceSetPositionOfCenter(bb.RightCenter);
                    break;
                case ResizeDirections.BottomRight:
                    ForceSetPositionOfCenter(bb.BottomRight);
                    HandleShape.ForceSetPositionOfCenter(bb.BottomRight);
                    break;
                case ResizeDirections.Bottom:
                    ForceSetPositionOfCenter(bb.BottomCenter);
                    HandleShape.ForceSetPositionOfCenter(bb.BottomCenter);
                    break;
                case ResizeDirections.BottomLeft:
                    ForceSetPositionOfCenter(bb.BottomLeft);
                    HandleShape.ForceSetPositionOfCenter(bb.BottomLeft);
                    break;
                case ResizeDirections.Left:
                    ForceSetPositionOfCenter(bb.LeftCenter);
                    HandleShape.ForceSetPositionOfCenter(bb.LeftCenter);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Render(DrawingContext dc)
        {
            if (HandleShape != null)
            {
                HandleShape.Canvas = Canvas;
                HandleShape.Render(dc);
            }
            
        }

        public Figure Owner { get; }
    }
}