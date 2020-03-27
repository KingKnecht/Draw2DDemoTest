using System.Windows.Media;
using Caliburn.Micro;
using Draw2D.Core.Constants;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Shapes.FigureExtensions;

namespace Draw2DDemo.ViewModels
{
    public class TextExample : ExampleViewModel
    {
        public TextExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Text Examples";


            // text2.Font = "Lucida Handwriting Kursiv";

        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var regionPolicy = new RegionDragDropEditPolicy(new Draw2D.Core.Geo.Rectangle(0, 0, Canvas.Width, Canvas.Height));

            var message =
                "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";

            var text1 = new BoxedText(message, 100, 100, 100, 100);
            text1.FillColor = Colors.LightSkyBlue;

            text1.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            text1.InstallEditPolicy(SelectionFeedbackPolicy);
            text1.InstallEditPolicy(regionPolicy);
            Canvas.AddFigure(text1);

            var text2 = new BoxedText("I'm a text with transparent background and big font size.", 250, 100, 400, 150)
            {
                FillColor = Colors.Transparent,
                FontSize = 24
            };

            text2.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            text2.InstallEditPolicy(regionPolicy);
            text2.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(text2);

            var text3 = new BoxedText("I'm a dark blue text.", 100, 300, 150, 30)
            {
                FillColor = Colors.White,
                FontSize = 16,
                StrokeColor = Colors.DarkBlue
            };
            text3.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            text3.InstallEditPolicy(regionPolicy);
            text3.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(text3);


            var text4 = new BoxedText("I'm text with fully transparent box.", 400, 300, 150, 30)
            {
                FillColor = Colors.Transparent,
                FontSize = 16,
                StrokeColor = Colors.Transparent,
            };

            text4.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            text4.InstallEditPolicy(regionPolicy);
            text4.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(text4);
        }
    }
}