using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace DynamicConvexHullCSharpRealization
{
    class DrawGraphic
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);

        static IntPtr hWnd = IntPtr.Zero;
        static IntPtr hDC = IntPtr.Zero;

        static Random rand = new Random();

        public static void DrawPolygon(List<Point> mypoints)
        {
            hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                hDC = GetDC(hWnd);
                if (hDC != IntPtr.Zero)
                {
                    using (Graphics consoleGraphics = Graphics.FromHdc(hDC))
                    {
                        PointF[] points = mypoints.Select((point) => new PointF((float)point.X, (float)point.Y)).ToArray();

                        Pen whitePen = new Pen(Color.FromArgb(rand.Next(100, 255), rand.Next(100, 255), rand.Next(100, 255)), 2);

                        for (int i = 0; i < points.Length - 1; ++i)
                        {
                            consoleGraphics.DrawLine(whitePen, points[i], points[i + 1]);
                        }
                         if (points.Length > 0)
                            consoleGraphics.DrawLine(whitePen, points[points.Length - 1], points[0]);


                        //SolidBrush whiteBrush = new SolidBrush(Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)));

                        //consoleGraphics.FillPolygon(whiteBrush, points);


                        whitePen.Dispose();
                    }

                }
                ReleaseDC(hWnd, hDC);
                DeleteDC(hDC);

            }

        }

        public static void DrawPoints(List<Point> mypoints)
        {
            hWnd = GetConsoleWindow();
            if (hWnd != IntPtr.Zero)
            {
                hDC = GetDC(hWnd);
                if (hDC != IntPtr.Zero)
                {
                    using (Graphics consoleGraphics = Graphics.FromHdc(hDC))
                    {
                        PointF[] points = mypoints.Select((point) => new PointF((float)point.X, (float)point.Y)).ToArray();

                        for (int i = 0; i < points.Length - 1; ++i)
                        {
                            Pen whitePen = new Pen(Color.FromArgb(rand.Next(100, 255), rand.Next(100, 255), rand.Next(100, 255)), 3.5f);

                            consoleGraphics.DrawEllipse(whitePen, new RectangleF((float)points[i].X - 3.5f, (float)points[i].Y - 3.5f, 7, 7));

                            whitePen.Dispose();
                        }

                        if (points.Length != 0)
                        {
                            Pen whitePen = new Pen(Color.FromArgb(rand.Next(255, 255), rand.Next(0, 0), rand.Next(0, 0)), 3.5f);

                            consoleGraphics.DrawEllipse(whitePen, new RectangleF((float)points[points.Length - 1].X - 3.5f, (float)points[points.Length - 1].Y - 3.5f, 7, 7));

                            whitePen.Dispose();
                        }
                    }

                }
                ReleaseDC(hWnd, hDC);
                DeleteDC(hDC);

            }

        }

    }
}
