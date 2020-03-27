using System.Windows.Media;
using Draw2D.Core.Handles;

namespace Draw2D.Core.Shapes.Basic
{
    public class GraphVertexHandle : VectorFigure, IHandle
    {
        private readonly GraphNode _node;
        private readonly Graph _graph;
        public Figure Owner { get; }
        public VectorFigure HandleShape { get; set; }

        public GraphVertexHandle(VectorFigure handleShape, GraphNode node, Graph graph)
        {
            _node = node;
            _graph = graph;
            HandleShape = handleShape;
            Owner = graph;

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
            ForceSetPositionOfCenter(_node.X, _node.Y);
            HandleShape.ForceSetPositionOfCenter(_node.X, _node.Y);
        }
        public override void Render(DrawingContext dc)
        {
            if (HandleShape != null)
            {
                HandleShape.Canvas = Canvas;
                HandleShape.Render(dc);
            }
            
        }

        public override void ForceSetPositionCenter(float x, float y)
        {
            base.ForceSetPositionCenter(x, y);
            HandleShape.ForceSetPositionOfCenter(x, y);
        }
    }
}