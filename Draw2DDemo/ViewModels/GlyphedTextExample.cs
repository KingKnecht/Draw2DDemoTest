using Caliburn.Micro;
using Draw2D.Core.Shapes.Basic;

namespace Draw2DDemo.ViewModels
{
    public class GlyphedTextExample : ExampleViewModel
    {
        public GlyphedTextExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Glyphed Text Example 1";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            //var rect1 = new Rectangle(30, 30, 100, 100) { FillColor = Colors.Chocolate };
            //rect1.InstallEditPolicy(SelectionFeedbackPolicy);
            //rect1.AddHandlesCornerDirections(Canvas, HandleSizes.Medium, HandleShapeType.Round);
            //Canvas.AddFigure(rect1);

            var message =
                "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

            var glyphedText = new GlyphedText(message, 100, 100, 100, 30);
            Canvas.AddFigure(glyphedText);
        }
    }
}