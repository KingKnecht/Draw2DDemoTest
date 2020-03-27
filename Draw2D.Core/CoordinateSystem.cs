using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Point = Draw2D.Core.Geo.Point;

namespace Draw2D.Core
{
    public class CartesianCoordinateSystem : ICoordinateSystem
    {
        public float OffsetX { get;  }
        public float OffsetY { get;   }
       

        public CartesianCoordinateSystem(float offsetX, float offsetY)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
       
        public Geo.Point ToWorldSpace(double screenPointX, double screenPointY)
        {
            var factor = 1; //internally everthing is stored in 1/100.
            return new Geo.Point((float)Math.Ceiling(screenPointX - OffsetX * factor),
                (float)Math.Ceiling(Canvas.Height * factor - screenPointY - OffsetY * factor));
        }

        public double[] ToScreenSpace(Geo.Point worldPoint)
        {
            var factor = 1; //internally everthing is stored in 1/100.
            return new double[]
            {
                worldPoint.X + OffsetX  / factor,
                -worldPoint.Y + Canvas.Height / factor - OffsetY  / factor
            };
        }

        public ICanvas Canvas { get; set; }
    }
}
