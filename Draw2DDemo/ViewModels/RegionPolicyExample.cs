using System.Windows.Media;
using Caliburn.Micro;
using Draw2D.Core.Constants;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Shapes.FigureExtensions;

namespace Draw2DDemo.ViewModels
{
    public class RegionPolicyExample : ExampleViewModel
    {
        public RegionPolicyExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Figure Region Policy";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            
            var visiblePolicyRect = new Rectangle(0,0,500,500);
            visiblePolicyRect.FillColor = Colors.CornflowerBlue;
            visiblePolicyRect.IsDragable = true;
            visiblePolicyRect.IsSelectable = true;
            var regionPolicy = new FigureRegionDragDropEditPolicy(visiblePolicyRect);

            Canvas.AddFigure(visiblePolicyRect);

            var rect1 = new Rectangle(10, 10, 100, 100)
            {
                FillColor = Colors.LightCoral
            };
            rect1.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            rect1.InstallEditPolicy(regionPolicy);
            rect1.InstallEditPolicy(SelectionFeedbackPolicy);
            visiblePolicyRect.InstallEditPolicy(new MasterSlaveDragDropPolicy(rect1));
            Canvas.AddFigure(rect1);

            //var rect2 = new Rectangle(100, 100, 100, 100)
            //{
            //    FillColor = Colors.LightSeaGreen
            //};
            //rect2.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            //rect2.InstallEditPolicy(SelectionFeedbackPolicy);
            //Canvas.AddFigure(rect2);
        }
    }
}