using System;
using OET_Math;
using OET_Types.Elements;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using OET_Types.Entities;

namespace OET_2DEngine
{
    public static class Engine
    {
        public static float MeshSize;

        public static void GenerateNodesByEntity(GlobalDocument GD)
        {
            float threshold = MeshSize / 2 ;
            List<Node> tempList = new List<Node>();
            List<Node> steelList = new List<Node>();
            List<XYPT> polyPoints = new List<XYPT>();
            List<IEntity> tempEntity = new List<IEntity>();

            tempEntity.AddRange(GD.Entities);
            SortEntities(ref tempEntity);

            foreach (var entity in tempEntity)
            {
                // CONCRETE
                if (entity.EntityType == eEntityType.area)
                {
                    Area area = (Area)entity;
                    Polygon unmeshedPoly = new Polygon();

                    foreach (PointF point in area.Points)
                    {
                        unmeshedPoly.Points.Add(new XYPT(point.X, point.Y));
                    }

                    Polygon meshedPoly = MeshPolygonSequential(unmeshedPoly);

                    foreach (XYPT pt in meshedPoly.Points)
                    {
                        Node node = new Node((float)pt.XX,(float)pt.YY , eMaterial.Concrete);
                        node.AreaIDList.Add(area.ID);
                        node.Thickness = area.Thickness;
                        tempList.Add(node);
                    }
                }
                // STEEL
                else if (entity.EntityType == eEntityType.segment)
                {
                    Segment segment = (Segment)entity;
                    Line line = new Line();
                    foreach (PointF point in segment.Points)
                    {
                        line.Points.Add(new XYPT(point.X , point.Y));
                    }
                    line.SortPoints();
                    //var testSteelPoints = new List<Node>();
                    XYPT pdrop;
                    foreach (var pt in (tempList.FindAll(x => x.Coord.YY <= line.BoundingPoints[1].y)
                                                .FindAll(x => x.Coord.YY >= line.BoundingPoints[0].y)
                                                .FindAll(x => x.Coord.XX <= line.BoundingPoints[1].x)
                                                .FindAll(x => x.Coord.XX >= line.BoundingPoints[0].x)))
                    {
                        if (line.Pt2LinePt(line.Points[0], line.Points[1], pt.Coord, out pdrop)
                            && pt.Coord.DistTo(pdrop) < threshold)
                        {
                            pt.Material = eMaterial.Steel;
                            pt.RebarCount = segment.Count;
                            pt.RebarSize = segment.Size;
                            pt.SegmentIDList.Add(segment.ID);
                            steelList.Add(pt);

                            //testSteelPoints.Add(pt);
                        }
                    }
                    //KruskalReorder(ref templist);
                }
            }

            GD.Nodes = SortAndNumberNodeList(tempList);
        }

        private static void KruskalReorder(ref List<Node> list)
        {
            foreach (var point in list)
            {
                if (point.Material == eMaterial.Steel)
                {
                    var steelNeighbors = list.FindAll(x =>  x.Material == eMaterial.Steel && x.Coord.XX >= point.Coord.XX && x.Coord.YY >= point.Coord.YY &&
                                                            x.Coord.DistTo(point.Coord) > 0 &&
                                                            x.Coord.DistTo(point.Coord) <= Math.Sqrt(MeshSize * MeshSize * 2) + 0.1);
                    if (steelNeighbors.Count >= 1)
                    {
                        steelNeighbors.OrderByDescending(x => x.Coord.DistTo(point.Coord));
                        var keep = steelNeighbors.Last();
                        steelNeighbors.Remove(keep);

                        foreach (var item in steelNeighbors.FindAll(x => x.Coord != keep.Coord))
                        {
                            list.Find(x => x.Coord == item.Coord).Material = eMaterial.Concrete;
                            list.Find(x => x.Coord == item.Coord).RebarCount = 0;
                            list.Find(x => x.Coord == item.Coord).RebarSize = 0;
                        }
                    }
                }
            }
        }

