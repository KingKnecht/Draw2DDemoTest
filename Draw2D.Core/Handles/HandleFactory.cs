using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Draw2D.Core.Handles
{
    internal class Freeable
    {
        public Freeable(Geometry geometry)
        {
            Geometry = geometry;
        }

        public bool IsFree { get; set; }
        public Geometry Geometry { get; private set; }
    }

    public interface IHandleFactory
    {
        Geometry GetHandle();
        void Release(Geometry geometry);
    }

    public class HandleFactory : IHandleFactory
    {
        private readonly List<Freeable> _handles = new List<Freeable>();

        public Geometry GetHandle()
        {
            var freeableGeometry = _handles.FirstOrDefault(f => f.IsFree);
            if (freeableGeometry == null)
            {
                var geometry = new EllipseGeometry(new System.Windows.Point(0, 0), 10, 10);
                geometry.Freeze();

                freeableGeometry = new Freeable(geometry);
            }

            freeableGeometry.IsFree = false;
            _handles.Add(freeableGeometry);

            return freeableGeometry.Geometry;
        }

        public void Release(Geometry geometry)
        {
            var freeable = _handles.Find(h => h.Geometry.Equals(geometry));
            if (freeable != null)
            {
                freeable.IsFree = true;
            }
        }
    }
}