using Draw2D.Core.Geo;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public interface IRegionAwarePolicy
    {
        Rectangle Region { get; set; }
    }
}