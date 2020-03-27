using System.Windows.Media;
using Caliburn.Micro;
using Draw2D.Core;
using Draw2D.Core.Constants;
using Draw2D.Core.Handles;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Policies.RouterPolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Shapes.FigureExtensions;
using Draw2D.Core.Utlils;

namespace Draw2DDemo.ViewModels
{
    public class HandlesExample : ExampleViewModel
    {
        public HandlesExample(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            DisplayName = "Handles";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var rect1 = new Rectangle(100, 100, 100, 100);
            var resizeHandleTop = new ResizeHandle(rect1, new Ellipse(0, 0, 30, 30)
            {
                FillColor = Colors.Red.AdjustOpacity(0.5)

            }, 0, 0, ResizeDirections.Top);

            resizeHandleTop.InstallEditPolicy(new SnapGridPolicy());
            rect1.AddHandle(resizeHandleTop);
            rect1.InstallEditPolicy(SelectionFeedbackPolicy);
            rect1.InstallEditPolicy(new DragFeedbackPolicy());

            Canvas.AddFigure(rect1);

            var rect2 = new Rectangle(250, 100, 100, 100);
            rect2.AddHandle(new ResizeHandle(rect2, new Ellipse(0, 0, 30, 30), 0, 0, ResizeDirections.Top));
            rect2.AddHandle(new ResizeHandle(rect2, new Ellipse(0, 0, 30, 30), 0, 0, ResizeDirections.Left));
            rect2.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(rect2);

            var rect3 = new Rectangle(400, 100, 100, 100);
            rect3.AddHandlesCornerDirections(Canvas, HandleSizes.Big, HandleShapeType.Round);
            rect3.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(rect3);

            var rect4 = new Rectangle(100, 300, 100, 100);
            rect4.AddHandlesLeftRightDirections(Canvas, HandleSizes.Medium, HandleShapeType.Square);
            rect4.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(rect4);

            var rect5 = new Rectangle(250, 300, 100, 100);
            rect5.AddHandlesAllDirections(Canvas, HandleSizes.Medium, HandleShapeType.Square);
            rect5.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(rect5);

            var rect6 = new Rectangle(400, 300, 100, 100);
            rect6.AddHandlesLeftRightDirections(Canvas, HandleSizes.Medium, HandleShapeType.Square);
            rect6.AddHandlesTopBottomDirections(Canvas, HandleSizes.Medium, HandleShapeType.Round);
            rect6.InstallEditPolicy(SelectionFeedbackPolicy);
            Canvas.AddFigure(rect6);
        }
    }
}