        public static void GenerateFramesByNodes(GlobalDocument GD)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            int id = 0;
            for (int i = 0; i < GD.Nodes.Count; i++)
            {
                for (int j = i+1; j < GD.Nodes.Count; j++)
                {
                    if (i != j  && GD.Nodes[i].Coord.DistTo(GD.Nodes[j].Coord) < GD.Horizon)
                    {
                        id++;
                        if (GD.Nodes[i].Material == eMaterial.Steel && GD.Nodes[j].Material == eMaterial.Steel &&
                            GD.Nodes[i].SegmentIDList.Intersect(GD.Nodes[j].SegmentIDList).Any() )
                        {
                            if (GD.Nodes[i].Coord.DistTo(GD.Nodes[j].Coord) < Math.Sqrt(GD.MeshSize * GD.MeshSize * 2) + 0.1)
                            {
                                var rebar = new Rebar(GD.Nodes[i], GD.Nodes[j], GD.Nodes[i].RebarCount, GD.Nodes[j].RebarSize);
                                rebar.Id = id;
                                GD.Frames.Add(rebar);
                            }
                        }
                        else if(GD.Nodes[i].AreaIDList.Intersect(GD.Nodes[j].AreaIDList).Any())
                        {
                            var concrete = new Concrete(GD.Nodes[i], GD.Nodes[j]);
                            concrete.Id = id;
                            GD.Frames.Add(concrete);
                        }
                    }
                    else if(Math.Abs(GD.Nodes[i].Coord.YY - GD.Nodes[j].Coord.YY) > GD.Horizon)
                    {
                        break;
                    }
                }
            }
            st.Stop();
            string time = (st.ElapsedMilliseconds / 1000).ToString();           
        }
        
        public static void GenerateNodeLoads(GlobalDocument GD)
        {
            List<IEntity> tempEntity = new List<IEntity>();
            List<Dot> dotlist = new List<Dot>();


            tempEntity.AddRange(GD.Entities.FindAll(x=>x.EntityType == eEntityType.dot));
            foreach (var entity in tempEntity)
            {
                var dot = (Dot)entity;
                foreach (var node in GD.Nodes.FindAll(x => x.Coord == new XYPT(dot.Points[0].X, dot.Points[0].Y)))
                {
                   
                }
            }

        }
        
        public static List<Load> GenerateLoads(GlobalDocument GD)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        private static List<Node> DistinctConcrete2(List<Node> list)
        {
            var retList = new List<Node>();

            HashSet<XYPT> seenKeys = new HashSet<XYPT>();
            for (int i = 0; i < list.Count; i++)
            {
                if (seenKeys.Add(list[i].Coord))
                {
                    retList.Add(list[i]);
                }
                else
                {
                    var k = seenKeys.ToList().IndexOf(list[i].Coord);
                    retList[k].Thickness = (retList[k].Thickness + list[i].Thickness) / 2;
                    retList[k].AreaIDList.AddRange(list[i].AreaIDList);
                }
            }
            return retList;
        }

        private static List<Node> SortAndNumberNodeList(List<Node> list)
        {

            //list = list.DistinctBy(x => x.Coord).ToList();
            list = DistinctConcrete2(list);
            List<Node> ordererdlist = new List<Node>();
            ordererdlist = list.OrderBy(x => x.Coord.y).ToList();

            int id = 0;
            foreach (var item in ordererdlist)
            {
                id++;
                item.Id = id;
            }

            return ordererdlist;
        } 

        private static Polygon MeshPolygonSequential(Polygon polygon)
        {
            Stopwatch st = new Stopwatch();
            st.Start();

            var tempPolygon = new Polygon();
            int xCount = (int)((polygon.BoundingPoints[1].x - polygon.BoundingPoints[0].x) / MeshSize) + 1;
            int yCount = (int)((polygon.BoundingPoints[1].y - polygon.BoundingPoints[0].y) / MeshSize) + 1;
            for (int j = 0; j < yCount; j++)
            {
                for (int i = 0; i < xCount; i++)
                {
                    XYPT currentPoint = new XYPT(polygon.BoundingPoints[0].x + i * MeshSize, polygon.BoundingPoints[0].y + j * MeshSize);

                    Polygon innerPoly = new Polygon();
                    innerPoly.AddPoint(currentPoint);
                    innerPoly.AddPoint(currentPoint + new XYPT(MeshSize, 0));
                    innerPoly.AddPoint(currentPoint + new XYPT(MeshSize, MeshSize));
                    innerPoly.AddPoint(currentPoint + new XYPT(0, MeshSize));

                    if (polygon.PolygonInPolygon(innerPoly,0.7f))
                    {
                        foreach (var pt in innerPoly.Points)
                        {
                            tempPolygon.AddPoint(pt);
                        }
                    }
                }
            }
            st.Stop();
            string time = (st.ElapsedMilliseconds / 1000).ToString();
            var MeshedPolygon = new Polygon(tempPolygon.Points.Distinct().OrderBy(x=>x.XX).ToList());
            return MeshedPolygon;
        }

