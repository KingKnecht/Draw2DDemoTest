using System;
using System.Windows;
using System.Windows.Media;
using Point = Draw2D.Core.Geo.Point;

namespace Draw2D.Core.Shapes.Basic
{
    public class BoxedText : Rectangle
    {
        public BoxedText(string message, string cultureInfo, FlowDirection direction, string font, int fontSize, Color fontColor, float x, float y, float width, float height)
            : base(x, y, width, height)
        {
            Message = message;
            CultureInfo = cultureInfo;
            Direction = direction;
            Font = font;
            FontSize = fontSize;
            FontColor = fontColor;
        }

        public BoxedText(string message, float x, float y, float width, float height) : base(x, y, width, height)
        {
            Message = message;
        }

        public BoxedText(string message) : base(0, 0, 100, 30)
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
            var brush = new SolidColorBrush(StrokeColor);
            brush.Freeze();

            var formattedtext = new FormattedText(Message,
                                    System.Globalization.CultureInfo.GetCultureInfo(CultureInfo),
                                    Direction,
                                    new Typeface(Font),
                                    FontSize,
                                    brush);

            formattedtext.MaxTextWidth = Width;
            formattedtext.MaxTextHeight = Height;

            var screenPoint = Canvas.CoordinateSystem.ToScreenSpace(Position);
            var offset = new Point((float)(screenPoint[0] - X),(float)(screenPoint[1] - Y - Height));

            dc.PushTransform(new TranslateTransform(offset.X, offset.Y));
            dc.DrawText(formattedtext, new Point(X, Y));
            dc.Pop();
        }
    }
}