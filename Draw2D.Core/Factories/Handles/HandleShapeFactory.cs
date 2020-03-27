using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Draw2D.Core.Constants;
using Draw2D.Core.Policies.FigurePolicy;
using Draw2D.Core.Shapes.Basic;

namespace Draw2D.Core.Factories.Handles
{
    public class HandleShapeFactory
    {
        public Color FillColor { get; set; } = Colors.DarkMagenta;
        public Color StrokeColor { get; set; } = Colors.Black;
        public int StrokeThickness { get; set; } = 1;
        
        public VectorFigure GetRoundHandleShape(int width, int height)
        {
            return new Ellipse(0, 0, width, height)
            {
                MinWidth = width,
                MinHeight = height,
                FillColor = FillColor,
                StrokeColor = StrokeColor,
                StrokeThickness = StrokeThickness,
            };
        }

        public VectorFigure GetRoundHandleShape(HandleSizes handleSize)
        {
            switch (handleSize)
            {
                case HandleSizes.Custom:
                    throw new Exception("Use the explicit method with width/height to create a custom size.");
                case HandleSizes.Tiny:
                    return GetRoundHandleShape(5, 5);
                case HandleSizes.Small:
                    case HandleSizes.Medium:
                    return GetRoundHandleShape(12, 12);
                case HandleSizes.Big:
                    return GetRoundHandleShape(20, 20);
                case HandleSizes.Huge:
                    return GetRoundHandleShape(30, 30);
                default:
                    throw new ArgumentOutOfRangeException(nameof(handleSize), handleSize, null);
            }
        }

     

        public VectorFigure GetSquareHandleShape(int width, int height)
        {
            return new Rectangle(0, 0, width, height)
            {
                MinWidth = width,
                MinHeight = height,
                FillColor = FillColor,
                StrokeColor = StrokeColor,
                StrokeThickness = StrokeThickness,
            };
        }

        public VectorFigure GetSquareHandleShape(HandleSizes handleSize)
        {
            switch (handleSize)
            {
                case HandleSizes.Custom:
                    throw new Exception("Use the explicit method with width/height to create a custom size.");
                case HandleSizes.Tiny:
                    return GetSquareHandleShape(5, 5);
                case HandleSizes.Small:
                    return GetSquareHandleShape(10, 10);
                case HandleSizes.Medium:
                    return GetSquareHandleShape(15, 15);
                case HandleSizes.Big:
                    return GetSquareHandleShape(20, 20);
                case HandleSizes.Huge:
                    return GetSquareHandleShape(30, 30);
                default:
                    throw new ArgumentOutOfRangeException(nameof(handleSize), handleSize, null);
            }
        }
    }
}
