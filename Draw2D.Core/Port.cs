using System.Collections.Generic;
using Draw2D.Core.Geo;
using Draw2D.Core.Layout;

namespace Draw2D.Core
{
    public class Port : Shapes.Basic.Ellipse
    {
      
        public Port(int x, int y, int width, int height) : base(x, y, width, height)
        {
            ConnectionAnchor = new ConnectionAnchor(this);
        }

        public IEnumerable<Connection> Connections { get; } = new List<Connection>();

        public ConnectionAnchor ConnectionAnchor { get;private set; }

        public Point GetConnectionAnchorLocation(Point referencePoint, Connection inquiringConnection)
        {
            return ConnectionAnchor.GetLocation(referencePoint, inquiringConnection);
        }

        public Point GetConnectionAnchorReferencePoint(Connection inquiringConnection)
        {
            return ConnectionAnchor.GetReferencePoint(inquiringConnection);
        }

        public PortDirections PreferredDirection { get; set; }
        
    }

    public enum PortDirections
    {
        Up,
        Right,
        Down,
        Left
    }
}