using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draw2D.Core.Constants;
using Draw2D.Core.Handles;

namespace Draw2D.Core.Shapes.FigureExtensions
{
    public static class FigureExtensions
    {
        public static Figure AddHandles(this Figure owner, VectorFigure handleShape, float offsetX, float offsetY, params ResizeDirections[] directions)
        {
            foreach (var direction in directions)
            {
                owner.AddHandle(new ResizeHandle(owner, handleShape, offsetX, offsetY, direction));
            }

            return owner;
        }

        public static Figure AddHandles(this Figure owner, VectorFigure handleShape, params ResizeDirections[] directions)
        {
            foreach (var direction in directions)
            {
                owner.AddHandle(new ResizeHandle(owner, handleShape, 0, 0, direction));
            }

            return owner;
        }

        public static Figure AddHandles(this Figure figure, ICanvas canvas, HandleSizes handleSizes, HandleShapeType shapeType, float offsetX, float offsetY, params ResizeDirections[] directions)
        {
            foreach (var direction in directions)
            {
                switch (shapeType)
                {
                  case HandleShapeType.Round:
                         figure.AddHandle(new ResizeHandle(figure, canvas.HandleShapeFactory.GetRoundHandleShape(handleSizes),
                            offsetX, offsetY, direction));
                        break;
                    case HandleShapeType.Square:
                         figure.AddHandle(new ResizeHandle(figure, canvas.HandleShapeFactory.GetSquareHandleShape(handleSizes),
                            offsetX, offsetY, direction));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(shapeType), shapeType, null);
                }
            }

            return figure;
        }

        public static Figure AddHandles(this Figure figure, ICanvas canvas, HandleSizes handleSizes, HandleShapeType shapeType, params ResizeDirections[] directions)
        {
            return figure.AddHandles(canvas, handleSizes, shapeType, 0, 0, directions);
        }

        public static Figure AddHandlesAllDirections(this Figure figure, ICanvas canvas, HandleSizes handleSizes, HandleShapeType shapeType)
        {
            return figure.AddHandles(canvas, handleSizes, shapeType, 0f, 0f, ResizeDirections.TopLeft, ResizeDirections.Top,
                ResizeDirections.TopRight, ResizeDirections.Right, ResizeDirections.BottomRight, ResizeDirections.Bottom,
                ResizeDirections.BottomLeft, ResizeDirections.Left);
        }

        public static Figure AddHandlesTopBottomDirections(this Figure figure, ICanvas canvas, HandleSizes handleSizes, HandleShapeType shapeType)
        {
            return figure.AddHandles(canvas, handleSizes, shapeType, 0f, 0f, ResizeDirections.Top, ResizeDirections.Bottom);
        }

        public static Figure AddHandlesLeftRightDirections(this Figure figure, ICanvas canvas, HandleSizes handleSizes, HandleShapeType shapeType)
        {
            return figure.AddHandles(canvas, handleSizes, shapeType, 0f, 0f, ResizeDirections.Left, ResizeDirections.Right);
        }

        public static Figure AddHandlesCornerDirections(this Figure figure, ICanvas canvas, HandleSizes handleSizes, HandleShapeType shapeType)
        {
            return figure.AddHandles(canvas, handleSizes, shapeType, 0f, 0f, ResizeDirections.TopLeft, ResizeDirections.TopRight,
                ResizeDirections.BottomRight, ResizeDirections.BottomLeft);
        }
    }
}
