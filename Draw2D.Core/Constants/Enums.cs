using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draw2D.Core.Constants
{

    public enum PositionConstants
    {
        North,
        East,
        South,
        West    
    }

    public enum ResizeDirections
    {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left
    }

    public enum DragDropDirections
    {
        All,
        Horizontal,
        Vertical
    }

    public enum HandleSizes
    {
        Custom,
        Tiny,
        Small,
        Medium,
        Big,
        Huge
    }

    public enum HandleShapeType
    {
        Round,
        Square,
    }
}
