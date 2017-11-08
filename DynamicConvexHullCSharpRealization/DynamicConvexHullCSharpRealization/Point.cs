using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class Point : IComparable<Point>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
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
