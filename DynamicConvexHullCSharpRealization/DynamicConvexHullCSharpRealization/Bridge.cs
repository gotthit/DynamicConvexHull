using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class Bridge
    {
        public Point Left { get; set; }
        public Point Right { get; set; }

        public Bridge(Point left, Point right)
        {
            Left = left;
            Right = right;
        }
    }
}
