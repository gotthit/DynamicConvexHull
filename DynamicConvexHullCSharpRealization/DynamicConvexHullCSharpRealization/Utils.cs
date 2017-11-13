using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    static class Utils
    {
        public static T Max<T>(T first, T second) where T : IComparable<T>
        {
            if (first.CompareTo(second) >= 0)
            {
                return first;
            }
            else
            {
                return second;
            }
        }

        public static T Min<T>(T first, T second) where T : IComparable<T>
        {
            if (first.CompareTo(second) <= 0)
            {
                return first;
            }
            else
            {
                return second;
            }
        }

        public static PointPosition DeterminePosition(Point beginVector, Point endVector, Point toDetermine)
        {
            double vectorMultiplictionResult = (endVector.X - beginVector.X) * (toDetermine.Y - beginVector.Y) - 
                                            (endVector.Y - beginVector.Y) * (toDetermine.X - beginVector.X);

            if (vectorMultiplictionResult > 0.0000001)
            {
                return PointPosition.Right;
            }
            else if (vectorMultiplictionResult < 0.0000001)
            {
                return PointPosition.Left;
            }
            else
            {
                return PointPosition.On;
            }
        }
    }
}
