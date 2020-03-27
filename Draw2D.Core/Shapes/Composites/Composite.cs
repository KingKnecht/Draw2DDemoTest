using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Draw2D.Core.Shapes.Basic;
using Draw2D.Core.Utlils;

namespace Draw2D.Core.Shapes.Composites
{
    public  class Composite : Rectangle
    {
        private readonly List<Figure> _members = new List<Figure>();

        public Composite(float x, float y, float width, float height) : base(x, y, width, height)
        {
            
        }

        public virtual Composite AddMember(Figure figure)
        {
            figure.BringToFront();
            _members.Add(figure);
            return this;
        }

        public override Figure Select(bool showHandles = true)
        {
            base.Select(showHandles);

            SendToBack();
            foreach (var member in _members)
            {
                member.BringToFront();
            }

            return this;
        }

        public override void OnDrag(Canvas canvas, float dxSum, float dySum, float dx, float dy, bool isShiftKey, bool isCtrlKey)
        {
            base.OnDrag(canvas, dxSum, dySum, dx, dy, isShiftKey, isCtrlKey);

            if (HittedResizeHandle == null)
            {
                foreach (var member in _members)
                {
                    member.OnDrag(canvas, dxSum, dySum, dx, dy, isShiftKey, isCtrlKey);
                }
            }
        }
    }
}
