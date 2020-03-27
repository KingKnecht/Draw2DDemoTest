using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draw2D.Core.Utlils
{
    public class FiguresCollection : ChildCollection<Figure,Figure>
    {
        public FiguresCollection(Figure parent) : base(parent)
        {
        }

        public FiguresCollection(Figure parent, int capacity) : base(parent, capacity)
        {
        }

        protected override Figure GetParent(Figure child)
        {
            return child.Parent;
        }

        protected override void SetParent(Figure child, Figure parent)
        {
            child.Parent = parent;
        }
    }
}
