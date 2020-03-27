using System;
using System.Linq;
using System.Windows.Media;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Utlils;

namespace Draw2D.Core.Policies.CanvasPolicy
{
    public abstract class SelectFeedbackPolicy : CanvasPolicy, ISelectionAware, IDimensionsAware
    {
        public abstract void OnSelectionChanged(Canvas canvas, Figure figure);
        public abstract void OnDimensionsChanged(Canvas canvas, Figure figure);

    }

    public class MultiSelectFeedbackPolicy : SelectFeedbackPolicy
    {
        private Rectangle _selectionBox;

        public override void OnSelectionChanged(Canvas canvas, Figure figure)
        {
            if (canvas.Selection.All.Count() > 1)
            {
                var bb = CalculateSelectionBox(canvas);

                if (_selectionBox == null)
                {
                    _selectionBox = new Selectionbox(bb)
                    {
                        DashStyle = DashStyles.Dash,
                        StrokeThickness = 1
                    };
                    canvas.AddFigure(_selectionBox);
                    _selectionBox.FillColor = Colors.Black.AdjustOpacity(0);
                    _selectionBox.IsSelectable = false;
                }

                _selectionBox.ForceSetDimensions(bb);

                canvas.Selection.All.ToList().ForEach(f => f.SendToBack());

                _selectionBox.BringToFront();
            }
            else
            {
                canvas.RemoveFigure(_selectionBox);
                _selectionBox = null;
            }
        }

        private static Geo.Rectangle CalculateSelectionBox(Canvas canvas)
        {
            var bb = canvas.Selection.All.First().BoundingBox;

            foreach (var bbOther in canvas.Selection.All.Skip(1).Select(f => f.BoundingBox))
            {
                bb = bb.Union(bbOther);
            }

            bb = bb.Extented(2);
            return bb;
        }

        public override void OnDimensionsChanged(Canvas canvas, Figure figure)
        {
            AdjustSelectionBox(canvas);
        }

        private void AdjustSelectionBox(Canvas canvas)
        {
            if (_selectionBox != null)
            {
                var bb = CalculateSelectionBox(canvas);
                _selectionBox.ForceSetDimensions(bb);
            }
        }

    }
}