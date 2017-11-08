using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class ConvexHull
    {
        private class RedBlackNode
        {
            public RedBlackNode Left;
            public RedBlackNode Right;
            public RedBlackNode Parent;

            public bool IsBlack;

            public Point MinPoint;
            public Point MaxPoint;
            public Bridge Bridge;

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

            public RedBlackNode(Point newPoint = null, RedBlackNode newParent = null, RedBlackNode newLeft = null, RedBlackNode newRight = null)
            {
                Left = newLeft;
                Right = newRight;
                Parent = newParent;

                MinPoint = newPoint;
                Bridge = null;

                IsBlack = IsRoot;
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

        private RedBlackNode root;

        public void Insert(Point pointToInsert)
        {
            if (root == null)
            {
                root = new RedBlackNode(pointToInsert);
            }
            else
            {
                insert(root, pointToInsert);
            }
        }

        private void insert(RedBlackNode current, Point pointToInsert)
        {
            
        }

        private void repair(RedBlackNode current)
        {

        }
    }
}
