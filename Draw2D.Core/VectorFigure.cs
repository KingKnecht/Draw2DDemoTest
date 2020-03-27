using System;
using System.Linq;
using System.Windows.Media;
using Draw2D.Core.Geo;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Utlils;

namespace Draw2D.Core
{
    public abstract class VectorFigure : Figure
    {
        private Canvas _canvas;
        private DashStyle _storedDashStyle;
        private Color _storedFillColor;
        private Color _strokeColor = Colors.Black;
        private float _strokeThickness = 2;
        private DashStyle _dashStyle = DashStyles.Solid;
        private Color _fillColor = Colors.White;

        public Color FillColor
        {
            get { return _fillColor; }
            set
            {
                _fillColor = value;
                _storedFillColor = value;
                Canvas?.NeedsRepaint(this);
            }
        }

        public Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                _strokeColor = value;
                Canvas?.NeedsRepaint(this);
            }
        }

        public float StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                _strokeThickness = value;
                Canvas?.NeedsRepaint(this);
            }
        }

        public DashStyle DashStyle
        {
            get { return _dashStyle; }
            set
            {
                _dashStyle = value;
                Canvas?.NeedsRepaint(this);
            }
        }

        public override Figure EnableSelectionFeedback(bool isFeedbackEnabled)
        {
            if (isFeedbackEnabled)
            {
                _storedDashStyle = DashStyle;
                _storedFillColor = FillColor;
                FillColor = FillColor.Lighter(0.5);
                DashStyle = DashStyles.Dash;
            }
            else
            {
                FillColor = _storedFillColor;
                DashStyle = _storedDashStyle;
            }

            
            Canvas?.NeedsRepaint(this);

            return this;
        }

        public virtual Figure EnableDragFeedback(bool isFeedbackEnabled)
        {
            if (isFeedbackEnabled)
            {
                _storedDashStyle = DashStyle;
                _storedFillColor = FillColor;
                FillColor = FillColor.AdjustOpacity(0.5);
            }
            else
            {
                FillColor = _storedFillColor;
            }

            
            Canvas?.NeedsRepaint(this);

            return this;
        }


        public override void OnDragEnd(Canvas canvas, bool isShiftKey, bool isCtrlKey)
        {
            base.OnDragEnd(canvas, isShiftKey, isCtrlKey);

            foreach (var policy in Policies.OfType<DragFeedbackPolicy>())
            {
                policy.OnDragEnd(canvas, this, isShiftKey, isCtrlKey);
            }
        }

        public override void OnDrag(Canvas canvas, float dxSum, float dySum, float dx, float dy, bool isShiftKey, bool isCtrlKey)
        {
            base.OnDrag(canvas, dxSum, dySum, dx, dy, isShiftKey, isCtrlKey);

            foreach (var policy in Policies.OfType<DragFeedbackPolicy>())
            {
                policy.OnDrag(canvas, this, dxSum, dySum, dx, dy, isShiftKey, isCtrlKey);
            }

        }


        public override Canvas Canvas
        {
            get { return _canvas; }
            set
            {
                if (value == null)
                {
                    Unselect();
                }
                _canvas = value;

            }
        }

        public abstract void Render(DrawingContext dc);

    }
}