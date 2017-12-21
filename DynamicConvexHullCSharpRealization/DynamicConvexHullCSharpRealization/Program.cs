using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

            List<Point> list = new List<Point>();

            var rand = new Random();

            while (true)
            {

                for (int i = 0; i < 100; ++i)
                {
                    Point p = new Point(rand.Next(5, 1200), rand.Next(5, 600));
                    hull.Insert(p);
                    list.Add(p);

                    Console.Clear();

                    List<Point> res = hull.GetHull();

                    DrawGraphic.DrawPolygon(res);
                    DrawGraphic.DrawPoints(list);

                    //Thread.Sleep(200);
                    Console.ReadKey();
                }

                for (int i = 0; i < 100; ++i)
                {
                    Point p = list[0];
                    hull.Delete(p);
                    list.Remove(p);

                    Console.Clear();

                    List<Point> res = hull.GetHull();

                    DrawGraphic.DrawPolygon(res);
                    DrawGraphic.DrawPoints(list);

                    //Thread.Sleep(200);
                    Console.ReadKey();
                }
            }

            Console.ReadKey();
        }
    }
}