        private static void RemoveUnconnectibles(Polygon bigPolygon, List<IElement> frames)
        {
            foreach (var vertex in bigPolygon.Points)
            {
                List<XYPT> neighbors = new List<XYPT>();

                neighbors.Add(new XYPT(vertex.x - MeshSize, vertex.y - MeshSize));
                neighbors.Add(new XYPT(vertex.x , vertex.y - MeshSize));
                neighbors.Add(new XYPT(vertex.x + MeshSize, vertex.y - MeshSize));
                neighbors.Add(new XYPT(vertex.x + MeshSize, vertex.y ));
                neighbors.Add(new XYPT(vertex.x + MeshSize, vertex.y + MeshSize));
                neighbors.Add(new XYPT(vertex.x , vertex.y + MeshSize));
                neighbors.Add(new XYPT(vertex.x - MeshSize, vertex.y + MeshSize));
                neighbors.Add(new XYPT(vertex.x - MeshSize, vertex.y ));


                var outPoint = neighbors.FindAll(x => !bigPolygon.PointInPolygon(x));

                //Line intersect = new Line(vertex, outPoint);

            }
        }

        private static bool IsConnectible(IElement frame,float horizon)
        {
            float dx = (float)(frame.EndNode.Coord.x - frame.StartNode.Coord.x);
            float dy = (float)(frame.EndNode.Coord.y - frame.StartNode.Coord.y);

            float fraction = frame.Length / horizon;
            
            return false;
        }

        private static Polygon MeshPolygonParalel(Polygon polygon)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            ConcurrentBag<XYPT> polygonBag = new ConcurrentBag<XYPT>();

            int xCount = (int)((polygon.BoundingPoints[1].x - polygon.BoundingPoints[0].x) / MeshSize) + 1;
            int yCount = (int)((polygon.BoundingPoints[1].y - polygon.BoundingPoints[0].y) / MeshSize) + 1;

            //Parallel.For(0, yCount, j =>
            for (int j = 0; j < yCount; j++)
            {
                Parallel.For(0, xCount, i =>
                {
                    XYPT currentPoint = new XYPT(polygon.BoundingPoints[0].x + i * MeshSize, polygon.BoundingPoints[0].y + j * MeshSize);

                    ConcurrentBag<XYPT> innerBag = new ConcurrentBag<XYPT>();
                    innerBag.Add(currentPoint);
                    innerBag.Add(currentPoint + new XYPT(MeshSize, 0));
                    innerBag.Add(currentPoint + new XYPT(MeshSize, MeshSize));
                    innerBag.Add(currentPoint + new XYPT(0, MeshSize));
                    Polygon innerPoly = new Polygon(innerBag.ToList());

                    if (polygon.PolygonInPolygon(innerPoly))
                    {
                        foreach (var pt in innerPoly.Points)
                        {
                            polygonBag.Add(pt);
                        }
                    }
                });
            }

            string time = (st.ElapsedMilliseconds / 1000).ToString();
            var MeshedPolygon = new Polygon(polygonBag.Distinct().ToList());
            return MeshedPolygon;
        }

        private static void SortEntities(ref List<IEntity> entities)
        {
            var tempList = new List<IEntity>();
            foreach (Area entity in entities.FindAll(x => x.EntityType == eEntityType.area))
            {
                tempList.Add(entity);
            }
            foreach (Segment entity in entities.FindAll(x => x.EntityType == eEntityType.segment))
            {
                tempList.Add(entity);
            }
            foreach (Dot entity in entities.FindAll(x => x.EntityType == eEntityType.dot))
            {
                tempList.Add(entity);
            }
            entities = tempList;
        }


    }
}
