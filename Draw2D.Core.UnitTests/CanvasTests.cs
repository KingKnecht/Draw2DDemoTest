using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draw2D.Core.Shapes.Basic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Draw2D.Core.UnitTests
{
    [TestClass()]
    public class CanvasTests
    {
       

    }

    [TestClass()]
    public class Geo_RectangleTests
    {
        [TestMethod]
        public void IntersectTest()
        {
            var rect1 = new Geo.Rectangle(0,0,100,100);
            var rect2 = new Geo.Rectangle(50, 50, 100, 100);

            Assert.IsTrue(rect1.Intersects(rect2));
            Assert.IsTrue(rect2.Intersects(rect1));

            rect1 = new Geo.Rectangle(0, 0, 100, 100);
            rect2 = new Geo.Rectangle(0, 0, 100, 100);
            Assert.IsTrue(rect1.Intersects(rect2));

            rect1 = new Geo.Rectangle(0, 0, 100, 100);
            rect2 = new Geo.Rectangle(0, 0, 1, 100);
            Assert.IsTrue(rect1.Intersects(rect2));
        }

    }
}
