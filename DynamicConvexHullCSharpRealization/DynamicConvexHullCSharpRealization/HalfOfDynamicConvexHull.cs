﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    partial class DynamicConvexHull
    {
        private partial class HalfOfDynamicConvexHull
        {
            private bool IsLeftHalf;
            private RedBlackNode hullRoot;

            public HalfOfDynamicConvexHull(bool newIsLeftHalf)
            {
                IsLeftHalf = newIsLeftHalf;
            }

            public void Insert(Point pointToInsert)
            {
                if (hullRoot == null)
                {
                    hullRoot = new RedBlackNode(pointToInsert);
                }
                else
                {
                    hullRoot = insert(hullRoot, pointToInsert);
                }
            }

            public void Delete(Point pointToDelete)
            {
                if (hullRoot != null)
                {
                    if (hullRoot.IsLeaf && hullRoot.MaxPoint.CompareTo(pointToDelete) == 0)
                    {
                        hullRoot = null;
                    }
                    else
                    {
                        RedBlackNode newRoot = delete(hullRoot, pointToDelete);
                        hullRoot = newRoot;
                    }
                }
            }

            public void GetHalfHull()
            {
                hullRoot.ConvexHull.GetArray();
            }

            #region hull_recount_operations

            private PointPosition determinePosition(Point beginVector, Point endVector, Point toDetermine)
            {
                // propably we change answer for right hull to make it esier to analize in other methods
                //  PointPosition.Right -> PointPosition.Left and vise versa

                if (toDetermine == null)
                {
                    return PointPosition.Right;
                }
                else
                {
                    PointPosition answer = Utils.DeterminePosition(beginVector, endVector, toDetermine);
                    if (answer != PointPosition.On)
                    {
                        if (!IsLeftHalf)
                        {
                            if (answer == PointPosition.Left)
                            {
                                return PointPosition.Right;
                            }
                            else
                            {
                                return PointPosition.Left;
                            }
                        }
                        else
                        {
                            return answer;
                        }
                    }
                    else
                    {
                        return PointPosition.Right;
                    }
                }
            }

            private double getXbyY(Point first, Point second, double yCoord)
            {
                return (yCoord - first.Y) * (second.X - first.X) / (second.Y - first.Y) + first.X;
            }

            private bool isIntersectLoverY(Point firstLeft, Point secondLeft, Point firstRight, Point secondRight, double yCoord)
            {
                double leftXbyY = getXbyY(firstLeft, secondLeft, yCoord);
                double rightXbyY = getXbyY(firstRight, secondRight, yCoord);
                if (IsLeftHalf)
                {
                    return leftXbyY < rightXbyY;
                }
                else
                {
                    return rightXbyY < leftXbyY;
                }
            }

            private void goLeft(ref Treap<Point> current, ref Point currentPoint, ref Point minPoint, ref Point maxPoint, 
                                ref int toCut, ref int minToCut, ref int maxToCut)
            {
                if (current?.Left != null)
                {
                    maxPoint = current.Key;
                    maxToCut = toCut;

                    toCut = toCut - Treap<Point>.GetSize(current.Left.Right) - 1;
                    current = current.Left;
                    currentPoint = current.Key;
                }
                else if (minPoint != null)
                {
                    maxPoint = currentPoint;
                    maxToCut = toCut;

                    currentPoint = minPoint;
                    toCut = minToCut;

                    minPoint = null;
                    minToCut = 0;

                    current = null;
                }
            }

            private void goRight(ref Treap<Point> current, ref Point currentPoint, ref Point minPoint, ref Point maxPoint,
                                 ref int toCut, ref int minToCut, ref int maxToCut)
            {
                if (current?.Right != null)
                {
                    minPoint = current.Key;
                    minToCut = toCut;

                    toCut = toCut + Treap<Point>.GetSize(current.Right.Left) + 1;
                    current = current.Right;
                    currentPoint = current.Key;
                }
                else if (maxPoint != null)
                {
                    minPoint = currentPoint;
                    minToCut = toCut;

                    currentPoint = maxPoint;
                    toCut = maxToCut;

                    maxPoint = null;
                    maxToCut = int.MaxValue - 100;

                    current = null;
                }
            }

            private bool makeDecisionAboutRotation(ref Treap<Point> leftTop, ref Point leftTopPoint, Point leftPrevPoint, Point leftNextPoint, 
                                                   ref Point leftMinPoint, ref Point leftMaxPoint, 
                                                   ref int toCutFromLeftHull, ref int minPointCutFromLeft, ref int maxPointCutFromLeft,
                                                   ref Treap<Point> rightTop, ref Point rightTopPoint, Point rightPrevPoint, Point rightNextPoint,
                                                   ref Point rightMinPoint, ref Point rightMaxPoint,
                                                   ref int toLeaveInRightHull, ref int minPointLeaveInRight, ref int maxPointLeaveInRight,
                                                   double mediumY)
            {
                PointPosition leftPrevPos = determinePosition(leftTopPoint, rightTopPoint, leftPrevPoint);
                PointPosition leftNextPos = determinePosition(leftTopPoint, rightTopPoint, leftNextPoint);

                PointPosition rightPrevPos = determinePosition(leftTopPoint, rightTopPoint, rightPrevPoint);
                PointPosition rightNextPos = determinePosition(leftTopPoint, rightTopPoint, rightNextPoint);

                if (leftPrevPos == PointPosition.Right && leftNextPos == PointPosition.Right &&
                    rightPrevPos == PointPosition.Right && rightNextPos == PointPosition.Right) // A - found
                {
                    return true; // answer found
                }
                else if (leftPrevPos == PointPosition.Left && leftNextPos == PointPosition.Right &&
                         rightPrevPos == PointPosition.Right && rightNextPos == PointPosition.Right) // B
                {
                    goLeft(ref leftTop, ref leftTopPoint, ref leftMinPoint, ref leftMaxPoint,
                           ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft);

                    //goRight(ref rightTop, ref rightTopPoint, ref rightMinPoint, ref rightMaxPoint,
                    //       ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight);
                }
                else if (leftPrevPos == PointPosition.Right && leftNextPos == PointPosition.Left &&
                         rightPrevPos == PointPosition.Right && rightNextPos == PointPosition.Right) // C
                {
                    goRight(ref leftTop, ref leftTopPoint, ref leftMinPoint, ref leftMaxPoint,
                           ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft);

                    //goRight(ref rightTop, ref rightTopPoint, ref rightMinPoint, ref rightMaxPoint,
                    //       ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight);
                }
                else if (leftPrevPos == PointPosition.Right && leftNextPos == PointPosition.Right &&
                         rightPrevPos == PointPosition.Right && rightNextPos == PointPosition.Left) // D
                {
                    //goLeft(ref leftTop, ref leftTopPoint, ref leftMinPoint, ref leftMaxPoint,
                    //       ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft);

                    goRight(ref rightTop, ref rightTopPoint, ref rightMinPoint, ref rightMaxPoint,
                           ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight);
                }
                else if (leftPrevPos == PointPosition.Right && leftNextPos == PointPosition.Right &&
                         rightPrevPos == PointPosition.Left && rightNextPos == PointPosition.Right) // E
                {
                    //goLeft(ref leftTop, ref leftTopPoint, ref leftMinPoint, ref leftMaxPoint,
                    //       ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft);

                    goLeft(ref rightTop, ref rightTopPoint, ref rightMinPoint, ref rightMaxPoint,
                           ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight);
                }
                else if (leftPrevPos == PointPosition.Left && leftNextPos == PointPosition.Right &&
                         rightPrevPos == PointPosition.Right && rightNextPos == PointPosition.Left) // F
                {
                    goLeft(ref leftTop, ref leftTopPoint, ref leftMinPoint, ref leftMaxPoint,
                           ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft);

                    goRight(ref rightTop, ref rightTopPoint, ref rightMinPoint, ref rightMaxPoint,
                           ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight);
                }
                else if (leftPrevPos == PointPosition.Left && leftNextPos == PointPosition.Right &&
                         rightPrevPos == PointPosition.Left && rightNextPos == PointPosition.Right) // G
                {
                    goLeft(ref leftTop, ref leftTopPoint, ref leftMinPoint, ref leftMaxPoint,
                           ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft);
                }
                else if (leftPrevPos == PointPosition.Right && leftNextPos == PointPosition.Left &&
                         rightPrevPos == PointPosition.Right && rightNextPos == PointPosition.Left) // H
                {
                    goRight(ref rightTop, ref rightTopPoint, ref rightMinPoint, ref rightMaxPoint,
                           ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight);
                }
                else if (leftPrevPos == PointPosition.Right && leftNextPos == PointPosition.Left &&
                         rightPrevPos == PointPosition.Left && rightNextPos == PointPosition.Right) // I - the hardest
                {
                    if (isIntersectLoverY(leftTopPoint, leftNextPoint, rightTopPoint, rightPrevPoint, mediumY))
                    {
                        goRight(ref leftTop, ref leftTopPoint, ref leftMinPoint, ref leftMaxPoint,
                           ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft);
                    }
                    else
                    {
                        goLeft(ref rightTop, ref rightTopPoint, ref rightMinPoint, ref rightMaxPoint,
                           ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight);
                    }
                }
                else
                {
                    throw new Exception("something wrong");
                }

                return false; // answer not found
            }

            private void recount(RedBlackNode current)
            {
                if (current != null && current.Left != null && current.Right != null && current.ConvexHull == null)
                {
                    // left is lover hull and right is upper hull

                    current.MaxPoint = Utils.Max(current.Left.MaxPoint, current.Right.MaxPoint);

                    Treap<Point> leftTop = current.Left.ConvexHull;
                    Treap<Point> rightTop = current.Right.ConvexHull;
                    Point leftTopPoint = leftTop.Key;
                    Point rightTopPoint = rightTop.Key;

                    Point leftMinPoint = null;
                    Point leftMaxPoint = null;
                    Point rightMinPoint = null;
                    Point rightMaxPoint = null;

                    double mediumY = (leftTop.MaxElement.Y + rightTop.MinElement.Y) / 2;

                    int toCutFromLeftHull = Treap<Point>.GetSize(leftTop?.Left) + 1;
                    int toLeaveInRightHull = Treap<Point>.GetSize(rightTop?.Left) + 1;

                    int minPointCutFromLeft = 0;
                    int maxPointCutFromLeft = leftTop.Size;

                    int minPointLeaveInRight = 0;
                    int maxPointLeaveInRight = rightTop.Size;

                    bool bridgeFound = false;
                    while (!bridgeFound)
                    {
                        Point leftPrevPoint = leftTop?.Left != null ? leftTop.Left.MaxElement : leftMinPoint;
                        Point leftNextPoint = leftTop?.Right != null ? leftTop.Right.MinElement : leftMaxPoint;

                        Point rightPrevPoint = rightTop?.Left != null ? rightTop.Left.MaxElement : rightMinPoint;
                        Point rightNextPoint = rightTop?.Right != null ? rightTop.Right.MinElement : rightMaxPoint;

                        bridgeFound = makeDecisionAboutRotation(ref leftTop, ref leftTopPoint, leftPrevPoint, leftNextPoint,
                                                   ref leftMinPoint, ref leftMaxPoint,
                                                   ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft,
                                                   ref rightTop, ref rightTopPoint, rightPrevPoint, rightNextPoint,
                                                   ref rightMinPoint, ref rightMaxPoint,
                                                   ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight,
                                                   mediumY);
                    }
                    // we want to have top point in right treap after split
                    --toLeaveInRightHull;

                    Treap<Point> leftLeftHalf;
                    Treap<Point> leftRightHalf;
                    Treap<Point> rightLeftHalf;
                    Treap<Point> rightRightHalf;

                    if (leftTopPoint.Y == rightTopPoint.Y)
                    {
                        if ((leftTopPoint.X < rightTopPoint.X && IsLeftHalf) ||
                            (leftTopPoint.X > rightTopPoint.X && !IsLeftHalf))
                        {
                            ++toLeaveInRightHull;
                        }
                        else if ((leftTopPoint.X > rightTopPoint.X && IsLeftHalf) ||
                                 (leftTopPoint.X < rightTopPoint.X && !IsLeftHalf))
                        {
                            --toCutFromLeftHull;
                        }
                        else
                        {
                            toLeaveInRightHull++;
                        }
                    }

                    current.leftSubHullSize = toCutFromLeftHull;
                    Treap<Point>.SplitBySize(current.Left.ConvexHull, toCutFromLeftHull, out leftLeftHalf, out leftRightHalf);
                    Treap<Point>.SplitBySize(current.Right.ConvexHull, toLeaveInRightHull, out rightLeftHalf, out rightRightHalf);

                    current.ConvexHull = Treap<Point>.Merge(leftLeftHalf, rightRightHalf);
                    current.Left.ConvexHull = leftRightHalf;
                    current.Right.ConvexHull = rightLeftHalf;
                }
            }

            private static void pushSubHullDown(RedBlackNode current)
            {
                if (current != null && !current.IsLeaf && current.ConvexHull != null)
                {
                    Treap<Point> leftHalf;
                    Treap<Point> rightHalf;
                    Treap<Point>.SplitBySize(current.ConvexHull, current.leftSubHullSize, out leftHalf, out rightHalf);
                    current.Left.ConvexHull = Treap<Point>.Merge(leftHalf, current.Left.ConvexHull);
                    current.Right.ConvexHull = Treap<Point>.Merge(current.Right.ConvexHull, rightHalf);

                    current.ConvexHull = null;
                }
            }

            private RedBlackNode recountToUp(RedBlackNode current)
            {
                while (current != null && !current.IsRoot)
                {
                    recount(current);
                    current = current.Parent;
                }
                recount(current);
                return current;
            }

            #endregion hull_recount_operations

            #region red_black_tree_operations

            private RedBlackNode insert(RedBlackNode current, Point pointToInsert)
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
                    current.ConvexHull = null;

                    recount(current);
                    return initialInsertRepair(current);
                }
                else
                {
                    pushSubHullDown(current);

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

            private RedBlackNode initialInsertRepair(RedBlackNode current)
            {
                // considering the spacific of our insertion - if current not black (in this case we have to do nothing)
                // current and current.Brother were red leafs before last insert, so we can make start already from 
                // case 3 in https://ru.wikipedia.org/wiki/%D0%9A%D1%80%D0%B0%D1%81%D0%BD%D0%BE-%D1%87%D1%91%D1%80%D0%BD%D0%BE%D0%B5_%D0%B4%D0%B5%D1%80%D0%B5%D0%B2%D0%BE 

                if (current.IsBlack == RedBlackNode.Color.Red)
                {
                    current.IsBlack = RedBlackNode.Color.Black;
                    current.Brother.IsBlack = RedBlackNode.Color.Black;
                    current.Parent.IsBlack = RedBlackNode.Color.Red;
                    return repairAfterInsert(current.Parent);
                }
                else
                {
                    return recountToUp(current);
                }
            }

            private RedBlackNode repairAfterInsert(RedBlackNode current)
            {
                recount(current);
                if (current.IsRoot)
                {
                    current.IsBlack = RedBlackNode.Color.Black;
                    return current;
                }
                else if (current.Parent.IsBlack == RedBlackNode.Color.Red)
                {
                    if (current.Uncle != null && current.Uncle.IsBlack == RedBlackNode.Color.Red)
                    {
                        current.Parent.IsBlack = RedBlackNode.Color.Black;
                        current.Uncle.IsBlack = RedBlackNode.Color.Black;
                        current.Grandparent.IsBlack = RedBlackNode.Color.Red;

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

                        current.Parent.IsBlack = RedBlackNode.Color.Black;
                        current.Grandparent.IsBlack = RedBlackNode.Color.Red;

                        recount(current.Parent);

                        if (current == current.Parent.Left)
                        {
                            current.Grandparent.RotateRight();
                            current = current.Parent;
                            recount(current.Right);
                        }
                        else
                        {
                            current.Grandparent.RotateLeft();
                            current = current.Parent;
                            recount(current.Left);
                        }
                    }
                }
                return recountToUp(current);
            }

            private RedBlackNode delete(RedBlackNode current, Point pointToDelete)
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

                    if (current.Brother.IsBlack == RedBlackNode.Color.Red || current.Parent.IsBlack == RedBlackNode.Color.Red)
                    {
                        current.Brother.IsBlack = RedBlackNode.Color.Black;

                        return recountToUp(current.Brother);
                    }
                    else
                    {
                        return repairAfterDelete(current.Brother);
                    }
                }
                else if (!current.IsLeaf)
                {
                    pushSubHullDown(current);

                    if (pointToDelete.CompareTo(current.Left.MaxPoint) <= 0)
                    {
                        return delete(current.Left, pointToDelete);
                    }
                    else
                    {
                        return delete(current.Right, pointToDelete);
                    }
                }
                return recountToUp(current);
            }

            private RedBlackNode repairAfterDelete(RedBlackNode current)
            {
                recount(current);
                if (current.IsRoot)
                {
                    return current;
                }
                else
                {
                    if (current.Brother.IsBlack == RedBlackNode.Color.Red)
                    {
                        current.Parent.IsBlack = RedBlackNode.Color.Red;
                        current.Brother.IsBlack = RedBlackNode.Color.Black;
                        if (current.Parent.Left == current)
                        {
                            current.Parent.RotateLeft();
                        }
                        else
                        {
                            current.Parent.RotateRight();
                        }
                    }

                    if (current.Parent.IsBlack == RedBlackNode.Color.Black && 
                        current.Brother.IsBlack == RedBlackNode.Color.Black && 
                        current.Brother.Left.IsBlack == RedBlackNode.Color.Black && 
                        current.Brother.Right.IsBlack == RedBlackNode.Color.Black)
                    {
                        current.Brother.IsBlack = RedBlackNode.Color.Red;
                        return repairAfterDelete(current.Parent);
                    }
                    else if (current.Parent.IsBlack == RedBlackNode.Color.Red && 
                        current.Brother.IsBlack == RedBlackNode.Color.Black && 
                        current.Brother.Left.IsBlack == RedBlackNode.Color.Black && 
                        current.Brother.Right.IsBlack == RedBlackNode.Color.Black)
                    {
                        current.Brother.IsBlack = RedBlackNode.Color.Red;
                        current.Parent.IsBlack = RedBlackNode.Color.Black;
                        return recountToUp(current.Parent);
                    }
                    else // if (current.Brother.IsBlack) always true
                    {
                        pushSubHullDown(current.Brother);

                        if (current.Parent.Left == current && 
                            current.Brother.Left.IsBlack == RedBlackNode.Color.Red && 
                            current.Brother.Right.IsBlack == RedBlackNode.Color.Black)
                        {
                            current.Brother.IsBlack = RedBlackNode.Color.Red;
                            current.Brother.Left.IsBlack = RedBlackNode.Color.Black;

                            current.Brother.RotateRight();

                            recount(current.Brother.Right);
                            recount(current.Brother);
                        }
                        else if (current.Parent.Right == current && 
                            current.Brother.Left.IsBlack == RedBlackNode.Color.Black && 
                            current.Brother.Right.IsBlack == RedBlackNode.Color.Red)
                        {
                            current.Brother.IsBlack = RedBlackNode.Color.Red;
                            current.Brother.Right.IsBlack = RedBlackNode.Color.Black;

                            current.Brother.RotateLeft();

                            recount(current.Brother.Left);
                            recount(current.Brother);
                        }

                        current.Brother.IsBlack = current.Parent.IsBlack;
                        current.Parent.IsBlack = RedBlackNode.Color.Black;

                        if (current == current.Parent.Left)
                        {
                            current.Brother.Right.IsBlack = RedBlackNode.Color.Black;
                            current.Parent.RotateLeft();
                        }
                        else
                        {
                            current.Brother.Left.IsBlack = RedBlackNode.Color.Black;
                            current.Parent.RotateRight();
                        }
                        return recountToUp(current.Parent);
                    }
                }
            }

            #endregion #region red_black_tree_operations
        }
    }
}
