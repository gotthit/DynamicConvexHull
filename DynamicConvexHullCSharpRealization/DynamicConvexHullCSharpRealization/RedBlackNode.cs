﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    partial class DynamicConvexHull
    {
        private class RedBlackNode
        {
            public enum Color
            {
                Red,
                Black
            }

            private int leftLowerSubHullSize;
            private Treap<Point> leftConvexHull;

            private int rightLowerSubHullSize { get; set; }
            private Treap<Point> rightConvexHull;

            public RedBlackNode Left;
            public RedBlackNode Right;
            public RedBlackNode Parent;

            public Color NodeColor;

            public Point MaxPoint;

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
            public RedBlackNode Brother
            {
                get
                {
                    if (Parent == null)
                    {
                        return null;
                    }
                    if (Parent.Left == this)
                    {
                        return Parent.Right;
                    }
                    return Parent.Left;
                }
            }

            public int GeLowerSubHullSize(bool isLeftHalf)
            {
                return isLeftHalf ? leftLowerSubHullSize : rightLowerSubHullSize;
            }

            public void SetLowerSubHullSize(bool isLeftHalf, int value)
            {
                if (isLeftHalf)
                {
                    leftLowerSubHullSize = value;
                }
                else
                {
                    rightLowerSubHullSize = value;
                }
            }

            public Treap<Point> GetConvexHull(bool isLeftHalf)
            {
                return isLeftHalf ? leftConvexHull : rightConvexHull;
            }

            public void SetConvexHull(bool isLeftHalf, Treap<Point> value)
            {
                if (isLeftHalf)
                {
                    leftConvexHull = value;
                }
                else
                {
                    rightConvexHull = value;
                }
            }

            private RedBlackNode(Point newPoint = null, RedBlackNode newLeft = null, RedBlackNode newRight = null, RedBlackNode newParent = null)
            {
                Left = newLeft;
                Right = newRight;
                Parent = newParent;

                MaxPoint = newPoint;

                NodeColor = IsRoot ? Color.Black : Color.Red;

                leftConvexHull = null;
                leftLowerSubHullSize = 0;

                rightConvexHull = null;
                rightLowerSubHullSize = 0;
            }

            public RedBlackNode(Point newPoint, RedBlackNode newParent = null) : this(newPoint, null, null, newParent)
            {
                leftConvexHull = new Treap<Point>(newPoint);
                rightConvexHull = new Treap<Point>(newPoint);
            }

            public void RotateLeft()
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

            public void RotateRight()
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
    }
}
