using ImageSharp;
using ImageSharp.Drawing;
using SixLabors.Fonts;
using SixLabors.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WordArt
{
    public static class ImageExtensions
    {
        private static readonly Vector2 DefaultTextDpi = new Vector2(72);

        /// <summary>
        /// Draws the text onto the the image filled via the brush then outlined via the pen.
        /// </summary>
        /// <typeparam name="TColor">The type of the color.</typeparam>
        /// <param name="source">The image this method extends.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="location">The location.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// The <see cref="Image{TColor}" />.
        /// </returns>
        public static Image<TColor> DrawTextAlongPath<TColor>(this Image<TColor> source, string text, Font font, IBrush<TColor> brush, IPath path, TextGraphicsOptions options)
           where TColor : struct, IPixel<TColor>
        {

            Vector2 dpi = DefaultTextDpi;
            if (options.UseImageResolution)
            {
                dpi = new Vector2((float)source.MetaData.HorizontalResolution, (float)source.MetaData.VerticalResolution);
            }

            FontSpan style = new FontSpan(font, dpi)
            {
                ApplyKerning = options.ApplyKerning,
                TabWidth = options.TabWidth,
                WrappingWidth = options.WrapTextWidth,
                Alignment = TextAlignment.Center
            };
            var size = new TextMeasurer().MeasureText(text, style);
            WordArtGlyphBuilder glyphBuilder = new WordArtGlyphBuilder(path, size);

            TextRenderer renderer = new TextRenderer(glyphBuilder);

            renderer.RenderText(text, style);

            System.Collections.Generic.IEnumerable<SixLabors.Shapes.IPath> shapesToDraw = glyphBuilder.Paths;

            GraphicsOptions pathOptions = (GraphicsOptions)options;
            if (brush != null)
            {
                foreach (SixLabors.Shapes.IPath s in shapesToDraw)
                {
                    source.Fill(brush, s, pathOptions);
                }
            }

            return source;
        }
    }
}
