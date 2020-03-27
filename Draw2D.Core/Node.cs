using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
using Draw2D.Core.Geo;
using Draw2D.Core.Utlils;

namespace Draw2D.Core
{
    public class Node<TNodePayload>
    {
        public TNodePayload Payload { get; }
        public Point Point { get;}
        public float Position { get; }

        public Node(float position, TNodePayload payload, Point point)
        {
            Position = Math.Round(position,3).ToFloat();
            Payload = payload;
            Point = point.Clone();
        }
    }
}
