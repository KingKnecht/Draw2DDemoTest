using System.Windows.Media;
using Caliburn.Micro;
using Draw2D.Core.Constants;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Shapes.FigureExtensions;

namespace Draw2DDemo.ViewModels
{
    public class PerformanceTest1 : ExampleViewModel
    {
        public PerformanceTest1(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "PerformanceTest 1";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var message =
                "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

            var xCount = 300;
            var yCount = 300;

            Canvas.Width = xCount * 150 + 100;
            Canvas.Height = yCount * 150 + 100;

            Canvas.StartBulkEdit();
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    var rect1 = new Rectangle(i * 150, j * 150, 100, 100) { FillColor = Colors.Chocolate };
                    rect1.InstallEditPolicy(SelectionFeedbackPolicy);
                    rect1.AddHandlesCornerDirections(Canvas, HandleSizes.Medium, HandleShapeType.Round);
                    Canvas.AddFigure(rect1);

                    //var text1 = new BoxedText(message, i * 150, j * 150, 100, 100);
                    //text1.FillColor = Colors.LightSkyBlue;

                    //text1.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
                    //text1.InstallEditPolicy(SelectionFeedbackPolicy);
                    //Canvas.AddFigure(text1);
                }
            }
            Canvas.EndBulkEdit();

        }
    }
}