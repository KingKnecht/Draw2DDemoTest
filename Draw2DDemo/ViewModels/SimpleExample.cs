using Caliburn.Micro;
using Draw2D.Core.Geo;
using Draw2D.Core.Shapes.Basic;

namespace Draw2DDemo.ViewModels
{
    public class SimpleExample : ExampleViewModel
    {
        public SimpleExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Simple Example 1";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            //var rect1 = new Rectangle(110, 110, 90, 110);
            //Canvas.AddFigure(rect1);

            var graph = new Graph();
            var vertex1 = new Point(100, 100);
            var vertex2 = new Point(300, 100);
            var vertex3 = new Point(300, 300);
            var vertex4 = new Point(200, 200);

            graph.AddVerticesAndEdge(vertex1, vertex2);
            graph.AddVerticesAndEdge(vertex2, vertex3);
            graph.AddVerticesAndEdge(vertex2, vertex4);

            Canvas.AddFigure(graph);
        }
    }
}