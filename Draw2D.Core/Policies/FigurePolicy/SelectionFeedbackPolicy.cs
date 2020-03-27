using System.Linq;
using Draw2D.Core.Policies.CanvasPolicy;

namespace Draw2D.Core.Policies.FigurePolicy
{

    public class DragFeedbackPolicy : PolicyBase
    {
        public virtual void OnDragStart(Canvas canvas, VectorFigure figure, double x, double y)
        {
            figure.EnableDragFeedback(true);
        }

        public virtual void OnDrag(Canvas canvas, VectorFigure figure, double dxSum, double dySum, double dx, double dy, bool isShiftKey,
            bool isCtrlKey)
        {
            
        }

        public void OnDragEnd(Canvas canvas, VectorFigure figure, bool isShiftKey, bool isCtrlKey)
        {
            figure.EnableDragFeedback(false);
        }
    }
    public class SelectionFeedbackPolicy : PolicyBase
    {
        public virtual void OnSelect(Canvas canvas, Figure figure)
        {
            figure.EnableSelectionFeedback(true);
        }

        public virtual void OnUnselect(Canvas canvas, Figure figure)
        {
            figure.EnableSelectionFeedback(false);
        }

        internal override void OnInstall(Figure hostFigure)
        {
            base.OnInstall(hostFigure);

            var canvas = hostFigure.Canvas;
            if (canvas != null)
            {
                if (canvas.Selection.Contains(hostFigure))
                {
                    OnSelect(canvas, hostFigure);
                }
            }
        }
    }
}