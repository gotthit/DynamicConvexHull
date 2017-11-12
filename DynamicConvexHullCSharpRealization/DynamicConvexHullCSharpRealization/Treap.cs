using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class Treap<T> where T : IComparable<T>
    {
        private static Random randomGenerator = new Random();

        public int size { get; private set; }

        public Treap<T> Left { get; private set; }
        public Treap<T> Right { get; private set; }

        public T Key { get; private set; }

        public Treap(T newKey, Treap<T> newLeft = null, Treap<T> newRight = null)
        {
            Key = newKey;
            Left = newLeft;
            Right = newRight;
            size = 1;
        }

        public static int GetSize(Treap<T> treap)
        {
            return treap == null ? 0 : treap.size;
        }

        private void update()
        {
            size = GetSize(Left) + GetSize(Right) + 1;
        }

        public static void Split(Treap<T> current, T divider, out Treap<T> leftHalf, out Treap<T> rightHalf)
        {
            if (current == null)
            {
                leftHalf = null;
                rightHalf = null;
            }
            if (divider.CompareTo(current.Key) > 0)
            {
                Split(current.Right, divider, out leftHalf, out rightHalf);
                current.Right = rightHalf;
                current.update();
                rightHalf = current;
            }
            else
            {
                Split(current.Left, divider, out leftHalf, out rightHalf);
                current.Left = leftHalf;
                current.update();
                leftHalf = current;
            }
        }

        public static void SplitBySize(Treap<T> current, int toCut, out Treap<T> leftHalf, out Treap<T> rightHalf)
        {
            if (current == null)
            {
                leftHalf = null;
                rightHalf = null;
            }
            if (GetSize(current.Left) >= toCut)
            {
                SplitBySize(current.Right, toCut, out leftHalf, out rightHalf);
                current.Right = rightHalf;
                current.update();
                rightHalf = current;
            }
            else
            {
                SplitBySize(current.Left, toCut - GetSize(current.Left) - 1, out leftHalf, out rightHalf);
                current.Left = leftHalf;
                current.update();
                leftHalf = current;
            }
        }

        public static Treap<T> Merge(Treap<T> leftTreap, Treap<T> rightTreap)
        {
            if (leftTreap == null)
            {
                return rightTreap;
            }
            if (rightTreap == null)
            {
                return leftTreap;
            }
            if (randomGenerator.Next(0, leftTreap.size + rightTreap.size) < leftTreap.size)
            {
                leftTreap.Right = Merge(leftTreap.Right, rightTreap);
                leftTreap.update();
                return leftTreap;
            }
            else
            {
                rightTreap.Left = Merge(leftTreap, rightTreap.Left);
                rightTreap.update();
                return rightTreap;
            }
        }
    }
}
