using System.Windows.Media;
using Caliburn.Micro;
using Draw2D.Core;
using Draw2D.Core.Constants;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Shapes.FigureExtensions;

namespace Draw2DDemo.ViewModels
{
    public class ShapesExample : ExampleViewModel
    {
        public ShapesExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Basic Shapes";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            Canvas.CoordinateSystem = new CartesianCoordinateSystem(Canvas.Width/2,Canvas.Height/2);
            
            var rect1 = new Rectangle(0, 0, 100, 100)
            {
                FillColor = Colors.CornflowerBlue,
                StrokeColor = Colors.Coral,
                StrokeThickness = 8
            };
            rect1.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            rect1.InstallEditPolicy(SelectionFeedbackPolicy);
            
            Canvas.AddFigure(rect1);

            var rect2 = new Rectangle(250, 100, 100, 100)
            {
                FillColor = Color.FromArgb(100, 100, 170, 80)
            };

            rect2.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            rect2.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(rect2);

            var ellipse2 = new Ellipse(100, 300, 100, 100)
            {
                FillColor = Colors.Transparent
            };
            ellipse2.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            ellipse2.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(ellipse2);

            var ellipse3 = new Ellipse(250, 300, 100, 100);
            ellipse3.FillColor = Colors.Brown;
            ellipse3.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            ellipse3.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(ellipse3);
        }
    }
}