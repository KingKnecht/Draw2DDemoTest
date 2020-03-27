using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draw2D.Core.Layout
{
    public class Locator
    {
        public virtual void Bind(Figure figure, Figure child)
        {
           // child.IsDragable = false;
           // child.IsSelectable = false;
        }

        public virtual void Unbind(Figure figure, Figure child)
        {
            
        }
    }

    public class PortLocator : Locator
    {
        
    }

    public class LeftTopAbsPortLocator : PortLocator
    {
        private readonly double _x;
        private readonly double _y;

        public LeftTopAbsPortLocator(double x, double y)
        {
            _x = x;
            _y = y;
        }
    }

    public class DragableLocator : Locator
    {
        public override void Bind(Figure figure, Figure child)
        {
            child.IsDragable = true;
            child.IsSelectable = true;
        }
    }
}
