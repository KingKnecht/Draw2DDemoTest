using Draw2D.Core.Geo;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public class AdjustPositionResult
    {
        public float Dx
        {
            get { return _dx; }
            set
            {
                if (_isDxLocked)
                    return;
                _dx = value;
            }
        }

        public float Dy
        {
            get { return _dy; }
            set
            {
                if (_isDyLocked)
                    return;
                _dy = value;
            }
        }

        private bool _isDxLocked;
        private bool _isDyLocked;
        private float _dx;
        private float _dy;


        public AdjustPositionResult(float dx, float dy)
        {
            Dx = dx;
            Dy = dy;
        }

        public void LockDx()
        {
            _isDxLocked = true;
        }

        public void LockDy()
        {
            _isDyLocked = true;
        }
    }
}