﻿using System;
using System.Collections.Generic;
using System.Numerics;

using SixLabors.Fonts;
using SixLabors.Shapes;
using SixLabors.Primitives;

namespace SixLabors.Shapes.Text
{

    /// <summary>
    /// rendering surface that Fonts can use to generate Shapes.
    /// </summary>
    internal class BaseGlyphBuilder : IGlyphRenderer
    {
        protected readonly PathBuilder builder = new PathBuilder();
        private readonly List<IPath> paths = new List<IPath>();
        private Vector2 currentPoint = default(Vector2);

        /// <summary>
        /// Initializes a new instance of the <see cref="GlyphBuilder"/> class.
        /// </summary>
        public BaseGlyphBuilder()
        {
            // glyphs are renderd realative to bottom left so invert the Y axis to allow it to render on top left origin surface
            this.builder = new PathBuilder();
        }


        /// <summary>
        /// Gets the paths that have been rendered by this.
        /// </summary>
        public IPathCollection Paths => new PathCollection(this.paths);

        void IGlyphRenderer.EndText()
        {
        }

        void IGlyphRenderer.BeginText(RectangleF rect)
        {
            BeginText(rect);
        }

        protected virtual void BeginText(RectangleF rect)
        {
        }

        bool IGlyphRenderer.BeginGlyph(RectangleF rect, int hashCode)
        {
            this.builder.Clear();
            BeginGlyph(rect);
            return true;
        }

        protected virtual void BeginGlyph(RectangleF rect)
        {
        }

        /// <summary>
        /// Begins the figure.
        /// </summary>
        void IGlyphRenderer.BeginFigure()
        {
            this.builder.StartFigure();
        }

        /// <summary>
        /// Draws a cubic bezier from the current point  to the <paramref name="point"/>
        /// </summary>
        /// <param name="secondControlPoint">The second control point.</param>
        /// <param name="thirdControlPoint">The third control point.</param>
        /// <param name="point">The point.</param>
        void IGlyphRenderer.CubicBezierTo(PointF secondControlPoint, PointF thirdControlPoint, PointF point)
        {
            this.builder.AddBezier(this.currentPoint, secondControlPoint, thirdControlPoint, point);
            this.currentPoint = point;
        }

        /// <summary>
        /// Ends the glyph.
        /// </summary>
        void IGlyphRenderer.EndGlyph()
        {
            this.paths.Add(this.builder.Build());
        }

        /// <summary>
        /// Ends the figure.
        /// </summary>
        void IGlyphRenderer.EndFigure()
        {
            this.builder.CloseFigure();
        }

        /// <summary>
        /// Draws a line from the current point  to the <paramref name="point"/>.
        /// </summary>
        /// <param name="point">The point.</param>
        void IGlyphRenderer.LineTo(PointF point)
        {
            this.builder.AddLine(this.currentPoint, point);
            this.currentPoint = point;
        }

        /// <summary>
        /// Moves to current point to the supplied vector.
        /// </summary>
        /// <param name="point">The point.</param>
        void IGlyphRenderer.MoveTo(PointF point)
        {
            this.builder.StartFigure();
            this.currentPoint = point;
        }

        /// <summary>
        /// Draws a quadratics bezier from the current point  to the <paramref name="point"/>
        /// </summary>
        /// <param name="secondControlPoint">The second control point.</param>
        /// <param name="point">The point.</param>
        void IGlyphRenderer.QuadraticBezierTo(PointF secondControlPoint, PointF point)
        {
            this.builder.AddBezier(this.currentPoint, secondControlPoint, point);
            this.currentPoint = point;
        }

    }
}
