using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xaml;
using Caliburn.Micro;
using Draw2D.Core;
using Draw2D.Core.Constants;
using Draw2D.Core.Geo;
using Draw2D.Core.Layout.Connection;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Policies.RouterPolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Shapes.FigureExtensions;
using Draw2D.Core.Utlils;
using Rectangle = Draw2D.Core.Shapes.Basic.Rectangle;

namespace Draw2DDemo.ViewModels
{

   
    public class DragDropPoliciesExample : ExampleViewModel
    {
        public DragDropPoliciesExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "DragDrop Policies";


        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var regionPolicy = new RegionDragDropEditPolicy(new Draw2D.Core.Geo.Rectangle(0, 0, Canvas.Width, Canvas.Height));

            var rect1 = new Rectangle(100, 100, 100, 100);
            rect1.InstallEditPolicy(new DirectionDragDropPolicy(DragDropDirections.Horizontal));
            rect1.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            rect1.InstallEditPolicy(regionPolicy);
            rect1.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(rect1);

            var text1 = new BoxedText("I have a 'HorizontalDragBehavior' attached.", 225, 150, 300, 30);
            text1.IsDragable = false;
            rect1.InstallEditPolicy(new MasterSlaveDragDropPolicy(text1));
            Canvas.AddFigure(text1);


            var rect2 = new Rectangle(100, 300, 100, 100);
            rect2.InstallEditPolicy(new DirectionDragDropPolicy(DragDropDirections.Vertical));
            rect2.AddHandlesCornerDirections(Canvas, HandleSizes.Small, HandleShapeType.Round);
            rect2.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(rect2);

            var text2 = new BoxedText("I have a 'VerticalDragBehavior' attached.", 230, 330, 300, 30);
            text2.IsDragable = false;
            rect2.InstallEditPolicy(new MasterSlaveDragDropPolicy(text2));
            Canvas.AddFigure(text2);
        }
    }

}
