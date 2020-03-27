namespace Draw2D.Core.Policies.FigurePolicy
{
    public class AdjustSizeResult
    {
        private float _deltaTop;
        private float _deltaRight;
        private float _deltaBottom;
        private float _deltaLeft;

        public AdjustSizeResult(float deltaTop, float deltaRight, float deltaBottom, float deltaLeft)
        {
            DeltaTop = deltaTop;
            DeltaRight = deltaRight;
            DeltaBottom = deltaBottom;
            DeltaLeft = deltaLeft;
        }


        public float DeltaTop
        {
            get { return _deltaTop; }
            set
            {
                if(IsLockedDeltaTop)
                    return;
                _deltaTop = value;
            }
        }

        public float DeltaRight
        {
            get { return _deltaRight; }
            set
            {
                if (IsLockedDeltaRight)
                    return;
                _deltaRight = value;
            }
        }

        public float DeltaBottom
        {
            get { return _deltaBottom; }
            set
            {
                if (IsLockedDeltaBottom)
                    return;
                _deltaBottom = value;
            }
        }

        public float DeltaLeft
        {
            get { return _deltaLeft; }
            set
            {
                if (IsLockedDeltaLeft)
                    return;
                _deltaLeft = value;
            }
        }

        public bool IsLockedDeltaTop { get;private set; }
        public bool IsLockedDeltaRight { get; private set; }
        public bool IsLockedDeltaBottom { get; private set; }
        public bool IsLockedDeltaLeft { get; private set; }

        public void LockDeltaTop()
        {
            IsLockedDeltaTop = true;
        }

        public void LockDeltaRight()
        {
            IsLockedDeltaRight = true;
        }

        public void LockDeltaBottom()
        {
            IsLockedDeltaBottom = true;
        }

        public void LockDeltaLeft()
        {
            IsLockedDeltaLeft = true;
        }

    }
}