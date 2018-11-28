using System.Collections.Generic;
using System;
using OET_Math;
namespace ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            XYPT point = new XYPT(21, 40);
            Polygon big = new Polygon();

            big.AddPoint(new XYPT(20, 20));
            big.AddPoint(new XYPT(40, 20));
            big.AddPoint(new XYPT(40, 40));
            big.AddPoint(new XYPT(20, 40));

            big.PointInPolygon(point);
            

            //foreach (var point in Unsorted.Points)
            //{
            //    Console.WriteLine(point.x + " " + point.y);
            //}

            Console.Read();
        }
    }
}
