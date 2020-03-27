using System;
using System.Linq;
using System.Windows.Media;
using Draw2D.Core.Geo;
using QuickGraph;

namespace Draw2D.Core.Shapes.Basic
{
    public class Graph : VectorFigure
    {
        private readonly BidirectionalGraph<GraphNode, GraphEdge> _graph = new BidirectionalGraph<GraphNode, GraphEdge>();

        public Graph()
        {

        }

        public void AddVerticesAndEdge(Point from, Point to)
        {
            var fromNode = new GraphNode(this, from);
            var toNode = new GraphNode(this, to);

            _graph.AddVerticesAndEdge(new GraphEdge(fromNode, toNode));

            InstallHandle(fromNode);
            InstallHandle(toNode);

            BoundingBox = CalculateBoundingBox();
        }

        private void InstallHandle(GraphNode node)
        {
            var handle = new GraphVertexHandle(new Ellipse(node.X, node.Y, 5, 5), node, this);

            handle.SetSnapTargets(SnapTargets.Center);

            AddHandle(handle);
        }

        private Geo.Rectangle CalculateBoundingBox()
        {
            var bb = Geo.Rectangle.GetBoundingBoxAroundPoints(_graph.Vertices);

            bb.Y -= StrokeThickness / 2;
            bb.X -= StrokeThickness / 2;

            bb.Height += StrokeThickness;
            bb.Width += StrokeThickness;

            return bb;
        }

        public float CoronaWidth { get; set; } = 5;



        public override bool HitTest(float x, float y)
        {
            foreach (var graphEdge in _graph.Edges)
            {
                if (Hit(CoronaWidth + StrokeThickness, graphEdge.Source.X,
                    graphEdge.Source.Y, graphEdge.Target.X, graphEdge.Target.Y, x, y))
                {
                    return true;
                }
            }

            return false;
        }

        protected bool Hit(float coronaWidth, float x1, float y1, float x2, float y2, float x, float y)
        {
            return LineFunctions.Distance(x1, y1, x2, y2, x, y) < coronaWidth;
        }



        public override void Render(DrawingContext dc)
        {
            var strokeBrush = new SolidColorBrush(StrokeColor);
            strokeBrush.Freeze();

            var pen = new Pen(strokeBrush, StrokeThickness);
            pen.Freeze();

            var geom = new StreamGeometry();
            using (StreamGeometryContext ctx = geom.Open())
            {
                foreach (var edge in _graph.Edges)
                {
                    var arr = Canvas.CoordinateSystem.ToScreenSpace(edge.Source);
                    var sourcePoint = new System.Windows.Point(arr[0],arr[1]);
                    arr = Canvas.CoordinateSystem.ToScreenSpace(edge.Target);
                    var targetPoint = new System.Windows.Point(arr[0], arr[1]);

                    ctx.BeginFigure(sourcePoint, false, false);
                    ctx.LineTo(targetPoint, true/* is stroked */, true /* is smooth join */);

                    geom.Freeze();
                    dc.DrawGeometry(null, pen, geom);
                }

                var strokeBrushNode = new SolidColorBrush(StrokeColor);
                strokeBrushNode.Freeze();
                var penNode = new Pen(strokeBrushNode, StrokeThickness)
                {
                    DashStyle = DashStyle
                };

                var fillBrushNode = new SolidColorBrush(StrokeColor);
                fillBrushNode.Freeze();
                pen.Freeze();

                foreach (var vertex in _graph.Vertices.Select(Canvas.CoordinateSystem.ToScreenSpace))
                {
                    dc.DrawEllipse(fillBrushNode, penNode, new System.Windows.Point(vertex[0], vertex[1]), 2, 2);
                }
            }

        }
    }


    public class GraphEdge : Edge<GraphNode>
    {
        public GraphEdge(GraphNode source, GraphNode target) : base(source, target)
        {
        }
    }

    public class GraphNode : Point
    {
        private readonly Graph _graph;

        public GraphNode(Graph graph, float x, float y) : base(x, y)
        {
            _graph = graph;
        }

        public GraphNode(Graph graph, Point point) : this(graph, point.X, point.Y)
        {
        }
    }
}