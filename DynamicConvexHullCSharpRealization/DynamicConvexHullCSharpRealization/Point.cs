using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class Point : IComparable<Point>
    {
        public readonly double X;
        public readonly double Y;

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(Point other)
        {
            if (this.Y < other.Y || (this.Y == other.Y && this.X < other.X))
            {
                return -1;
            }
            if (this.Y > other.Y || (this.Y == other.Y && this.X > other.X))
            {
                return 1;
            }
            return 0;
        }
    }
}
