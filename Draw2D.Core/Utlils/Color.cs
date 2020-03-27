using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Draw2D.Core.Utlils
{
    public static class ColorExtensions
    {
        public static Color Darker(this Color color, double fraction)
        {
            var red = (int)Math.Round(color.R * (1.0 - fraction));
            var green = (int)Math.Round(color.G * (1.0 - fraction));
            var blue = (int)Math.Round(color.B * (1.0 - fraction));

            if (red < 0) red = 0; else if (red > 255) red = 255;
            if (green < 0) green = 0; else if (green > 255) green = 255;
            if (blue < 0) blue = 0; else if (blue > 255) blue = 255;


            byte redByte = (byte)red;
            byte greenByte = (byte)green;
            byte blueByte = (byte)blue;


            return Color.FromArgb(color.A, redByte, greenByte, blueByte);
        }

        public static Color Lighter(this Color color, double fraction)
        {
            var red = (int)Math.Round(color.R * (1.0 + fraction));
            var green = (int)Math.Round(color.G * (1.0 + fraction));
            var blue = (int)Math.Round(color.B * (1.0 + fraction));

            if (red < 0) red = 0; else if (red > 255) red = 255;
            if (green < 0) green = 0; else if (green > 255) green = 255;
            if (blue < 0) blue = 0; else if (blue > 255) blue = 255;


            byte redByte = (byte)red;
            byte greenByte = (byte)green;
            byte blueByte = (byte)blue;


            return Color.FromArgb(color.A, redByte, greenByte, blueByte);
        }

        public static Color AdjustOpacity(this Color color, double fraction)
        {

            var opacity = (int)Math.Round(255 * fraction);
            if (opacity < 0) opacity = 0; else if (opacity > 255) opacity = 255;

            byte opacityByte = (byte)opacity; ;

          return Color.FromArgb(opacityByte, color.R,color.G, color.B);
        }
    }
}
