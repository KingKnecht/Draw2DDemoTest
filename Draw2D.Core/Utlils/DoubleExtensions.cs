using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draw2D.Core.Geo;

namespace Draw2D.Core.Utlils
{

    public static class PointExtensions
    {
        public static float[] PointToArray(this Point point)
        {
            return new[] {point.X, point.Y};
        }
    }

    public static class FloatExtensions
    {
        public static bool Same(this float x, float y)
        {
            return Math.Abs(x - y) < 0.001;
        }

        public static bool NotSame(this float x, float y)
        {
            return !Same(x, y);
        }
    }

    public static class DoubleExtensions
    {
        public static bool Same(this double x, double y)
        {
            return Math.Abs(x - y) < 0.001;
        }

        public static bool NotSame(this double x, double y)
        {
            return !Same(x, y);
        }
        public static float ToFloat(this double dValue)
        {
            if (float.IsPositiveInfinity(Convert.ToSingle(dValue)))
            {
                return float.MaxValue;
            }
            if (float.IsNegativeInfinity(Convert.ToSingle(dValue)))
            {
                return float.MinValue;
            }
            return Convert.ToSingle(dValue);
        }
    }
}
