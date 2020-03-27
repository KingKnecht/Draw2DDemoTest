namespace Draw2D.Core.Policies.CanvasPolicy
{
    public abstract class SelectionPolicy : CanvasPolicy
    {
        public abstract void Select(Canvas canvas, Figure figure);

        public virtual void Unselect(Canvas canvas, Figure figure)
        {
            canvas.Selection.Remove(figure);
        }
    }
}