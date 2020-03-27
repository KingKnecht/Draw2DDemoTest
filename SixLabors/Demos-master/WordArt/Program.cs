using ImageSharp;
using ImageSharp.Drawing.Brushes;
using SixLabors.Fonts;
using SixLabors.Shapes;
using System;
using System.Linq;
using System.Numerics;

namespace WordArt
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Image img = new Image(1500, 500))
            {
                var curve = new Path(new BezierLineSegment(new Vector2(50, 450), new Vector2(200, 50), new Vector2(300, 50), new Vector2(450, 450))).Translate(new Vector2(500, 0));
                var font = new Font(FontCollection.SystemFonts.Find("Arial"), 39, FontStyle.Regular);
                string text = "Hello World";
                text = string.Join(" ", Enumerable.Repeat(text, 5));
                img.Fill(Color.White);
                img.Fill(Color.Gray, curve.GenerateOutline(3));
                img.DrawTextAlongPath(text, font, Brushes.Solid(Color.Black), curve, new ImageSharp.Drawing.TextGraphicsOptions(true) {
                    WrapTextWidth = curve.Length 
                });

                img.Save("curve.png");
            }
        }
    }
}