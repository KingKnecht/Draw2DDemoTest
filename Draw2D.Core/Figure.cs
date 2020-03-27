using System;
using System.Collections.Generic;
using System.Linq;
using Draw2D.Core.Geo;
using Draw2D.Core.Handles;
using Draw2D.Core.Layout;
using Draw2D.Core.Policies;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Utlils;



namespace Draw2D.Core
{

    [Flags]
    public enum SnapTargets
    {
        None = 0,
        Vertices = 1, //Corners of a rectangle, points of a line/polyline
        MidPoints = 2, //TopCenter,TopRight etc. of a rectangle. Mid-Points of a line/polyline.
        Center = 4, //Center of bounding box. Less useful with line/polyline.
    }



    public abstract class Figure
    {
        private readonly List<PolicyBase> _policies = new List<PolicyBase>();
        readonly List<IHandle> _handles = new List<IHandle>();
        private float _x;
        private float _y;
        private float _width;
        private float _height;
        private bool _isVisible;
        private int _zOrder;
        protected IHandle HittedResizeHandle;
        protected SnapTargets SnapTargets;

        protected readonly List<ConstraintPoint> FixConstraintPoints = new List<ConstraintPoint>();
        protected readonly List<ConstraintPoint> DynamicConstraintPoints = new List<ConstraintPoint>();

        public List<ConstraintPoint> ConstraintPoints => FixConstraintPoints.Concat(DynamicConstraintPoints).ToList();

        public int ZOrder
        {
            get { return _zOrder; }
            set
            {
                _zOrder = value;
                Canvas?.NeedsRepaint(this);
            }
        }

        protected Figure()
        {
            BoundingBox = new Geo.Rectangle(X, Y, Width, Height);
            IsDragable = true;
            IsSelectable = true;
            IsVisible = true;
            ZOrder = 0;

            SetSnapTargets(SnapTargets.Center | SnapTargets.MidPoints | SnapTargets.Vertices);
        }

        public void AddHandle(IHandle handle)
        {
            _handles.Add(handle);
        }

        public void RemoveHandle(IHandle handle)
        {
            _handles.Remove(handle);
        }

        public virtual bool HitTest(Point point)
        {
            return BoundingBox.HitTest(point);
        }

        public virtual bool HitTest(float x, float y)
        {
            return BoundingBox.HitTest(x, y);
        }

        public Figure InstallEditPolicy(PolicyBase policyBase)
        {
            if (policyBase is SelectionFeedbackPolicy)
            {
                var installedPolicy = _policies.OfType<SelectionFeedbackPolicy>().FirstOrDefault();

                installedPolicy?.OnUninstall(this);
            }

            policyBase.OnInstall(this);
            _policies.Add(policyBase);

            return this;
        }

        public virtual Figure BringToFront()
        {
            return Canvas?.BringToFront(this);
        }

        public virtual Figure SendToBack()
        {
            return Canvas?.SendToBack(this);
        }

        public Geo.Rectangle BoundingBox { get; set; }

        public float X
        {
            get { return _x; }
            protected set
            {
                _x = value;
                BoundingBox = new Geo.Rectangle(_x, Y, Width, Height);
            }
        }

        public float Y
        {
            get { return _y; }
            protected set
            {
                _y = value;
                BoundingBox = new Rectangle(X, _y, Width, Height);
            }
        }

        public float Width
        {
            get { return _width; }
            protected set
            {
                _width = value;
                BoundingBox = new Rectangle(X, Y, _width, Height);
            }
        }

        public float Height
        {
            get { return _height; }
            protected set
            {
                _height = value;
                BoundingBox = new Rectangle(X, Y, Width, _height);
            }
        }

        public Point Position => new Point(X, Y);
        public float MinWidth { get; set; } = 20;
        public float MinHeight { get; set; } = 20;


        public bool IsMoving { get; set; }

        public virtual Canvas Canvas { get; set; }


        public bool Contains(Figure figure)
        {
            //if (figure.Parent == this)
            //{
            //    return true;
            //}

            return _handles.Any(handle => handle.Owner == figure);

            //Todo: Children checks...
        }

        public virtual Figure Select(bool showHandles = true)
        {
            BringToFront();

            Canvas?.Selection.Add(this);

            foreach (var policy in _policies.OfType<SelectionFeedbackPolicy>())
            {
                policy.OnSelect(Canvas, this);
            }

            if (showHandles)
            {
                foreach (var handle in _handles)
                {
                    handle.Show(Canvas);
                }
            }

            foreach (var policy in _policies.OfType<ILink>())
            {
                policy.GetLinkedFigures().ToList().ForEach(s => s.BringToFront());
            }

            Canvas?.NeedsRepaint(this);

            return this;
        }

        public virtual Figure Unselect()
        {
            foreach (var policy in _policies.OfType<SelectionFeedbackPolicy>())
            {
                policy.OnUnselect(Canvas, this);
            }

            foreach (var handle in _handles)
            {
                handle.Hide(Canvas);
            }

            Canvas?.Selection.Remove(this);


            return this;
        }

