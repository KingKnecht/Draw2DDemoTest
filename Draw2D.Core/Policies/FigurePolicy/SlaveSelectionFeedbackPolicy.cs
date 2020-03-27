using Draw2D.Core.Shapes.Basic;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public class SlaveSelectionFeedbackPolicy : SelectionFeedbackPolicy
    {
        private readonly Figure _master;
        private Line _line;

        public SlaveSelectionFeedbackPolicy(Figure master)
        {
            _master = master;
        }

        public override void OnSelect(Canvas canvas, Figure figure)
        {
            _line = new Line(figure.BoundingBox.Center, _master.BoundingBox.Center);
            canvas.AddFigure(_line);
        }

        public override void OnUnselect(Canvas canvas, Figure figure)
        {
            canvas.RemoveFigure(_line);
        }
    }
}