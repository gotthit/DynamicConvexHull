using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    partial class DynamicConvexHull
    {
        private RedBlackNode hullRoot;

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

        public List<Point> GetHull()
        {
            if (hullRoot != null)
            {
                List<Point> leftHullList = hullRoot.GetConvexHull(true).GetArray();
                List<Point> rightHullList = hullRoot.GetConvexHull(false).GetArray();
                rightHullList.Reverse();

                int beginRight = 0;
                int endRight = rightHullList.Count;

                while (beginRight < leftHullList.Count &&
                       beginRight < rightHullList.Count &&
                       leftHullList[leftHullList.Count - beginRight - 1].CompareTo(rightHullList[beginRight]) == 0)
                {
                    ++beginRight;
                }
                while (rightHullList.Count - endRight < leftHullList.Count &&
                       endRight >= 0 &&
                       leftHullList[rightHullList.Count - endRight].CompareTo(rightHullList[endRight - 1]) == 0)
                {
                    --endRight;
                }
                if (endRight > beginRight)
                {
                    leftHullList.AddRange(rightHullList.GetRange(beginRight, endRight - beginRight));
                }
                return leftHullList;
            }
            return new List<Point>();
        }

        #region hull_recount_operations

        private static PointPosition determinePosition(Point beginVector, Point endVector, Point toDetermine, bool isLeftHalf)
        {
            // we change answer for right hull to make it esier to analize in other methods
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
                    if (!isLeftHalf)
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

        private static double getXbyY(Point first, Point second, double yCoord)
        {
            return (yCoord - first.Y) * (second.X - first.X) / (second.Y - first.Y) + first.X;
        }

        private static bool isIntersectLoverY(Point firstLeft, Point secondLeft, Point firstRight, Point secondRight, double yCoord, bool isLeftHalf)
        {
            double leftXbyY = getXbyY(firstLeft, secondLeft, yCoord);
            double rightXbyY = getXbyY(firstRight, secondRight, yCoord);
            if (isLeftHalf)
            {
                return leftXbyY < rightXbyY;
            }
            else
            {
                return rightXbyY < leftXbyY;
            }
        }

        private static void goLeft(ref Treap<Point> current, ref Point currentPoint, ref Point minPoint, ref Point maxPoint,
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
                throw new Exception("ha haaaa!");

                maxPoint = currentPoint;
                maxToCut = toCut;

                currentPoint = minPoint;
                toCut = minToCut;

                minPoint = null;
                minToCut = 0;

                current = null;
            }
        }

        private static void goRight(ref Treap<Point> current, ref Point currentPoint, ref Point minPoint, ref Point maxPoint,
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
                throw new Exception("ha haaaa!");

                minPoint = currentPoint;
                minToCut = toCut;

                currentPoint = maxPoint;
                toCut = maxToCut;

                maxPoint = null;
                maxToCut = int.MaxValue - 100;

                current = null;
            }
        }

        private static bool makeDecisionAboutRotation(ref Treap<Point> lowerCurrent, ref Point lowerCurrentPoint, Point lowerPrevPoint, Point lowerNextPoint,
                                                      ref Point lowerMinPoint, ref Point lowerMaxPoint,
                                                      ref int toCutFromLowerHull, ref int minPointCutFromLower, ref int maxPointCutFromLower,
                                                      ref Treap<Point> upperCurrent, ref Point upperCurrentPoint, Point upperPrevPoint, Point upperNextPoint,
                                                      ref Point upperMinPoint, ref Point upperMaxPoint,
                                                      ref int toLeaveInUpperHull, ref int minPointLeaveInUpper, ref int maxPointLeaveInUpper,
                                                      double mediumY, bool isLeftHalf)
        {
            PointPosition lowerPrevPos = determinePosition(lowerCurrentPoint, upperCurrentPoint, lowerPrevPoint, isLeftHalf);
            PointPosition lowerNextPos = determinePosition(lowerCurrentPoint, upperCurrentPoint, lowerNextPoint, isLeftHalf);

            PointPosition upperPrevPos = determinePosition(lowerCurrentPoint, upperCurrentPoint, upperPrevPoint, isLeftHalf);
            PointPosition upperNextPos = determinePosition(lowerCurrentPoint, upperCurrentPoint, upperNextPoint, isLeftHalf);

            if (lowerPrevPos == PointPosition.Right && lowerNextPos == PointPosition.Right &&
                upperPrevPos == PointPosition.Right && upperNextPos == PointPosition.Right) // A - found
            {
                return true; // answer found
            }
            else if (lowerPrevPos == PointPosition.Left && lowerNextPos == PointPosition.Right &&
                     upperPrevPos == PointPosition.Right && upperNextPos == PointPosition.Right) // B
            {
                goLeft(ref lowerCurrent, ref lowerCurrentPoint, ref lowerMinPoint, ref lowerMaxPoint,
                       ref toCutFromLowerHull, ref minPointCutFromLower, ref maxPointCutFromLower);

                //goRight(ref rightTop, ref rightTopPoint, ref rightMinPoint, ref rightMaxPoint,
                //       ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight);
            }
            else if (lowerPrevPos == PointPosition.Right && lowerNextPos == PointPosition.Left &&
                     upperPrevPos == PointPosition.Right && upperNextPos == PointPosition.Right) // C
            {
                goRight(ref lowerCurrent, ref lowerCurrentPoint, ref lowerMinPoint, ref lowerMaxPoint,
                       ref toCutFromLowerHull, ref minPointCutFromLower, ref maxPointCutFromLower);

                //goRight(ref rightTop, ref rightTopPoint, ref rightMinPoint, ref rightMaxPoint,
                //       ref toLeaveInRightHull, ref minPointLeaveInRight, ref maxPointLeaveInRight);
            }
            else if (lowerPrevPos == PointPosition.Right && lowerNextPos == PointPosition.Right &&
                     upperPrevPos == PointPosition.Right && upperNextPos == PointPosition.Left) // D
            {
                //goLeft(ref leftTop, ref leftTopPoint, ref leftMinPoint, ref leftMaxPoint,
                //       ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft);

                goRight(ref upperCurrent, ref upperCurrentPoint, ref upperMinPoint, ref upperMaxPoint,
                       ref toLeaveInUpperHull, ref minPointLeaveInUpper, ref maxPointLeaveInUpper);
            }
            else if (lowerPrevPos == PointPosition.Right && lowerNextPos == PointPosition.Right &&
                     upperPrevPos == PointPosition.Left && upperNextPos == PointPosition.Right) // E
            {
                //goLeft(ref leftTop, ref leftTopPoint, ref leftMinPoint, ref leftMaxPoint,
                //       ref toCutFromLeftHull, ref minPointCutFromLeft, ref maxPointCutFromLeft);

                goLeft(ref upperCurrent, ref upperCurrentPoint, ref upperMinPoint, ref upperMaxPoint,
                       ref toLeaveInUpperHull, ref minPointLeaveInUpper, ref maxPointLeaveInUpper);
            }
            else if (lowerPrevPos == PointPosition.Left && lowerNextPos == PointPosition.Right &&
                     upperPrevPos == PointPosition.Right && upperNextPos == PointPosition.Left) // F
            {
                goLeft(ref lowerCurrent, ref lowerCurrentPoint, ref lowerMinPoint, ref lowerMaxPoint,
                       ref toCutFromLowerHull, ref minPointCutFromLower, ref maxPointCutFromLower);

                goRight(ref upperCurrent, ref upperCurrentPoint, ref upperMinPoint, ref upperMaxPoint,
                       ref toLeaveInUpperHull, ref minPointLeaveInUpper, ref maxPointLeaveInUpper);
            }
            else if (lowerPrevPos == PointPosition.Left && lowerNextPos == PointPosition.Right &&
                     upperPrevPos == PointPosition.Left && upperNextPos == PointPosition.Right) // G
            {
                goLeft(ref lowerCurrent, ref lowerCurrentPoint, ref lowerMinPoint, ref lowerMaxPoint,
                       ref toCutFromLowerHull, ref minPointCutFromLower, ref maxPointCutFromLower);
            }
            else if (lowerPrevPos == PointPosition.Right && lowerNextPos == PointPosition.Left &&
                     upperPrevPos == PointPosition.Right && upperNextPos == PointPosition.Left) // H
            {
                goRight(ref upperCurrent, ref upperCurrentPoint, ref upperMinPoint, ref upperMaxPoint,
                       ref toLeaveInUpperHull, ref minPointLeaveInUpper, ref maxPointLeaveInUpper);
            }
            else if (lowerPrevPos == PointPosition.Right && lowerNextPos == PointPosition.Left &&
                     upperPrevPos == PointPosition.Left && upperNextPos == PointPosition.Right) // I - the hardest
            {
                if (isIntersectLoverY(lowerCurrentPoint, lowerNextPoint, upperCurrentPoint, upperPrevPoint, mediumY, isLeftHalf))
                {
                    goRight(ref lowerCurrent, ref lowerCurrentPoint, ref lowerMinPoint, ref lowerMaxPoint,
                       ref toCutFromLowerHull, ref minPointCutFromLower, ref maxPointCutFromLower);
                }
                else
                {
                    goLeft(ref upperCurrent, ref upperCurrentPoint, ref upperMinPoint, ref upperMaxPoint,
                       ref toLeaveInUpperHull, ref minPointLeaveInUpper, ref maxPointLeaveInUpper);
                }
            }
            else
            {
                throw new Exception("something wrong");
            }

            return false; // answer not found
        }

        private static void recount(RedBlackNode current)
        {
            recount(current, true);
            recount(current, false);
        }

        private static void recount(RedBlackNode current, bool isLeftHalf)
        {
            if (current != null && current.Left != null && current.Right != null && current.GetConvexHull(isLeftHalf) == null)
            {
                current.MaxPoint = Utils.Max(current.Left.MaxPoint, current.Right.MaxPoint);

                Treap<Point> lowerCurrent = current.Left.GetConvexHull(isLeftHalf);
                Treap<Point> upperCurrent = current.Right.GetConvexHull(isLeftHalf);
                Point loweCurrentPoint = lowerCurrent.Key;
                Point upperCurrentPoint = upperCurrent.Key;

                Point lowerMinPoint = null;
                Point lowerMaxPoint = null;
                Point upperMinPoint = null;
                Point upperMaxPoint = null;

                double mediumY = (lowerCurrent.MaxElement.Y + upperCurrent.MinElement.Y) / 2;

                int toCutFromLowerHull = Treap<Point>.GetSize(lowerCurrent?.Left) + 1;
                int toLeaveInUpperHull = Treap<Point>.GetSize(upperCurrent?.Left) + 1;

                int minPointCutFromLower = 0;
                int maxPointCutFromLower = lowerCurrent.Size;

                int minPointLeaveInUpper = 0;
                int maxPointLeaveInUpper = upperCurrent.Size;

                bool bridgeFound = false;
                while (!bridgeFound)
                {
                    Point lowerPrevPoint = lowerCurrent?.Left != null ? lowerCurrent.Left.MaxElement : lowerMinPoint;
                    Point lowerNextPoint = lowerCurrent?.Right != null ? lowerCurrent.Right.MinElement : lowerMaxPoint;

                    Point upperPrevPoint = upperCurrent?.Left != null ? upperCurrent.Left.MaxElement : upperMinPoint;
                    Point upperNextPoint = upperCurrent?.Right != null ? upperCurrent.Right.MinElement : upperMaxPoint;

                    bridgeFound = makeDecisionAboutRotation(ref lowerCurrent, ref loweCurrentPoint, lowerPrevPoint, lowerNextPoint,
                                               ref lowerMinPoint, ref lowerMaxPoint,
                                               ref toCutFromLowerHull, ref minPointCutFromLower, ref maxPointCutFromLower,
                                               ref upperCurrent, ref upperCurrentPoint, upperPrevPoint, upperNextPoint,
                                               ref upperMinPoint, ref upperMaxPoint,
                                               ref toLeaveInUpperHull, ref minPointLeaveInUpper, ref maxPointLeaveInUpper,
                                               mediumY, isLeftHalf);
                }
                // we want to have top point in right treap after split
                --toLeaveInUpperHull;

                Treap<Point> leftPartOfLowerHull;
                Treap<Point> rightPartOfLowerHull;
                Treap<Point> leftPartOfUpperHull;
                Treap<Point> rightPartOfUpperHull;

                if (loweCurrentPoint.Y == upperCurrentPoint.Y)
                {
                    if ((loweCurrentPoint.X < upperCurrentPoint.X && isLeftHalf) ||
                        (loweCurrentPoint.X > upperCurrentPoint.X && !isLeftHalf))
                    {
                        ++toLeaveInUpperHull;
                    }
                    else if ((loweCurrentPoint.X > upperCurrentPoint.X && isLeftHalf) ||
                             (loweCurrentPoint.X < upperCurrentPoint.X && !isLeftHalf))
                    {
                        --toCutFromLowerHull;
                    }
                    else
                    {
                        toLeaveInUpperHull++;
                    }
                }

                current.SetLowerSubHullSize(isLeftHalf, toCutFromLowerHull);
                Treap<Point>.SplitBySize(current.Left.GetConvexHull(isLeftHalf), toCutFromLowerHull, out leftPartOfLowerHull, out rightPartOfLowerHull);
                Treap<Point>.SplitBySize(current.Right.GetConvexHull(isLeftHalf), toLeaveInUpperHull, out leftPartOfUpperHull, out rightPartOfUpperHull);

                current.SetConvexHull(isLeftHalf, Treap<Point>.Merge(leftPartOfLowerHull, rightPartOfUpperHull));
                current.Left.SetConvexHull(isLeftHalf, rightPartOfLowerHull);
                current.Right.SetConvexHull(isLeftHalf, leftPartOfUpperHull);
            }
        }

        private static void pushSubHullDown(RedBlackNode current)
        {
            pushSubHullDown(current, true);
            pushSubHullDown(current, false);
        }

        private static void pushSubHullDown(RedBlackNode current, bool isLeftHalf)
        {
            if (current != null && !current.IsLeaf && current.GetConvexHull(isLeftHalf) != null)
            {
                Treap<Point> leftHalf;
                Treap<Point> rightHalf;
                Treap<Point>.SplitBySize(current.GetConvexHull(isLeftHalf), current.GeLowerSubHullSize(isLeftHalf), out leftHalf, out rightHalf);

                current.Left.SetConvexHull(isLeftHalf, Treap<Point>.Merge(leftHalf, current.Left.GetConvexHull(isLeftHalf)));
                current.Right.SetConvexHull(isLeftHalf, Treap<Point>.Merge(current.Right.GetConvexHull(isLeftHalf), rightHalf));

                current.SetConvexHull(isLeftHalf, null);
            }
        }

        private static RedBlackNode recountToUp(RedBlackNode current)
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

        private static RedBlackNode insert(RedBlackNode current, Point pointToInsert)
        {
            if (current.IsLeaf && pointToInsert.CompareTo(current.MaxPoint) != 0)
            {
                if (pointToInsert.CompareTo(current.MaxPoint) < 0)
                {
                    current.Left = new RedBlackNode(pointToInsert, current);
                    current.Right = new RedBlackNode(current.MaxPoint, current);
                }
                else
                {
                    current.Left = new RedBlackNode(current.MaxPoint, current);
                    current.Right = new RedBlackNode(pointToInsert, current);
                }
                current.SetConvexHull(true, null);
                current.SetConvexHull(false, null);

                recount(current);
                return initialInsertRepair(current);
            }
            else if (!current.IsLeaf)
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
            return recountToUp(current);
        }

        private static RedBlackNode initialInsertRepair(RedBlackNode current)
        {
            // considering the specific of our insertion - if current not black (in this case we have to do nothing)
            // current and current.Brother were red leafs before last insert, so we can make start already from 
            // case 3 in https://ru.wikipedia.org/wiki/%D0%9A%D1%80%D0%B0%D1%81%D0%BD%D0%BE-%D1%87%D1%91%D1%80%D0%BD%D0%BE%D0%B5_%D0%B4%D0%B5%D1%80%D0%B5%D0%B2%D0%BE 

            if (current.NodeColor == RedBlackNode.Color.Red)
            {
                current.NodeColor = RedBlackNode.Color.Black;
                current.Brother.NodeColor = RedBlackNode.Color.Black;
                current.Parent.NodeColor = RedBlackNode.Color.Red;
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
                current.NodeColor = RedBlackNode.Color.Black;
                return current;
            }
            else if (current.Parent.NodeColor == RedBlackNode.Color.Red)
            {
                if (current.Uncle != null && current.Uncle.NodeColor == RedBlackNode.Color.Red)
                {
                    current.Parent.NodeColor = RedBlackNode.Color.Black;
                    current.Uncle.NodeColor = RedBlackNode.Color.Black;
                    current.Grandparent.NodeColor = RedBlackNode.Color.Red;

                    recount(current.Parent);

                    return repairAfterInsert(current.Grandparent);
                }
                else
                {
                    if (current.Grandparent.Left == current.Parent && current.Parent.Right == current)
                    {
                        pushSubHullDown(current);
                        current.Parent.RotateLeft();
                        current = current.Left;

                        recount(current);
                    }
                    else if (current.Grandparent.Right == current.Parent && current.Parent.Left == current)
                    {
                        pushSubHullDown(current);
                        current.Parent.RotateRight();
                        current = current.Right;

                        recount(current);
                    }

                    current.Parent.NodeColor = RedBlackNode.Color.Black;
                    current.Grandparent.NodeColor = RedBlackNode.Color.Red;

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

                if (current.Brother.NodeColor == RedBlackNode.Color.Red || current.Parent.NodeColor == RedBlackNode.Color.Red)
                {
                    current.Brother.NodeColor = RedBlackNode.Color.Black;

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

        private static RedBlackNode repairAfterDelete(RedBlackNode current)
        {
            recount(current);
            if (current.IsRoot)
            {
                return current;
            }
            else
            {
                if (current.Brother.NodeColor == RedBlackNode.Color.Red)
                {
                    current.Parent.NodeColor = RedBlackNode.Color.Red;
                    current.Brother.NodeColor = RedBlackNode.Color.Black;
                    pushSubHullDown(current.Brother);
                    if (current.Parent.Left == current)
                    {
                        current.Parent.RotateLeft();
                    }
                    else
                    {
                        current.Parent.RotateRight();
                    }
                }

                if (current.Parent.NodeColor == RedBlackNode.Color.Black &&
                    current.Brother.NodeColor == RedBlackNode.Color.Black &&
                    current.Brother.Left.NodeColor == RedBlackNode.Color.Black &&
                    current.Brother.Right.NodeColor == RedBlackNode.Color.Black)
                {
                    current.Brother.NodeColor = RedBlackNode.Color.Red;
                    return repairAfterDelete(current.Parent);
                }
                else if (current.Parent.NodeColor == RedBlackNode.Color.Red &&
                    current.Brother.NodeColor == RedBlackNode.Color.Black &&
                    current.Brother.Left.NodeColor == RedBlackNode.Color.Black &&
                    current.Brother.Right.NodeColor == RedBlackNode.Color.Black)
                {
                    current.Brother.NodeColor = RedBlackNode.Color.Red;
                    current.Parent.NodeColor = RedBlackNode.Color.Black;
                    return recountToUp(current.Parent);
                }
                else // if (current.Brother.IsBlack) always true
                {
                    pushSubHullDown(current.Brother);

                    if (current.Parent.Left == current &&
                        current.Brother.Left.NodeColor == RedBlackNode.Color.Red &&
                        current.Brother.Right.NodeColor == RedBlackNode.Color.Black)
                    {
                        current.Brother.NodeColor = RedBlackNode.Color.Red;
                        current.Brother.Left.NodeColor = RedBlackNode.Color.Black;

                        pushSubHullDown(current.Brother.Left);
                        current.Brother.RotateRight();

                        recount(current.Brother.Right);
                    }
                    else if (current.Parent.Right == current &&
                        current.Brother.Left.NodeColor == RedBlackNode.Color.Black &&
                        current.Brother.Right.NodeColor == RedBlackNode.Color.Red)
                    {
                        current.Brother.NodeColor = RedBlackNode.Color.Red;
                        current.Brother.Right.NodeColor = RedBlackNode.Color.Black;

                        pushSubHullDown(current.Brother.Right);
                        current.Brother.RotateLeft();

                        recount(current.Brother.Left);
                    }

                    current.Brother.NodeColor = current.Parent.NodeColor;
                    current.Parent.NodeColor = RedBlackNode.Color.Black;

                    if (current == current.Parent.Left)
                    {
                        current.Brother.Right.NodeColor = RedBlackNode.Color.Black;
                        current.Parent.RotateLeft();
                    }
                    else
                    {
                        current.Brother.Left.NodeColor = RedBlackNode.Color.Black;
                        current.Parent.RotateRight();
                    }
                    return recountToUp(current.Parent);
                }
            }
        }

        #endregion #region red_black_tree_operations
    }
}
