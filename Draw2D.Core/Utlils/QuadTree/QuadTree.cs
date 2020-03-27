﻿using System.Collections.Generic;
using Draw2D.Core.Geo;

namespace Draw2D.Core.Utlils.QuadTree
{
    //Comes from: http://www.codeproject.com/Articles/30535/A-Simple-QuadTree-Implementation-in-C
    /// <summary>
    /// A Quadtree is a structure designed to partition space so
    /// that it's faster to find out what is inside or outside a given 
    /// area. See http://en.wikipedia.org/wiki/Quadtree
    /// This QuadTree contains items that have an area (RectangleF)
    /// it will store a reference to the item in the quad 
    /// that is just big enough to hold it. Each quad has a bucket that 
    /// contain multiple items.
    /// </summary>
    public class QuadTree<T> where T : Figure
    {
        /// <summary>
        /// The root QuadTreeNode
        /// </summary>
        QuadTreeNode<T> m_root;

        /// <summary>
        /// The bounds of this QuadTree
        /// </summary>
        Rectangle m_rectangle;

        /// <summary>
        /// An delegate that performs an action on a QuadTreeNode
        /// </summary>
        /// <param name="obj"></param>
        public delegate void QTAction(QuadTreeNode<T> obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rectangle"></param>
        public QuadTree(Rectangle rectangle)
        {
            m_rectangle = rectangle;
            m_root = new QuadTreeNode<T>(m_rectangle);
        }

        /// <summary>
        /// Get the count of items in the QuadTree
        /// </summary>
        public int Count { get { return m_root.Count; } }

        /// <summary>
        /// Insert the feature into the QuadTree
        /// </summary>
        /// <param name="item"></param>
        public void Insert(T item)
        {
            m_root.Insert(item);
        }

        /// <summary>
        /// Query the QuadTree, returning the items that are in the given area
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public List<T> Query(Rectangle area)
        {
            return m_root.Query(area);
        }
        
        /// <summary>
        /// Do the specified action for each item in the quadtree
        /// </summary>
        /// <param name="action"></param>
        public void ForEach(QTAction action)
        {
            m_root.ForEach(action);
        }

    }

}
