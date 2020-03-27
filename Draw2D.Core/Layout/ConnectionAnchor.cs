using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draw2D.Core.Geo;

namespace Draw2D.Core.Layout
{
    public class ConnectionAnchor
    {
        public Figure Owner { get; set; }

        public ConnectionAnchor(Figure owner)
        {
            Owner = owner;
        }

        public Point GetLocation(Point reference, Core.Connection inquiringConnection)
        {
            return GetReferencePoint(inquiringConnection);
        }

        public Rectangle GetOwnerBox()
        {
           return Owner?.GetAbsoluteBounds();
        }

        public Point GetReferencePoint(Core.Connection inquiringConnection)
        {
           return  Owner?.GetAbsolutePosition();
        }
    }
}
