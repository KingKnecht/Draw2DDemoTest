namespace Draw2D.Core.Policies.CanvasPolicy
{
    public interface IDimensionsAware
    {
        void OnDimensionsChanged(Canvas canvas, Figure figure);
        
    }
}