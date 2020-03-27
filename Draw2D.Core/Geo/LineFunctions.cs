using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draw2D.Core.Geo
{
    public static class LineFunctions
    {
        public static double Distance(double x1, double y1, double x2, double y2,double px, double py)
        {
            // Adjust vectors relative to X1,Y1
            // X2,Y2 becomes relative vector from X1,Y1 to end of segment
            x2 -= x1;
            y2 -= y1;
            // px,py becomes relative vector from X1,Y1 to test point
            px -= x1;
            py -= y1;
            var dotprod = px * x2 + py * y2;
            double projlenSq;
            if (dotprod <= 0.0)
            {
                // px,py is on the side of X1,Y1 away from X2,Y2
                // distance to segment is length of px,py vector
                // "length of its (clipped) projection" is now 0.0
                projlenSq = 0.0;
            }
            else
            {
                // switch to backwards vectors relative to X2,Y2
                // X2,Y2 are already the negative of X1,Y1=>X2,Y2
                // to get px,py to be the negative of px,py=>X2,Y2
                // the dot product of two negated vectors is the same
                // as the dot product of the two normal vectors
                px = x2 - px;
                py = y2 - py;
                dotprod = px * x2 + py * y2;
                if (dotprod <= 0.0)
                {
                    // px,py is on the side of X2,Y2 away from X1,Y1
                    // distance to segment is length of (backwards) px,py vector
                    // "length of its (clipped) projection" is now 0.0
                    projlenSq = 0.0;
                }
                else
                {
                    // px,py is between X1,Y1 and X2,Y2
                    // dotprod is the length of the px,py vector
                    // projected on the X2,Y2=>X1,Y1 vector times the
                    // length of the X2,Y2=>X1,Y1 vector
                    projlenSq = dotprod * dotprod / (x2 * x2 + y2 * y2);
                }
            }
            // Distance to line is now the length of the relative point
            // vector minus the length of its projection onto the line
            // (which is zero if the projection falls outside the range
            //  of the line segment).
            var lenSq = px * px + py * py - projlenSq;
            if (lenSq < 0)
            {
                lenSq = 0;
            }
            return Math.Sqrt(lenSq);
        }
    }
}
