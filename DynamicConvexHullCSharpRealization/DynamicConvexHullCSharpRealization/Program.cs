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
            hull.GetHull();

            Console.WriteLine();
            Console.WriteLine("---------------------");
            Console.ReadKey();
        }
    }
}
