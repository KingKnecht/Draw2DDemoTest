using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Draw2D.Core
{
    public class Selection
    {
        private readonly Canvas _canvas;
        private Figure _primary;
        private readonly List<Figure> _all = new List<Figure>();

        public Selection(Canvas canvas)
        {
            _canvas = canvas;
        }

        public Figure Primary
        {
            get { return _primary; }
            set
            {
                _primary = value;
                Add(_primary);
            }
        }

        public List<Figure> All => _all;

        public void Add(Figure figure)
        {
            if (figure != null && !_all.Contains(figure))
            {
                _all.Add(figure);
                Primary = figure;
                _canvas.OnSelectionChanged(figure);
            }
        }

        public Selection Remove(Figure figure)
        {
            var isRemoved =_all.Remove(figure);

            if (Primary == figure)
            {
                Primary = _all.LastOrDefault();
            }

            if (isRemoved)
            {
                _canvas.OnSelectionChanged(figure);
            }

            return this;
        }

        public bool Contains(Figure figure, bool checkDescendant = true)
        {
            if (checkDescendant)
            {
                if (_all.Any(figureToCheck => figureToCheck==figure || figureToCheck.Contains(figure)))
                {
                    return true;
                }    
            }
            return _all.Contains(figure);
        }

        public void Clear()
        {
            var all = All.ToList();
            foreach (var figure in all)
            {
                Remove(figure);
            }
        }
    }
}
