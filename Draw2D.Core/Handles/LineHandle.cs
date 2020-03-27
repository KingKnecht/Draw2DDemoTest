using System.Windows.Media;
using Draw2D.Core.Geo;
using Draw2D.Core.Shapes.Basic;

namespace Draw2D.Core.Handles
{
    internal class LineHandle : VectorFigure, IHandle
    {
        private readonly Line _line;
        public VectorFigure HandleShape { get; set; }
        public int LinePointIndex { get; private set; }

        public Figure Owner { get; }

        public LineHandle(VectorFigure handleShape, int linePointIndex, Line line)
        {

            _line = line;
            Owner = line;
            HandleShape = handleShape;
            LinePointIndex = linePointIndex;


            Width = HandleShape.Width;
            Height = handleShape.Height;

            Update();

            handleShape.IsDragable = false;
            handleShape.IsVisible = false;
            handleShape.IsSelectable = false;
            handleShape.CanBeSnapTarget = false;

            IsDragable = true;
            IsVisible = true;
            IsSelectable = true;
            CanBeSnapTarget = false;
           
            SetSnapTargets(SnapTargets.Center);
        }

        public override void OnDrag(Canvas canvas, float dxSum, float dySum, float dx, float dy, bool isShiftKey, bool isCtrlKey)
        {
            if (!IsDragable)
                return;

            var snapDelta = new Point(dx, dy);
            var isSnapped = false;

            foreach (var policy in canvas.GetSnapPolicies())
            {
                Point snapPoint;
                isSnapped = policy.Snap(canvas, HandleShape.Position, dx, dy, dxSum, dySum, out snapPoint, out snapDelta, new[]{ this});

                if (isSnapped)
                    break;
            }

            if (isSnapped)
            {
                dx = snapDelta.X;
                dy = snapDelta.Y;
            }
         
            _line.Translate(dx, dy, LinePointIndex);

            Update();

            
            Canvas?.NeedsRepaint(this);

        }


        public override void ForceSetPositionCenter(float x, float y)
        {
            base.ForceSetPositionCenter(x, y);
            HandleShape.ForceSetPositionOfCenter(x,y);
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
            ForceSetPositionOfCenter(_line[LinePointIndex].X, _line[LinePointIndex].Y);
            HandleShape.ForceSetPositionOfCenter(_line[LinePointIndex].X, _line[LinePointIndex].Y);
        }

        public override void Render(DrawingContext dc)
        {
            if (HandleShape != null)
            {
                HandleShape.Canvas = Canvas;
                HandleShape.Render(dc);
            }
            
        }
    }
}