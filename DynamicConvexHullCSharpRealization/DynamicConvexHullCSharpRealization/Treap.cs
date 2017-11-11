using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class Treap<T> where T : IComparable<T>
    {
        private static Random randomGeneranor = new Random();

        private int priority;

        public Treap<T> left { get; private set; }
        public Treap<T> right { get; private set; }

        public T Key { get; private set; }

        public Treap(T newKey, Treap<T> newLeft = null, Treap<T> newRight = null)
        {
            Key = newKey;
            left = newLeft;
            right = newRight;

            priority = randomGeneranor.Next();
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
                Split(current.right, divider, out leftHalf, out rightHalf);
                current.right = rightHalf;
                rightHalf = current;
            }
            else
            {
                Split(current.left, divider, out leftHalf, out rightHalf);
                current.left = leftHalf;
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
            if (leftTreap.priority > rightTreap.priority)
            {
                leftTreap.right = Merge(leftTreap.right, rightTreap);
                return leftTreap;
            }
            else
            {
                rightTreap.left = Merge(leftTreap, rightTreap.left);
                return rightTreap;
            }
        }
    }
}
