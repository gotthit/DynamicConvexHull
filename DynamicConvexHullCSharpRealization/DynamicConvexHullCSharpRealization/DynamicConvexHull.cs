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

        public void Delete(Point pointToDelete)
        {
            // we will delete only leafs
        }

        private static void recount(RedBlackNode current)
        {
            current.MaxPoint = Utils.Max(current.Left.MaxPoint, current.Right.MaxPoint);


            throw new NotImplementedException();
        }

        private static void recountToUp(RedBlackNode current)
        {
            while (current != null)
            {
                recount(current);
                current = current.Parent;
            }
        }

        private static void insert(RedBlackNode current, Point pointToInsert)
        {
            if (current.IsLeaf)
            {
                if (pointToInsert.CompareTo(current.MaxPoint) <= 0)
                {
                    current.Left = new RedBlackNode(pointToInsert, current);
                    current.Right = new RedBlackNode(current.MaxPoint, current);
                }
                else
                {
                    current.Left = new RedBlackNode(current.MaxPoint, current);
                    current.Right = new RedBlackNode(pointToInsert, current);
                }
                recount(current);
                initialInsertRepair(current);
            }
            else
            {
                if (pointToInsert.CompareTo(current.Left.MaxPoint) <= 0)
                {
                    insert(current.Left, pointToInsert);
                }
                else
                {
                    insert(current.Right, pointToInsert);
                }
            }
        }

        private static void initialInsertRepair(RedBlackNode current)
        {
            // considering the spacific of our insertion - if current not black (in this case we have to do nothing)
            // current and current.Brother were red leafs before last insert, so we can make start already from 
            // case 3 in https://ru.wikipedia.org/wiki/%D0%9A%D1%80%D0%B0%D1%81%D0%BD%D0%BE-%D1%87%D1%91%D1%80%D0%BD%D0%BE%D0%B5_%D0%B4%D0%B5%D1%80%D0%B5%D0%B2%D0%BE 

            if (!current.IsBlack)
            {
                current.IsBlack = true;
                current.Brother.IsBlack = true;
                current.Parent.IsBlack = false;
                repair(current.Parent);
            }
        }

        private static void repair(RedBlackNode current)
        {
            recount(current);
            if (current.IsRoot)
            {
                current.IsBlack = true;
            }
            else if (!current.Parent.IsBlack)
            {
                if (current.Uncle != null && !current.Uncle.IsBlack)
                {
                    current.Parent.IsBlack = true;
                    current.Uncle.IsBlack = true;
                    current.Grandparent.IsBlack = false;

                    recount(current.Parent);

                    repair(current.Grandparent);
                }
                else
                {
                    if (current.Grandparent.Left == current.Parent && current.Parent.Right == current)
                    {
                        current.Parent.RotateLeft();
                        current = current.Left;

                        recount(current);
                    }
                    else if (current.Grandparent.Right == current.Parent && current.Parent.Left == current)
                    {
                        current.Parent.RotateRight();
                        current = current.Right;

                        recount(current);
                    }

                    current.Parent.IsBlack = true;
                    current.Grandparent.IsBlack = false;

                    recount(current.Parent);

                    if (current == current.Parent.Left)
                    {
                        current.Grandparent.RotateRight();
                        recount(current.Right);
                    }
                    else
                    {
                        current.Grandparent.RotateLeft();
                        recount(current.Left);
                    }
                    recountToUp(current);
                }
            }
        }
    }
}
