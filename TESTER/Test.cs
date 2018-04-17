using Microsoft.VisualStudio.TestTools.UnitTesting;
using OET_Math;

namespace TESTER
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        public void SortFor2Points()
        {
            Polygon Unsorted = new Polygon();
            Polygon Sorted = new Polygon();

            Unsorted.AddPoint(new XYPT(0, 2));
            Unsorted.AddPoint(new XYPT(2, 0));

            Sorted.AddPoint(new XYPT(2, 0));
            Sorted.AddPoint(new XYPT(0, 2));

            Unsorted.SortPoints();

            for (int i = 0; i < Unsorted.Count; i++)
            {
                Assert.AreEqual(Sorted.Points[i], Unsorted.Points[i]);
            }

        }

        [TestMethod]
        public void SortForTriangle()
        {
            Polygon Unsorted = new Polygon();
            Polygon Sorted = new Polygon();

            Unsorted.AddPoint(new XYPT(2, 0));
            Unsorted.AddPoint(new XYPT(0, 2));
            Unsorted.AddPoint(new XYPT(-2, 0));

            Sorted.AddPoint(new XYPT(-2, 0));
            Sorted.AddPoint(new XYPT(2, 0));
            Sorted.AddPoint(new XYPT(0, 2));

            Unsorted.SortPoints();

            for (int i = 0; i < Unsorted.Count; i++)
            {
                Assert.AreEqual(Sorted.Points[i], Unsorted.Points[i]);
            }

        }

        [TestMethod]
        public void SortForRectangle()
        {
            Polygon Unsorted = new Polygon();
            Polygon Sorted = new Polygon();

            Unsorted.AddPoint(new XYPT(2, 0));
            Unsorted.AddPoint(new XYPT(0, 2));
            Unsorted.AddPoint(new XYPT(0, 0));
            Unsorted.AddPoint(new XYPT(2, 2));

            Sorted.AddPoint(new XYPT(0, 0));
            Sorted.AddPoint(new XYPT(2, 0));
            Sorted.AddPoint(new XYPT(2, 2));
            Sorted.AddPoint(new XYPT(0, 2));

            Unsorted.SortPoints();

            for (int i = 0; i < Unsorted.Count; i++)
            {
                Assert.AreEqual(Sorted.Points[i], Unsorted.Points[i]);
            }

        }

        [TestMethod]
        public void SortForPentagonWithTopAtBottom()
        {
            Polygon Unsorted = new Polygon();
            Polygon Sorted = new Polygon();

            Sorted.AddPoint(new XYPT(-2, 1));
            Sorted.AddPoint(new XYPT(0, 0));
            Sorted.AddPoint(new XYPT(2, 1));
            Sorted.AddPoint(new XYPT(1, 2));
            Sorted.AddPoint(new XYPT(-1, 2));

            Unsorted.AddPoint(new XYPT(-2, 1));
            Unsorted.AddPoint(new XYPT(1, 2));
            Unsorted.AddPoint(new XYPT(2, 1));
            Unsorted.AddPoint(new XYPT(0, 0));
            Unsorted.AddPoint(new XYPT(-1, 2));

            Unsorted.SortPoints();

            for (int i = 0; i < Unsorted.Count; i++)
            {
                Assert.AreEqual(Sorted.Points[i], Unsorted.Points[i]);
            }

        }

        [TestMethod]
        public void SortForPentagonWithTopAtBottomAndALittleToleft()
        {
            Polygon Unsorted = new Polygon();
            Polygon Sorted = new Polygon();

            Sorted.AddPoint(new XYPT(-2, 1));
            Sorted.AddPoint(new XYPT(-0.5, 0));
            Sorted.AddPoint(new XYPT(2, 1));
            Sorted.AddPoint(new XYPT(1, 2));
            Sorted.AddPoint(new XYPT(-1, 2));

            Unsorted.AddPoint(new XYPT(-2, 1));
            Unsorted.AddPoint(new XYPT(1, 2));
            Unsorted.AddPoint(new XYPT(2, 1));
            Unsorted.AddPoint(new XYPT(-0.5, 0));
            Unsorted.AddPoint(new XYPT(-1, 2));

            Unsorted.SortPoints();

            for (int i = 0; i < Unsorted.Count; i++)
            {
                Assert.AreEqual(Sorted.Points[i], Unsorted.Points[i]);
            }

        }

        [TestMethod]
        public void SortForOctagon()
        {
            Polygon Unsorted = new Polygon();
            Polygon Sorted = new Polygon();

            Sorted.AddPoint(new XYPT(-2, 1));
            Sorted.AddPoint(new XYPT(-1, 0));
            Sorted.AddPoint(new XYPT(1, 0));
            Sorted.AddPoint(new XYPT(2, 1));
            Sorted.AddPoint(new XYPT(2, 2));
            Sorted.AddPoint(new XYPT(1, 3));
            Sorted.AddPoint(new XYPT(-1, 3));
            Sorted.AddPoint(new XYPT(-2, 2));

            Unsorted.AddPoint(new XYPT(1, 0));
            Unsorted.AddPoint(new XYPT(-1, 0));
            Unsorted.AddPoint(new XYPT(2, 1));
            Unsorted.AddPoint(new XYPT(1, 3));
            Unsorted.AddPoint(new XYPT(-2, 1));
            Unsorted.AddPoint(new XYPT(2, 2));
            Unsorted.AddPoint(new XYPT(-1, 3));
            Unsorted.AddPoint(new XYPT(-2, 2));

            Unsorted.SortPoints();

            for (int i = 0; i < Unsorted.Count; i++)
            {
                Assert.AreEqual(Sorted.Points[i], Unsorted.Points[i]);
            }

        }

        [TestMethod]
        public void SortForTenGonFor7PointsBelow()
        {
            Polygon Unsorted = new Polygon();
            Polygon Sorted = new Polygon();

            Sorted.AddPoint(new XYPT(-3,-1));
            Sorted.AddPoint(new XYPT(-2, -2));
            Sorted.AddPoint(new XYPT(-1, -3));
            Sorted.AddPoint(new XYPT(0, -4));
            Sorted.AddPoint(new XYPT(1, -3));
            Sorted.AddPoint(new XYPT(2, -2));
            Sorted.AddPoint(new XYPT(3, -1));
            Sorted.AddPoint(new XYPT(4, 2));
            Sorted.AddPoint(new XYPT(0, 6));
            Sorted.AddPoint(new XYPT(-4, 2));

            Unsorted.AddPoint(new XYPT(0, -4));
            Unsorted.AddPoint(new XYPT(-2, -2));
            Unsorted.AddPoint(new XYPT(-1, -3));
            Unsorted.AddPoint(new XYPT(-3, -1));
            Unsorted.AddPoint(new XYPT(3, -1));
            Unsorted.AddPoint(new XYPT(2, -2));
            Unsorted.AddPoint(new XYPT(0, 6));
            Unsorted.AddPoint(new XYPT(-4, 2));
            Unsorted.AddPoint(new XYPT(1, -3));
            Unsorted.AddPoint(new XYPT(4, 2));

            Unsorted.SortPoints();

            for (int i = 0; i < Unsorted.Count; i++)
            {
                Assert.AreEqual(Sorted.Points[i], Unsorted.Points[i]);
            }

        }

        [TestMethod]
        public void PointInPolygon()
        {
            XYPT point1 = new XYPT(1.999999999,1.999999999);

            Polygon big = new Polygon();

            big.AddPoint(new XYPT(-2, -2));
            big.AddPoint(new XYPT(2, -2));
            big.AddPoint(new XYPT(2, 2));
            big.AddPoint(new XYPT(-2, 2));

            Assert.AreEqual(true,big.PointInPolygon(point1));
        }

        [TestMethod]
        public void PointInPolygonbotleft()
        {
            XYPT point1 = new XYPT(-2, -2);

            Polygon big = new Polygon();

            big.AddPoint(new XYPT(-2, -2));
            big.AddPoint(new XYPT(2, -2));
            big.AddPoint(new XYPT(2, 2));
            big.AddPoint(new XYPT(-2, 2));

            Assert.AreEqual(true, big.PointInPolygon(point1));

        }

        [TestMethod]
        public void PointInPolygonbotright()
        {
            XYPT point2 = new XYPT(2, -2);

            Polygon big = new Polygon();

            big.AddPoint(new XYPT(-2, -2));
            big.AddPoint(new XYPT(2, -2));
            big.AddPoint(new XYPT(2, 2));
            big.AddPoint(new XYPT(-2, 2));

            Assert.AreEqual(true, big.PointInPolygon(point2));

        }

        [TestMethod]
        public void PointInPolygontopright()
        {

            XYPT point3 = new XYPT(2, 2);
            Polygon big = new Polygon();

            big.AddPoint(new XYPT(-2, -2));
            big.AddPoint(new XYPT(2, -2));
            big.AddPoint(new XYPT(2, 2));
            big.AddPoint(new XYPT(-2, 2));

            Assert.AreEqual(true, big.PointInPolygon(point3));
        }

        [TestMethod]
        public void PointInPolygontopleft()
        {
            XYPT point4 = new XYPT(-2, 2);
            Polygon big = new Polygon();

            big.AddPoint(new XYPT(-2, -2));
            big.AddPoint(new XYPT(2, -2));
            big.AddPoint(new XYPT(2, 2));
            big.AddPoint(new XYPT(-2, 2));

            Assert.AreEqual(true, big.PointInPolygon(point4));
        }

        [TestMethod]
        public void SmallPolygonInsideBigPolygon()
        {
            Polygon big = new Polygon();
            Polygon small = new Polygon();

            big.AddPoint(new XYPT(-2, -2));
            big.AddPoint(new XYPT(2, -2));
            big.AddPoint(new XYPT(2, 2));
            big.AddPoint(new XYPT(-2, 2));


            small.AddPoint(new XYPT(-9, -9));
            small.AddPoint(new XYPT(1, -1));
            small.AddPoint(new XYPT(1, 1));
            small.AddPoint(new XYPT(-1, 1));


            Assert.AreEqual(true,big.PolygonInPolygon(small, 0.7f));

        }

        [TestMethod]
        public void PointMustNotBeInPolygonTriangle()
        {
            XYPT point4 = new XYPT(20, 0);

            Polygon big = new Polygon();

            big.AddPoint(new XYPT(20, 20));
            big.AddPoint(new XYPT(180, 0));
            big.AddPoint(new XYPT(80, 120));


            Assert.AreNotEqual(true, big.PointInPolygon(point4));
        }

        [TestMethod]
        public void PointMustBeInPolygonLineBottom()
        {
            XYPT point4 = new XYPT(21,20);
            Polygon big = new Polygon();

            big.AddPoint(new XYPT(20, 20));
            big.AddPoint(new XYPT(40, 20));
            big.AddPoint(new XYPT(40, 40));
            big.AddPoint(new XYPT(20, 40));


            Assert.AreEqual(true, big.PointInPolygon(point4));
        }

        [TestMethod]
        public void PointMustBeInPolygonLineTop()
        {
            XYPT point4 = new XYPT(21, 40);
            Polygon big = new Polygon();

            big.AddPoint(new XYPT(20, 20));
            big.AddPoint(new XYPT(40, 20));
            big.AddPoint(new XYPT(40, 40));
            big.AddPoint(new XYPT(20, 40));


            Assert.AreEqual(true, big.PointInPolygon(point4));
        }

        [TestMethod]
        public void PolygonInPolygon()
        {
            Polygon big = new Polygon();
            Polygon small = new Polygon();

            big.AddPoint(new XYPT(0, 0));
            big.AddPoint(new XYPT(10, 0));
            big.AddPoint(new XYPT(0, 10));
            big.AddPoint(new XYPT(10, 10));
            big.AddPoint(new XYPT(20, 10));
            big.AddPoint(new XYPT(0, 20));
            big.AddPoint(new XYPT(10, 20));
            big.AddPoint(new XYPT(20, 20));

            small.AddPoint(new XYPT(10, 0));
            small.AddPoint(new XYPT(20, 0));
            small.AddPoint(new XYPT(10, 10));
            small.AddPoint(new XYPT(20, 10));

            Assert.AreEqual(false,big.PolygonInPolygon(small,0.8f));
        }

    }
}
