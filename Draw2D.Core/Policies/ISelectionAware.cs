namespace Draw2D.Core.Policies.CanvasPolicy
{
    public interface ISelectionAware
    {
        void OnSelectionChanged(Canvas canvas, Figure figure);
    }
}