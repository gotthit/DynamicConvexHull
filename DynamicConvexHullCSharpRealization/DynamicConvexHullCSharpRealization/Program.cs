using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConvexHullCSharpRealization
{
    class Program
    {
        static void Main(string[] args)
        {
            DynamicConvexHull hull = new DynamicConvexHull();
            hull.Insert(new Point(0, 0));
            hull.Insert(new Point(1, 1));
            hull.Insert(new Point(0, 2));
            hull.Insert(new Point(2, 3));
            hull.Delete(new Point(2, 3));
            hull.Insert(new Point(2, 3));
            hull.GetHull();

            Console.WriteLine();
            Console.WriteLine("---------------------");
            Console.ReadKey();
        }
    }
}
