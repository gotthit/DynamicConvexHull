using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class Treap<T> where T : IComparable<T>
    {
        private static Random randomGenerator = new Random();

        public int Size { get; private set; }

        public T MaxElement { get; private set; }
        public T MinElement { get; private set; }

        public Treap<T> Left { get; private set; }
        public Treap<T> Right { get; private set; }

        public T Key { get; private set; }

        public Treap(T newKey, Treap<T> newLeft = null, Treap<T> newRight = null)
        {
            Key = newKey;
            Left = newLeft;
            Right = newRight;
            Size = 1;

            MaxElement = newKey;
            MinElement = newKey;

            update();
        }

        public static int GetSize(Treap<T> treap)
        {
            return treap == null ? 0 : treap.Size;
        }

        private void update()
        {
            Size = GetSize(Left) + GetSize(Right) + 1;

            MaxElement = Key;
            MinElement = Key;

            if (Left != null)
            {
                MaxElement = Utils.Max(MaxElement, Left.MaxElement);
                MinElement = Utils.Min(MinElement, Left.MinElement);
            }
            if (Right != null)
            {
                MaxElement = Utils.Max(MaxElement, Right.MaxElement);
                MinElement = Utils.Min(MinElement, Right.MinElement);
            }
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
                current.Left = rightHalf;
                current.update();
                rightHalf = current;
            }
            else
            {
                Split(current.Left, divider, out leftHalf, out rightHalf);
                current.Right = leftHalf;
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
            else if (GetSize(current.Left) >= toCut)
            {
                SplitBySize(current.Left, toCut, out leftHalf, out rightHalf);
                current.Left = rightHalf;
                current.update();
                rightHalf = current;
            }
            else
            {
                SplitBySize(current.Right, toCut - GetSize(current.Left) - 1, out leftHalf, out rightHalf);
                current.Right = leftHalf;
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
            if (randomGenerator.Next(0, leftTreap.Size + rightTreap.Size) < leftTreap.Size)
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

        public void GetArray()
        {
            Left?.GetArray();
            Console.Write(Key + " ");
            Right?.GetArray();
        }
    }
}
