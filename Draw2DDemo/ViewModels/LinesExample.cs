using Caliburn.Micro;
using Draw2D.Core;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Shapes.Basic;

namespace Draw2DDemo.ViewModels
{
    public class LinesExample : ExampleViewModel
    {
        public LinesExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Lines and connections";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Canvas.CoordinateSystem = new CartesianCoordinateSystem(Canvas.Width / 2, Canvas.Height / 2);

            var line1 = new Line(0, 0, 100, 100);
            line1.StrokeThickness = 1;
            line1.InstallEditPolicy(new SelectionFeedbackPolicy());
            line1.InstallLineHandle(0);
            line1.InstallLineHandle(1);
            line1.InstallEditPolicy(new RegionDragDropEditPolicy(new Draw2D.Core.Geo.Rectangle(0, 0, Canvas.Width, Canvas.Height)));
            Canvas.AddFigure(line1);

            //line1.InstallEditPolicy(new DirectionDragDropPolicy(DragDropDirections.Vertical));

            //var line2 = new Line(50, 150, 150, 150);
            //line2.StrokeThickness = 2;
            //line2.InstallEditPolicy(new SelectionFeedbackPolicy());
            //line2.InstallEditPolicy(new DirectionDragDropPolicy(DragDropDirections.Horizontal));
            //Canvas.AddFigure(line2);

            //var line3 = new Line(50, 150, 150, 300);
            //line3.StrokeThickness = 5;
            //line3.InstallEditPolicy(new SelectionFeedbackPolicy());
            //line3.InstallEditPolicy(new RegionDragDropEditPolicy(new Draw2D.Core.Geo.Rectangle(0,0,Canvas.Width,Canvas.Height)));
            //Canvas.AddFigure(line3);

            //var router = new PolylineTool(r => RouterDone((PolylineTool)r) );
            //Canvas.ActiveTool = router;
        }

    }
}