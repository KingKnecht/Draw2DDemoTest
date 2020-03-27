namespace Draw2D.Core.Policies.FigurePolicy
{

    public class MasterResizePolicy : SetSizePolicy
    {
        private readonly bool _forceSlaveToFollow;

        public MasterResizePolicy(bool forceSlaveToFollow = true, params Figure[] slaves)
        {
            _forceSlaveToFollow = forceSlaveToFollow;
            Slaves = slaves;
        }

        public Figure[] Slaves { get; }

        public override AdjustSizeResult AdjustSizeByDelta(Figure figure, float dTop, float dRight, float dBottom, float dLeft,
            AdjustSizeResult adjustSizeResult)
        {

            foreach (var slave in Slaves)
            {
                if (_forceSlaveToFollow)
                {
                    slave.ForceResize(dTop, dRight, dBottom, dLeft);
                }
                else
                {
                    slave.Resize(dTop, dRight, dBottom, dLeft);
                }

            }

            return adjustSizeResult;
        }
    }
}