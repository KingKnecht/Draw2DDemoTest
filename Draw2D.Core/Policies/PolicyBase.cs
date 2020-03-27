using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draw2D.Core.Geo;

namespace Draw2D.Core.Policies
{
    public abstract class PolicyBase
    {
        public bool Enabled { get; set; } = true;
        internal virtual void OnInstall(Figure hostFigure)
        {

        }

        internal virtual void OnUninstall(Figure hostFigure)
        {

        }

        internal virtual void OnInstall(Canvas hostCanvas)
        {

        }

        internal virtual void OnUninstall(Canvas hostCanvas)
        {

        }
    }

    public interface ILink
    {
        IEnumerable<Figure> GetLinkedFigures();
    }

    public interface IConstraint
    {
        
    }

    public class ConstraintPoint
    {
        private Point _point;

        public Point Point
        {
            get { return _point; }
            set
            {
                if (_point == value)
                    return;

                _point = value;
                OnPointChanged(_point);
            }
        }

        public event EventHandler<Point> PointChanged;
        public event EventHandler ConstraintPointRemoved;

        protected virtual void OnPointChanged(Point e)
        {
            PointChanged?.Invoke(this, e);
        }
        
        public ConstraintPoint(Point point)
        {
            Point = point;
        }

        public void Delete()
        {
            OnConstraintPointRemoved();
        }

        protected virtual void OnConstraintPointRemoved()
        {
            ConstraintPointRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}
