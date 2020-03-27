using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draw2D.Core.Geo;

namespace Draw2D.Core
{

    public class Node
    {
        public VectorFigure Shape { get; set; }
        public Point Position { get; set; }

    }

    public class ConnectionGraphEdge : QuickGraph.Edge<Node>
    {
        public ConnectionGraphEdge(Node source, Node target) : base(source, target)
        {

        }
    }
}
