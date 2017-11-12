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
                leftHullRoot = insert(leftHullRoot, pointToInsert);
            }

            if (rightHullRoot == null)
            {
                rightHullRoot = new RedBlackNode(pointToInsert);
            }
            else
            {
                rightHullRoot = insert(rightHullRoot, pointToInsert);
            }
        }

        public void Delete(Point pointToDelete)
        {
            if (leftHullRoot != null)
            {
                if (leftHullRoot.IsLeaf && leftHullRoot.MaxPoint.CompareTo(pointToDelete) == 0)
                {
                    leftHullRoot = null;
                }
                else
                {
                    RedBlackNode newRoot = delete(leftHullRoot, pointToDelete);
                    // null means we deleted nothing
                    if (newRoot != null)
                    {
                        leftHullRoot = newRoot;
                    }
                }
            }

            if (rightHullRoot != null)
            {
                if (rightHullRoot.IsLeaf && rightHullRoot.MaxPoint.CompareTo(pointToDelete) == 0)
                {
                    rightHullRoot = null;
                }
                else
                {
                    RedBlackNode newRoot = delete(rightHullRoot, pointToDelete);
                    // null means we deleted nothing
                    if (newRoot != null)
                    {
                        rightHullRoot = newRoot;
                    }
                }
            }
        }

        private static void recount(RedBlackNode current)
        {
            current.MaxPoint = Utils.Max(current.Left.MaxPoint, current.Right.MaxPoint);


            throw new NotImplementedException();
        }

        private static RedBlackNode recountToUp(RedBlackNode current)
        {
            while (!current.IsRoot)
            {
                recount(current);
                current = current.Parent;
            }
            recount(current);
            return current;
        }

        private static RedBlackNode insert(RedBlackNode current, Point pointToInsert)
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
                return initialInsertRepair(current);
            }
            else
            {
                if (pointToInsert.CompareTo(current.Left.MaxPoint) <= 0)
                {
                    return insert(current.Left, pointToInsert);
                }
                else
                {
                    return insert(current.Right, pointToInsert);
                }
            }
        }

        private static RedBlackNode initialInsertRepair(RedBlackNode current)
        {
            // considering the spacific of our insertion - if current not black (in this case we have to do nothing)
            // current and current.Brother were red leafs before last insert, so we can make start already from 
            // case 3 in https://ru.wikipedia.org/wiki/%D0%9A%D1%80%D0%B0%D1%81%D0%BD%D0%BE-%D1%87%D1%91%D1%80%D0%BD%D0%BE%D0%B5_%D0%B4%D0%B5%D1%80%D0%B5%D0%B2%D0%BE 

            if (current.IsBlack == false)
            {
                current.IsBlack = true;
                current.Brother.IsBlack = true;
                current.Parent.IsBlack = false;
                return repairAfterInsert(current.Parent);
            }
            else
            {
                return recountToUp(current);
            }
        }

        private static RedBlackNode repairAfterInsert(RedBlackNode current)
        {
            recount(current);
            if (current.IsRoot)
            {
                current.IsBlack = true;
                return current;
            }
            else if (current.Parent.IsBlack == false)
            {
                if (current.Uncle != null && current.Uncle.IsBlack == false)
                {
                    current.Parent.IsBlack = true;
                    current.Uncle.IsBlack = true;
                    current.Grandparent.IsBlack = false;

                    recount(current.Parent);

                    return repairAfterInsert(current.Grandparent);
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
                }
            }
            return recountToUp(current);
        }

        private static RedBlackNode delete(RedBlackNode current, Point pointToDelete)
        {
            // we will delete only leafs
            // every node have two children

            if (current.IsLeaf && current.MaxPoint.CompareTo(pointToDelete) == 0)
            {
                if (current.Grandparent != null)
                {
                    if (current.Grandparent.Left == current.Parent)
                    {
                        current.Grandparent.Left = current.Brother;
                    }
                    else
                    {
                        current.Grandparent.Right = current.Brother;
                    }
                }
                current.Brother.Parent = current.Grandparent;

                if (current.Brother.IsBlack == false || current.Parent.IsBlack == false)
                {
                    current.Brother.IsBlack = true;

                    return recountToUp(current.Brother);
                }
                else
                {
                    return repairAfterDelete(current.Brother);
                }
            }
            else if (!current.IsLeaf)
            {
                if (pointToDelete.CompareTo(current.Left.MaxPoint) <= 0)
                {
                    return delete(current.Left, pointToDelete);
                }
                else
                {
                    return delete(current.Right, pointToDelete);
                }
            }
            // it means we deleted nothing
            return null;
        }

        private static RedBlackNode repairAfterDelete(RedBlackNode current)
        {
            recount(current);
            if (current.IsRoot)
            {
                return current;
            }
            else
            {
                if (current.Brother.IsBlack == false)
                {
                    current.Parent.IsBlack = false;
                    current.Brother.IsBlack = true;
                    if (current.Parent.Left == current)
                    {
                        current.Parent.RotateLeft();
                    }
                    else
                    {
                        current.Parent.RotateRight();
                    }
                }

                if (current.Parent.IsBlack && current.Brother.IsBlack && current.Brother.Left.IsBlack && current.Brother.Right.IsBlack)
                {
                    current.Brother.IsBlack = false;
                    return repairAfterDelete(current.Parent);
                }
                else if (current.Parent.IsBlack == false && current.Brother.IsBlack && current.Brother.Left.IsBlack && current.Brother.Right.IsBlack)
                {
                    current.Brother.IsBlack = false;
                    current.Parent.IsBlack = true;
                    return recountToUp(current.Parent);
                }
                else // if (current.Brother.IsBlack) always true
                {
                    if (current.Parent.Left == current && current.Brother.Left.IsBlack == false && current.Brother.Right.IsBlack)
                    {
                        current.Brother.IsBlack = false;
                        current.Brother.Left.IsBlack = true;
                        current.Brother.RotateRight();

                        recount(current.Brother.Right);
                        recount(current.Brother);
                    }
                    else if (current.Parent.Right == current && current.Brother.Left.IsBlack && current.Brother.Right.IsBlack == false)
                    {
                        current.Brother.IsBlack = false;
                        current.Brother.Right.IsBlack = true;
                        current.Brother.RotateLeft();

                        recount(current.Brother.Left);
                        recount(current.Brother);
                    }

                    current.Brother.IsBlack = current.Parent.IsBlack;
                    current.Parent.IsBlack = true;

                    if (current == current.Parent.Left)
                    {
                        current.Brother.Right.IsBlack = true;
                        current.Parent.RotateRight();
                    }
                    else
                    {
                        current.Brother.Left.IsBlack = true;
                        current.Parent.RotateLeft();
                    }
                    return recountToUp(current.Parent);
                }
            }
        }
    }
}
