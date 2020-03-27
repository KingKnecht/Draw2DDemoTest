using Caliburn.Micro;
using Draw2D.Core;
using Draw2D.Core.Policies.RouterPolicy;

namespace Draw2DDemo.ViewModels
{
    public class ConnectionRouterExample : ExampleViewModel
    {
        public ConnectionRouterExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Connection Router Example 1";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var router = new OrthogonalConnectionRouter((r) => { });
            var connection1 = new Connection(router, 0, 0, 100, 100)
            {
                StrokeThickness = 3
            };

            Canvas.AddFigure(connection1);

            //var polyLine = new PolyLine(0, 0, 100, 100)
            //{
            //    StrokeThickness = 3
            //};

            //polyLine.AddToEnd(350, 200);

            //Canvas.AddFigure(polyLine);

            //var router = new OrthogonalConnectionRouter((r) => {});
            //router.Reroute(connection1);
        }
    }
}