        public bool IsSelected
        {
            get
            {
                if (Canvas != null)
                {
                    return Canvas.Selection.Contains(this);
                }

                return false;
            }
        }

        public IEnumerable<PolicyBase> Policies => _policies;
        public bool IsDragable { get; set; }
        public bool IsSelectable { get; set; }

        public void SetPostion(float newX, float newY)
        {
            foreach (var policy in _policies)
            {
                if (policy is DragDropEditPolicy)
                {
                    var dragDropPolicy = (DragDropEditPolicy)policy;
                    var newPos = dragDropPolicy.AdjustPosition(this, newX, newY);

                    if (!(Math.Abs(newPos.X - X) == 0 && Math.Abs(newPos.Y - Y) == 0))
                    {
                        X = newPos.X;
                        Y = newPos.Y;

                        Canvas?.NeedsRepaint(this);
                    }
                }
            }
        }

        public virtual void Translate(float dx, float dy)
        {
            if (!IsDragable)
                return;

            AdjustPositionResult adjustmentResult = new AdjustPositionResult(dx, dy);

            foreach (var policy in _policies)
            {
                if (policy is DragDropEditPolicy)
                {
                    var dragDropPolicy = (DragDropEditPolicy)policy;

                    adjustmentResult = dragDropPolicy.AdjustPositionByDelta(this, dx, dy, adjustmentResult);
                }
            }

            if (adjustmentResult.Dx != 0 || adjustmentResult.Dy != 0)
            {
                ForceTranslate(adjustmentResult.Dx, adjustmentResult.Dy);
            }
        }

        public virtual void ForceTranslate(float dx, float dy)
        {
            X += dx;
            Y += dy;

            foreach (var handle in _handles)
            {
                handle.Update();
            }

            Canvas?.OnFigureTranslated(this);


            Canvas?.NeedsRepaint(this);
        }

        public virtual void Resize(float dTop, float dRight, float dBottom, float dLeft)
        {
            if (!IsResizable)
                return;

            var adjustSizeResult = new AdjustSizeResult(dTop, dRight, dBottom, dLeft);

            foreach (var policy in _policies.OfType<IAdjustPositionAndSize>())
            {
                adjustSizeResult = policy.AdjustSizeByDelta(this, dTop, dRight, dBottom, dLeft, adjustSizeResult);
            }

            if (adjustSizeResult.DeltaTop != 0 || adjustSizeResult.DeltaRight != 0 || adjustSizeResult.DeltaBottom != 0 || adjustSizeResult.DeltaLeft != 0)
            {
                ApplyResize(adjustSizeResult.DeltaTop, adjustSizeResult.DeltaRight, adjustSizeResult.DeltaBottom, adjustSizeResult.DeltaLeft);

                foreach (var handle in _handles)
                {
                    handle.Update();
                }


                Canvas?.NeedsRepaint(this);
            }

        }

        private void ApplyResize(float dTop, float dRight, float dBottom, float dLeft)
        {
            var box = new Rectangle(X, Y, Width, Height);
            box.AdjustDimensions(dTop, dRight, dBottom, dLeft);

            if (box.Width >= MinWidth)
            {
                Width += dRight;
                X += dLeft;
                Width -= dLeft;
            }
            if (box.Height >= MinHeight)
            {
                Y += dBottom;
                Height += dTop;
                Height -= dBottom;
            }

            Canvas?.OnFigureTranslated(this);


            Canvas?.NeedsRepaint(this);
        }

        public virtual void ForceResize(float dTop, float dRight, float dBottom, float dLeft)
        {
            ApplyResize(dTop, dRight, dBottom, dLeft);
        }

        public void ForceSetDimensions(Geo.Rectangle rectangle)
        {
            X = rectangle.X;
            Y = rectangle.Y;
            Width = rectangle.Width;
            Height = rectangle.Height;


            Canvas?.NeedsRepaint(this);
        }

        public bool IsResizable { get; set; } = true;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;

