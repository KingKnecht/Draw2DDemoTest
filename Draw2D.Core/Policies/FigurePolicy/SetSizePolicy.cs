namespace Draw2D.Core.Policies.FigurePolicy
{
    public class SetSizePolicy : PolicyBase
    {
        public virtual AdjustSizeResult AdjustSizeByDelta(Figure figure, float dTop, float dRight, float dBottom, float dLeft, AdjustSizeResult adjustSizeResult)
        {
            return adjustSizeResult;
        }
    }

    public class InformSizeChangedPolicy : PolicyBase
    {
        private readonly Figure[] _clients;
        
        public InformSizeChangedPolicy(params Figure[] clients)
        {
            _clients = clients;
        }

        public virtual void InformSizeChanged(Figure changedFigure, float dTop, float dRight, float dBottom, float dLeft)
        {
            foreach (var client in _clients)
            {
                client.InformSizeChanged(changedFigure, dTop, dRight, dBottom, dLeft);
            }
            
        }
    }




}