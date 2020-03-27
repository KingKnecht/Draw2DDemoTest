namespace Draw2D.Core.Policies.CanvasPolicy
{
    public interface IMouseAware
    {
        void OnClick(Figure figure, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey);
        void OnDoubleClick(Figure figure, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey);
        void OnMouseMove(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey);
        void OnMouseLeftDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey);
        void OnMouseLeftDoubleClick(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey);
        void OnMouseLeftUp(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey);
        void OnMouseRightDown(Canvas canvas, float mouseX, float mouseY, bool isShiftKey, bool isCtrlKey);
    }

    public interface IDragAware
    {
        void OnMouseDrag(Canvas canvas, float dxSum, float dySum, float dx, float dy, bool isShiftKey, bool isCtrlKey);
        void OnDragStart(Canvas canvas,float startPosX, float startPosY, float dx, float dy, bool isShiftKey, bool isCtrlKey);
        void OnDragEnd(Canvas canvas, bool isShiftKey, bool isCtrlKey);

    }
}
