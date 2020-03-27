using System;
using System.Collections.Generic;
using System.Linq;
using Draw2D.Core.Geo;
using Draw2D.Core.Shapes.Basic;

namespace Draw2D.Core.Policies.CanvasPolicy
{
    public class SingleSelectionPolicy : SelectionPolicy, IMouseAware, IDragAware
    {
        private bool _mouseMovedDuringMouseDown;
        private Figure _mouseDraggingElement;
        private Figure _mouseDownElement;


        public virtual void OnClick(Figure figure, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
            throw new NotImplementedException();
        }

        public virtual void OnDoubleClick(Figure figure, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
            throw new NotImplementedException();
        }

        public virtual void OnMouseMove(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }

        public virtual void OnMouseLeftDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
            _mouseMovedDuringMouseDown = false;

            var figure = canvas.GetBestFigure(mouseX, mouseY, new List<Type> { typeof(Selectionbox) }, new List<Type>());

             if (figure == null)
            {
                Unselect(canvas, canvas.Selection.All);
                return;
            }

            if (figure.IsSelectable == false)
            {
                Unselect(canvas, canvas.Selection.All);
            }

            if (canvas.Selection.Contains(figure))
            {
                return;
            }

            //Ignore via policy linked figures. Only master will be selected.
            var slaves = canvas.Selection.All.SelectMany(f => f.Policies.OfType<ILink>())
                .SelectMany(f => f.GetLinkedFigures()).ToList();
            Unselect(canvas, slaves);

            //Unselect the slaves of the newly selected figure.
            var slavesOfToBeSelected = figure.Policies.OfType<ILink>().SelectMany(f => f.GetLinkedFigures()).ToList();
            Unselect(canvas, slavesOfToBeSelected);

            if (isCtrlKey)
            {
                if (canvas.Selection.Contains(figure))
                {
                    Unselect(canvas, figure);
                }
                else if (figure.IsSelectable)
                {
                    Select(canvas, figure);
                }
            }
            else
            {
                //Todo:Slect Problem ->  isDragging
                Unselect(canvas, canvas.Selection.All);
                Select(canvas, figure);
            }
        }

        public virtual void OnMouseLeftDoubleClick(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }


        public virtual void OnMouseDrag(Canvas canvas, float dxSum, float dySum, float dx, float dy, bool isShiftKey, bool isCtrlKey)
        {
            if (canvas == null)
                return;

            if (!canvas.Selection.All.Any())
            {
                //Panning of canvas because there is no selection to drag.
            }
            else
            {
                if (canvas.Selection.All.Count() == 1)
                {
                    canvas.Selection.All.ToList().ForEach(f => f.OnDrag(canvas, dxSum, dySum, dx, dy, isShiftKey, isCtrlKey));
                }
                else
                {
                    Point delta = new Point(dx, dy);
                    bool isSnapped = false;

                    foreach (var snapPolicy in canvas.GetSnapPolicies())
                    {
                        Point snapPoint;
                        isSnapped = snapPolicy.Snap(canvas, canvas.Selection.All.First().Position, dx, dy, dxSum, dySum, out snapPoint, out delta, canvas.Selection.All);

                        if (isSnapped)
                            break;
                    }

                    if (isSnapped)
                    {
                        dx = delta.X;
                        dy = delta.Y;
                    }

                    canvas.Selection.All.ToList().ForEach(f => f.Translate(dx, dy));
                }

            }
        }

        public virtual void OnDragStart(Canvas canvas, float startPosX, float startPosY, float dx, float dy, bool isShiftKey, bool isCtrlKey)
        {
            if (canvas == null)
                return;

            if (!canvas.Selection.All.Any())
            {
                //Panning of canvas because there is no selection to drag.
                var x = 5;
            }
            else
            {
                if (canvas.Selection.All.Count() == 1)
                {
                    canvas.Selection.All.ToList().ForEach(f => f.OnDragStart(canvas, startPosX, startPosY));
                }
                else
                {
                    foreach (var snapPolicy in canvas.GetSnapPolicies())
                    {
                        snapPolicy.InitSnap(canvas, canvas.Selection.All.SelectMany(f => f.GetSnapPoints()),
                            canvas.Selection.All);
                    }
                }

                //Do not forget the minimal drag distance already dragged.
                //canvas.Selection.All.ToList().ForEach(f => f.OnDrag(canvas,dx, dy, dx, dy, isShiftKey, isCtrlKey));
            }
        }

        public virtual void OnDragEnd(Canvas canvas, bool isShiftKey, bool isCtrlKey)
        {
            if (canvas == null)
                return;

            if (!canvas.Selection.All.Any())
            {
                //Panning of canvas because there is no selection to drag.
                var x = 5;
            }
            else
            {
                if (canvas.Selection.All.Count() == 1)
                {
                   foreach (var selectedFigure in canvas.Selection.All)
                    {
                        selectedFigure.ShowHandles(canvas);
                    }
                }
                else if (canvas.Selection.All.Count() > 1)
                {
                    foreach (var snapPolicy in canvas.GetSnapPolicies())
                    {
                        snapPolicy.EndSnapping(canvas);
                    }
                }

                canvas.Selection.All.ToList().ForEach(f => f.OnDragEnd(canvas, isShiftKey, isCtrlKey));

            }
        }

        public virtual void OnMouseLeftUp(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {
             if (canvas.Selection.All.Count() == 1)
            {
                foreach (var selectedFigure in canvas.Selection.All)
                {
                    selectedFigure.ShowHandles(canvas);
                }
            }
            else if (canvas.Selection.All.Count() > 1)
            {
                foreach (var selectedFigure in canvas.Selection.All)
                {
                    selectedFigure.HideHandles(canvas);
                }
            }
        }

        public virtual void OnMouseRightDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey)
        {

        }

        public override void Select(Canvas canvas, Figure figure)
        {
            if (figure == null || canvas == null)
                return;

            if (!figure.IsSelectable)
                return;

            if (canvas.Selection.Contains(figure))
                return;


            figure.Select(true);
            canvas.Selection.Primary = figure;

        }

        public override void Unselect(Canvas canvas, Figure figure)
        {
            if (figure == null)
                return;

            if (!figure.IsSelectable)
                return;

            canvas.Selection.Remove(figure);
            figure.Unselect();


        }

        private void Unselect(Canvas canvas, IEnumerable<Figure> all)
        {
            var newList = new List<Figure>(all);

            foreach (var figure in newList)
            {
                Unselect(canvas, figure);
            }

        }
    }
}