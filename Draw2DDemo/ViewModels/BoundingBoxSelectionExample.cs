using System.Windows.Media;
using Caliburn.Micro;
using Draw2D.Core.Constants;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Shapes.FigureExtensions;

namespace Draw2DDemo.ViewModels
{
    public class BoundingBoxSelectionExample : ExampleViewModel
    {
        public BoundingBoxSelectionExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "BoundingBox Selection";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var rect1 = new Rectangle(100, 100, 100, 100) { FillColor = Colors.Chocolate };
            rect1.InstallEditPolicy(SelectionFeedbackPolicy);
            rect1.AddHandlesCornerDirections(Canvas, HandleSizes.Medium, HandleShapeType.Round);
            Canvas.AddFigure(rect1);

            var rect2 = new Rectangle(400, 100, 100, 100) { FillColor = Colors.DarkGreen };
            rect2.InstallEditPolicy(SelectionFeedbackPolicy);
            rect2.AddHandlesCornerDirections(Canvas, HandleSizes.Medium, HandleShapeType.Round);
            Canvas.AddFigure(rect2);

            var rect3 = new Rectangle(200, 300, 100, 100) { FillColor = Colors.DarkRed };
            rect3.InstallEditPolicy(SelectionFeedbackPolicy);
            rect3.AddHandlesCornerDirections(Canvas, HandleSizes.Medium, HandleShapeType.Round);
            
            Canvas.AddFigure(rect3);
        }
    }
}