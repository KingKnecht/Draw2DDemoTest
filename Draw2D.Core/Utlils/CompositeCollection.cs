using Draw2D.Core.Shapes.Composites;

namespace Draw2D.Core.Utlils
{
    public class CompositeCollection : ChildCollection<Composite, Figure>
    {
        public CompositeCollection(Composite parent) : base(parent)
        {
        }

        public CompositeCollection(Composite parent, int capacity) : base(parent, capacity)
        {
        }

        protected override Composite GetParent(Figure child)
        {
            return (Composite)child.Parent;
        }

        protected override void SetParent(Figure child, Composite parent)
        {
            child.Parent = parent;
        }
    }
}