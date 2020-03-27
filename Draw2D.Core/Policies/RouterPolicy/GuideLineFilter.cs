using System.Collections.Generic;
using System.Linq;
using Draw2D.Core.Geo;

namespace Draw2D.Core.Policies.RouterPolicy
{
    internal class GuideLineFilter
    {
        private readonly List<GuideLineFilterItem> _filter = new List<GuideLineFilterItem>();

        public List<GuideLineFilterItem> Filter => _filter;

        public void Add(Point from, Point to)
        {
            var newItem = new GuideLineFilterItem(from, to);

            var sameAlignment = _filter.FirstOrDefault(i => i.To == newItem.To && i.Direction == newItem.Direction);
            if (sameAlignment != null)
            {
                if (sameAlignment.SquareLength > newItem.SquareLength)
                {
                    _filter.Remove(sameAlignment);
                    _filter.Add(newItem);
                }
            }
            else
            {
                _filter.Add(newItem);
            }
        }
    }
}