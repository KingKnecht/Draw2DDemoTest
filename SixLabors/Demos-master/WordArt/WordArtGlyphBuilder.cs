﻿using SixLabors.Fonts;
using SixLabors.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace WordArt
{
    internal class WordArtGlyphBuilder : IGlyphRenderer
    {
        private readonly PathBuilder builder = new PathBuilder();
        private readonly List<IPath> paths = new List<IPath>();
        private Vector2 currentPoint = default(Vector2);
        private IPath path;
        private float yOffset = 0;
        public WordArtGlyphBuilder(IPath path, SixLabors.Fonts.Size size)
        {
            this.yOffset = size.Height;

            this.builder = new PathBuilder();
            this.path = path;
            this.builder.SetTransform(Matrix3x2.Identity);
        }

        /// <summary>
        /// Gets the paths that have been rendered by this.
        /// </summary>
        public IEnumerable<IPath> Paths => this.paths;

        const float Pi = (float)Math.PI;
        const float HalfPi = Pi / 2f;
        /// <summary>
        /// Begins the glyph.
        /// </summary>
        /// <param name="location">The offset that the glyph will be rendered at.</param>
        void IGlyphRenderer.BeginGlyph(Vector2 location)
        {
            var point = this.path.PointAlongPath(location.X);

            var targetPoint = point.Point + new Vector2(0, (location.Y - this.yOffset));
            
            var matrix = Matrix3x2.CreateTranslation(targetPoint - location) * Matrix3x2.CreateRotation(point.Angle - Pi, point.Point);
            this.builder.SetTransform(matrix);
            this.builder.Clear();
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
        void IGlyphRenderer.CubicBezierTo(Vector2 secondControlPoint, Vector2 thirdControlPoint, Vector2 point)
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
        void IGlyphRenderer.LineTo(Vector2 point)
        {
            this.builder.AddLine(this.currentPoint, point);
            this.currentPoint = point;
        }

        /// <summary>
        /// Moves to current point to the supplied vector.
        /// </summary>
        /// <param name="point">The point.</param>
        void IGlyphRenderer.MoveTo(Vector2 point)
        {
            this.builder.StartFigure();
            this.currentPoint = point;
        }

        /// <summary>
        /// Draws a quadratics bezier from the current point  to the <paramref name="point"/>
        /// </summary>
        /// <param name="secondControlPoint">The second control point.</param>
        /// <param name="point">The point.</param>
        void IGlyphRenderer.QuadraticBezierTo(Vector2 secondControlPoint, Vector2 point)
        {
            Vector2 c1 = (((secondControlPoint - this.currentPoint) * 2) / 3) + this.currentPoint;
            Vector2 c2 = (((secondControlPoint - point) * 2) / 3) + point;

            this.builder.AddBezier(this.currentPoint, c1, c2, point);
            this.currentPoint = point;
        }
    }
}
