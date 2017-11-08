using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class RedBlackNode
    {
        private RedBlackNode left;
        private RedBlackNode right;
        private RedBlackNode parent;

        private bool isBlack;

        private Point point;
        private Bridge bridge;

        private bool isLeaf { get { return left == null && right == null; } }
        private bool isRoot { get { return parent == null; } }

        private RedBlackNode grandparent { get { return parent?.parent; } }
        private RedBlackNode uncle
        {
            get
            {
                if (grandparent == null)
                {
                    return null;
                }
                if (grandparent.left == parent)
                {
                    return grandparent.right;
                }
                return grandparent.left;
            }
        }

        public RedBlackNode(Point newPoint = null, Bridge newBridge = null,
                            RedBlackNode newParent = null, RedBlackNode newLeft = null, RedBlackNode newRight = null)
        {
            left = newLeft;
            right = newRight;
            parent = newParent;

            point = newPoint;
            bridge = newBridge;

            isBlack = isRoot;
        }

        private void rotateLeft()
        {
            if (right != null)
            {
                RedBlackNode rightSon = right;
                rightSon.parent = parent;

                if (parent != null)
                {
                    if (parent.left == this)
                    {
                        parent.left = rightSon;
                    }
                    else
                    {
                        parent.right = rightSon;
                    }
                }
                right = rightSon.left;
                if (rightSon.left != null)
                {
                    rightSon.left.parent = this;
                }
                rightSon.left = this;
                parent = rightSon;
            }
        }

        private void rotateRight()
        {
            if (left != null)
            {
                RedBlackNode leftSon = left;
                leftSon.parent = parent;

                if (parent != null)
                {
                    if (parent.left == this)
                    {
                        parent.left = leftSon;
                    }
                    else
                    {
                        parent.right = leftSon;
                    }
                }
                left = leftSon.right;
                if (leftSon.right != null)
                {
                    leftSon.right.parent = this;
                }
                leftSon.right = this;
                parent = leftSon;
            }
        }

        public void
    }
}
