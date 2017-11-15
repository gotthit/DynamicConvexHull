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

            //hull.Insert(new Point(, ));

            //hull.Delete(new Point(, ));

            hull.Insert(new Point(4, 2));

            hull.Insert(new Point(2, 4));

            hull.Insert(new Point(1, 0));

            hull.Insert(new Point(5, 5));

            hull.Insert(new Point(0, 0));

            hull.Insert(new Point(4, 4));

            hull.Insert(new Point(3, 3));

            hull.Insert(new Point(2, 2));

            hull.Insert(new Point(3, 2));

            hull.Insert(new Point(0, 1));

            hull.Insert(new Point(1, 3));

            hull.Insert(new Point(4, 1));

            var rand = new Random(23523);

            for (int i = 0; i < 10000; ++i)
            {
                hull.Insert(new Point(rand.Next(-10000, 10000), rand.Next(-10000, 10000)));
            }

            for (int i = 0; i < 10000; ++i)
            {
                hull.Delete(new Point(rand.Next(-10000, 10000), rand.Next(-10000, 10000)));
            }

            //hull.Delete(new Point(1, 3));
            //hull.Delete(new Point(1, 3));
            //hull.Delete(new Point(1, 3));
            //hull.Delete(new Point(1, 3));


            //hull.Delete(new Point(5, 5));

            //hull.Delete(new Point(4, 4));

            //hull.Delete(new Point(0, 1));

            //hull.Delete(new Point(0, 0));

            //hull.Delete(new Point(1, 3)); // maybe should check here

            //hull.Delete(new Point(3, 3));

            //hull.Delete(new Point(2, 4));

            //hull.Delete(new Point(2, 2));

            //hull.Delete(new Point(4, 1));

            //hull.Delete(new Point(1, 0));

            //hull.Delete(new Point(3, 2));

            //hull.Delete(new Point(4, 2));



            //hull.Insert(new Point(4, 3));
            //hull.Insert(new Point(4, 2));
            //hull.Insert(new Point(6, 1));
            //hull.Insert(new Point(1, 3));
            //hull.Insert(new Point(6, 5));

            //hull.Delete(new Point(1, 3));

            //hull.Insert(new Point(1, 3));

            hull.GetHull();

            Console.ReadKey();
        }
    }
}
