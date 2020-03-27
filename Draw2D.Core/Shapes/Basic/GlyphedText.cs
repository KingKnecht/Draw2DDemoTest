using System;
using System.Windows;
using System.Windows.Media;
using Point = Draw2D.Core.Geo.Point;

namespace Draw2D.Core.Shapes.Basic
{
    public class GlyphedText : Rectangle
    {
        public GlyphedText(string message, string cultureInfo, FlowDirection direction, string font, int fontSize, Color fontColor, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Message = message;
            CultureInfo = cultureInfo;
            Direction = direction;
            Font = font;
            FontSize = fontSize;
            FontColor = fontColor;
        }

        public GlyphedText(string message, int x, int y, int width, int height) : base(x, y, width, height)
        {
            Message = message;
        }

        public GlyphedText(string message) : base(0, 0, 100, 30)
        {
            Message = message;
        }
        public string Message { get; set; }
        public string CultureInfo { get; set; } = System.Globalization.CultureInfo.CurrentCulture.ToString();
        public FlowDirection Direction { get; set; } = FlowDirection.LeftToRight;
        public string Font { get; set; } = "Arial";
        public int FontSize { get; set; } = 12;
        public Color FontColor { get; set; } = Colors.Black;

        public override void Render(DrawingContext dc)
        {
            var screenPoint = Canvas.CoordinateSystem.ToScreenSpace(Position);
            var offset = new System.Windows.Point(screenPoint[0] - X, screenPoint[1] - Y - Height);

            Typeface typeface = new Typeface(new FontFamily(Font),
                           FontStyles.Normal,
                           FontWeights.Normal,
                           FontStretches.Normal);

            GlyphTypeface glyphTypeface;
            if (!typeface.TryGetGlyphTypeface(out glyphTypeface))
                throw new InvalidOperationException("No glyphtypeface found");


            ushort[] glyphIndexes = new ushort[Message.Length];
            double[] advanceWidths = new double[Message.Length];

            var totalWidth = 0.0;

            for (int n = 0; n < Message.Length; n++)
            {
                ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[Message[n]];
                glyphIndexes[n] = glyphIndex;

                var width = glyphTypeface.AdvanceWidths[glyphIndex] * FontSize;
                advanceWidths[n] = width;

                totalWidth += width;
            }

            System.Windows.Point origin = new System.Windows.Point(X, Y);

            GlyphRun glyphRun = new GlyphRun(glyphTypeface, 0, false, FontSize,
                glyphIndexes, origin, advanceWidths, null, null, null, null,
                null, null);

            dc.PushTransform(new TranslateTransform(offset.X, offset.Y));
            dc.DrawGlyphRun(Brushes.Black, glyphRun);
            dc.Pop();
        }
    }
}