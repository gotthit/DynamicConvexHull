﻿using System;
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
            // (х3 - х1) * (у2 - у1) - (у3 - у1) * (х2 - х1)

            double vectorMultiplictionResult = (toDetermine.X - beginVector.X) * (endVector.Y - beginVector.Y) - 
                                            (toDetermine.Y - beginVector.Y) * (endVector.X - beginVector.X);

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
