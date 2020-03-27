namespace Draw2D.Core.Handles
{
    public interface IHandle
    {
        Figure Owner { get; }
        VectorFigure HandleShape { get; set; }
        void OnDrag(Canvas canvas, float dxSum, float dySum, float dx, float dy, bool isShiftKey, bool isCtrlKey);
        bool OnDragStart(Canvas canvas, float x, float y);
        void OnDragEnd(Canvas canvas, bool isShiftKey, bool isCtrlKey);

        void Show(Canvas canvas);
        void Hide(Canvas canvas);
        void ForceTranslate(float dx, float dy);
        bool HitTest(float x, float y);
      
        void Update();

        //void ForceSetPositionOfCenter(float x, float y);
    }
}