                Canvas?.NeedsRepaint(this);
            }
        }

        public static IComparer<Figure> ZorderComparer { get; } = new ZorderComparer();

        public IEnumerable<IHandle> Handles => _handles;


        public virtual Figure EnableSelectionFeedback(bool isFeedbackEnabled)
        {

            return this;
        }



        public virtual bool OnDragStart(Canvas canvas, float x, float y)
        {
            HittedResizeHandle = _handles.FirstOrDefault(h => h.HitTest(x, y));
            if (HittedResizeHandle != null)
            {
                HittedResizeHandle.OnDragStart(canvas, x, y);
            }
            else
            {
                foreach (var policy in canvas.GetSnapPolicies())
                {
                    policy.InitSnap(canvas, GetSnapPoints(), new[] { this });
                }
            }

            return true;
        }


        public virtual void OnDrag(Canvas canvas, float dxSum, float dySum, float dx, float dy, bool isShiftKey, bool isCtrlKey)
        {
            if (HittedResizeHandle != null)
            {
                HittedResizeHandle.OnDrag(canvas, dxSum, dySum, dx, dy, isShiftKey, isCtrlKey);
            }
            else
            {
                var isSnapped = false;
                Point delta = null;
                foreach (var policy in canvas.GetSnapPolicies())
                {
                    Point snapPoint;
                    isSnapped = policy.Snap(canvas, Position, dx, dy, dxSum, dySum, out snapPoint, out delta, new[] { this });

                    if (isSnapped)
                        break;
                }

                if (isSnapped)
                {
                    Translate(delta.X, delta.Y);
                }
                else
                {
                    Translate(dx, dy);
                }
            }
        }

        public virtual void OnDragEnd(Canvas canvas, bool isShiftKey, bool isCtrlKey)
        {
            if (HittedResizeHandle != null)
            {
                HittedResizeHandle?.OnDragEnd(canvas, isShiftKey, isCtrlKey);
            }
            else
            {
                foreach (var policy in canvas.GetSnapPolicies())
                {
                    policy.EndSnapping(canvas);
                }
            }

            canvas.OnFigureDrag(this);
        }

        public Point GetAbsolutePosition()
        {
            return new Point(GetAbsoluteX(), GetAbsoluteY());
        }

        private float GetAbsoluteY()
        {
            return Y;//+ Parent?.GetAbsoluteY() ?? Y;
        }

        private float GetAbsoluteX()
        {
            return X;//+ Parent?.GetAbsoluteX() ?? X;
        }

        public Rectangle GetAbsoluteBounds()
        {
            return new Rectangle(GetAbsoluteX(), GetAbsoluteY(), Width, Height);
        }


        public Rectangle GetBoundingBoxIncludeLinked()
        {
            var resultBox = new Rectangle(BoundingBox);

            foreach (var linkedFigure in _policies.OfType<ILink>().SelectMany(p => p.GetLinkedFigures()))
            {
                resultBox = resultBox.Union(linkedFigure.BoundingBox);
            }

            return resultBox;
        }

        public void InformSizeChanged(Figure changedFigure, float dTop, float dRight, float dBottom, float dLeft)
        {
            foreach (var policy in _policies.OfType<FollowMasterResizePolicy>())
            {
                policy.OnSizeChanged(this, changedFigure, dTop, dRight, dBottom, dLeft);
            }
        }

        public void ForceSetPositionOfCenter(Point pos)
        {
            ForceSetPositionOfCenter(pos.X, pos.Y);
        }

        public void ForceSetPositionOfCenter(float x, float y)
        {
            ForceSetPosition(x - BoundingBox.Width / 2, y - BoundingBox.Height / 2);
        }

        public void ForceSetPosition(Point pos)
        {
            ForceSetPosition(pos.X, pos.Y);
        }

        public void ForceSetPosition(float x, float y)
        {
            X = x;
            Y = y;


            Canvas?.NeedsRepaint(this);
        }

        //Todo: Duplicate code.
        public virtual void ForceSetPositionCenter(float x, float y)
        {
            X = x - Width / 2;
            Y = y - Height / 2;


            Canvas?.NeedsRepaint(this);
        }

        public void InformSelectionChanged(Figure changedFigure, bool isSelected)
        {
            foreach (var policy in _policies.OfType<FollowSelectionChangedPolicy>())
            {
                policy.HandleSelectionChanged(this, changedFigure, isSelected);
            }
        }


        public void AddHandles(IEnumerable<ResizeHandle> handles)
        {
            foreach (var handle in handles)
            {
                AddHandle(handle);
            }
        }

        public void HideHandles(Canvas canvas)
        {
            foreach (var handle in Handles)
            {
                handle.Hide(Canvas);
            }
        }

        public void ShowHandles(Canvas canvas)
        {
            foreach (var handle in Handles)
            {
                handle.Show(Canvas);
            }
        }



        public virtual IEnumerable<Point> GetSnapPoints()
        {
            if (SnapTargets.HasFlag(SnapTargets.Vertices))
            {
                foreach (var vertex in BoundingBox.GetVertices())
                {
                    yield return vertex;
                }

            }
            if (SnapTargets.HasFlag(SnapTargets.MidPoints))
            {
                yield return BoundingBox.TopCenter.Clone();
                yield return BoundingBox.RightCenter.Clone();
                yield return BoundingBox.BottomCenter.Clone();
                yield return BoundingBox.LeftCenter.Clone();
            }
            if (SnapTargets.HasFlag(SnapTargets.Center))
            {
                yield return BoundingBox.Center;
            }

        }

        public bool CanBeSnapTarget { get; set; } = true;

        public void SetSnapTargets(SnapTargets snapTargets)
        {
            SnapTargets = snapTargets;
        }

    }
}

