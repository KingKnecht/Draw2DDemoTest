using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Draw2D.Core.Geo;

namespace Draw2D.Core.Policies.FigurePolicy
{
    public class MasterSlaveDragDropPolicy : DragDropEditPolicy, ILink
    {
        private readonly List<Figure> _slaves;
     
        public MasterSlaveDragDropPolicy( params Figure[] slaves)
        {
            _slaves = new List<Figure>(slaves);
        }
        

        public override AdjustPositionResult AdjustPositionByDelta(Figure master, float dx, float dy, AdjustPositionResult adjustmentResult)
        {
            _slaves.ForEach(f => f.ForceTranslate(adjustmentResult.Dx, adjustmentResult.Dy));

            return adjustmentResult;
        }


        public IEnumerable<Figure> GetLinkedFigures()
        {
            return
                _slaves.Concat(
                    _slaves.SelectMany(f => f.Policies).OfType<ILink>()
                        .SelectMany(l => l.GetLinkedFigures()));
        }
    }
}