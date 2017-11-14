using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    partial class DynamicConvexHull
    {
        // TODO: we can use one red bleck tree to hold both hulls

        private HalfOfDynamicConvexHull leftHull;
        private HalfOfDynamicConvexHull rightHull;

        public DynamicConvexHull()
        {
            leftHull = new HalfOfDynamicConvexHull(true);
            rightHull = new HalfOfDynamicConvexHull(false);
        }

        public void Insert(Point point)
        {
            leftHull.Insert(point);
            rightHull.Insert(point);
        }

        public void Delete(Point point)
        {
            leftHull.Delete(point);
            rightHull.Delete(point);
        }

        public List<Point> GetHull()
        {
            leftHull.GetHalfHull();
            Console.WriteLine();
            rightHull.GetHalfHull();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("---------------------");
            Console.WriteLine();

            return new List<Point>();
        }
    }
}
