using Caliburn.Micro;
using Draw2D.Core.Constants;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Shapes.FigureExtensions;

namespace Draw2DDemo.ViewModels
{
    public class MasterSlaveExample : ExampleViewModel
    {
        public MasterSlaveExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Master Slave";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var regionPolicy = new RegionDragDropEditPolicy(new Draw2D.Core.Geo.Rectangle(0, 0, Canvas.Width, Canvas.Height));

            var rect1 = new Rectangle(100, 100, 100, 100);
            rect1.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            rect1.InstallEditPolicy(SelectionFeedbackPolicy);
            rect1.InstallEditPolicy(regionPolicy);
            Canvas.AddFigure(rect1);

            var text1 = new BoxedText("I'm a slave of the rect.\nNo selection feedback policy installed.", rect1.X + 10f, rect1.Y + 10f, 150, 30);
            rect1.InstallEditPolicy(new MasterSlaveDragDropPolicy(text1));
            Canvas.AddFigure(text1);

            var text2 = new BoxedText("I'm a slave of the rect.\nSelection feedback policy installed.", rect1.X + 10, rect1.Y + 110, 150, 30);
            text2.InstallEditPolicy(SelectionFeedbackPolicy);
            rect1.InstallEditPolicy(new MasterSlaveDragDropPolicy(text2));
            Canvas.AddFigure(text2);
        }
    }
}