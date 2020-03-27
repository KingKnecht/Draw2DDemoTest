using System.Windows.Media;
using Caliburn.Micro;
using Draw2D.Core.Constants;
using Draw2D.Core.Geo;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Shapes.FigureExtensions;

namespace Draw2DDemo.ViewModels
{
    public class PathExample : ExampleViewModel
    {
        public PathExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Paths";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            var path1 = new Path();
            //CombinedGeometry combinedGeometry = new CombinedGeometry(
            //    GeometryCombineMode.Union, new EllipseGeometry(new Point(200, 200), 50, 50),
            //    new EllipseGeometry(new Point(230, 200), 50, 50));
            //path1.SetGeometry(combinedGeometry);
            var geometry = new EllipseGeometry(new Point(0, 0), 50, 50);
            path1.SetGeometry(geometry);
            path1.StrokeThickness = 5;
            path1.InstallEditPolicy(new SelectionFeedbackPolicy());
            path1.AddHandlesAllDirections(Canvas, HandleSizes.Medium, HandleShapeType.Round);
            //path1.InstallLineHandle(0);
            //path1.InstallLineHandle(1);
            Canvas.AddFigure(path1);


        }

    }
}