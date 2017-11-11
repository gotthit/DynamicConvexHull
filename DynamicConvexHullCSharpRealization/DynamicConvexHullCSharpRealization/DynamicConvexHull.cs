using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class DynamicConvexHull
    {
        private class RedBlackNode
        {
            public RedBlackNode Left;
            public RedBlackNode Right;
            public RedBlackNode Parent;

            public bool IsBlack;

            public Point MaxPoint;
            public Bridge Bridge;

            public Treap<Point> ConvexHull;

            public int leftSubHullSize;

            public bool IsLeaf { get { return Left == null && Right == null; } }
            public bool IsRoot { get { return Parent == null; } }

            public RedBlackNode Grandparent { get { return Parent?.Parent; } }
            public RedBlackNode Uncle
            {
                get
                {
                    if (Grandparent == null)
                    {
                        return null;
                    }
                    if (Grandparent.Left == Parent)
                    {
                        return Grandparent.Right;
                    }
                    return Grandparent.Left;
                }
            }

            private RedBlackNode(Point newPoint = null, RedBlackNode newLeft = null, RedBlackNode newRight = null, RedBlackNode newParent = null)
            {
                Left = newLeft;
                Right = newRight;
                Parent = newParent;

                MaxPoint = newPoint;
                Bridge = null;

                IsBlack = IsRoot;

                ConvexHull = null;
                leftSubHullSize = 0;
            }

            public RedBlackNode(Point newPoint, RedBlackNode newParent = null) : this(newPoint, null, null, newParent)
            {
                ConvexHull = new Treap<Point>(newPoint);
            }

            public RedBlackNode(RedBlackNode newLeft, RedBlackNode newRight, RedBlackNode newParent = null) : this(null, newLeft, newRight, newParent)
            {
                leftSubHullSize = Treap<Point>.GetSize(newLeft?.ConvexHull);
                ConvexHull = Treap<Point>.Merge(newLeft?.ConvexHull, newRight?.ConvexHull);
            }

            private void rotateLeft()
            {
                if (Right != null)
                {
                    RedBlackNode rightSon = Right;
                    rightSon.Parent = Parent;

                    if (Parent != null)
                    {
                        if (Parent.Left == this)
                        {
                            Parent.Left = rightSon;
                        }
                        else
                        {
                            Parent.Right = rightSon;
                        }
                    }
                    Right = rightSon.Left;
                    if (rightSon.Left != null)
                    {
                        rightSon.Left.Parent = this;
                    }
                    rightSon.Left = this;
                    Parent = rightSon;
                }
            }

            private void rotateRight()
            {
                if (Left != null)
                {
                    RedBlackNode leftSon = Left;
                    leftSon.Parent = Parent;

                    if (Parent != null)
                    {
                        if (Parent.Left == this)
                        {
                            Parent.Left = leftSon;
                        }
                        else
                        {
                            Parent.Right = leftSon;
                        }
                    }
                    Left = leftSon.Right;
                    if (leftSon.Right != null)
                    {
                        leftSon.Right.Parent = this;
                    }
                    leftSon.Right = this;
                    Parent = leftSon;
                }
            }
        }

        private RedBlackNode leftHullRoot;
        private RedBlackNode rightHullRoot;

        public void Insert(Point pointToInsert)
        {
            if (leftHullRoot == null)
            {
                leftHullRoot = new RedBlackNode(pointToInsert);
            }
            else
            {
                insert(leftHullRoot, pointToInsert);
            }
        }

        private void insert(RedBlackNode current, Point pointToInsert)
        {
            if (current.IsLeaf)
            {
                
            }
        }

        private void repair(RedBlackNode current)
        {

        }
    }
}
