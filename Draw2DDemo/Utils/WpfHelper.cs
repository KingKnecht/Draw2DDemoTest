using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Draw2DDemo.Utils
{
    public static class WpfHelper
    {
        public static IEnumerable<DependencyObject> GetVisualChildsRecursive(this DependencyObject parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = VisualTreeHelper.GetChild(parent, i);

                yield return v;

                foreach (var c in GetVisualChildsRecursive(v))
                    yield return c;
            }
        }
    }
}
