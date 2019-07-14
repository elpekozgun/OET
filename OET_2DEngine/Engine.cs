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
            float threshold = MeshSize / 1.95f  ;
            List<Node> tempList = new List<Node>();
            List<XYPT> polyPoints = new List<XYPT>();
            List<IEntity> tempEntity = new List<IEntity>();
            List<Node> steelnodes = new List<Node>();
            List<int> segmentCount = new List<int>();

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
                        Node node = new Node((float)pt.XX, (float)pt.YY, eMaterial.Concrete);
                        node.AreaIDList.Add(area.ID);
                        node.Thickness = area.Thickness;
                        tempList.Add(node);
                    }
                }
                // STEEL
                else if (entity.EntityType == eEntityType.segment)
                {
                    List<Node> steels = new List<Node>();
                    Segment segment = (Segment)entity;
                    segmentCount.Add(segment.ID);
                    Line line = new Line();
                    foreach (PointF point in segment.Points)
                    {
                        line.Points.Add(new XYPT(point.X, point.Y));
                    }
                    line.SortPoints();

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
                            steels.Add(pt);
                            steelnodes.Add(pt);
                        }
                    }
                }
            }
            AdjustPoints2(tempList, steelnodes,segmentCount);
            SortAndNumberNodeList(ref tempList);

            GD.Nodes = tempList;
        }

        private static void AdjustPoints2(List<Node> tempList, List<Node> steelNodes,List<int> segmentCount)
        {
            for (int i = 0; i < segmentCount.Count; i++)
            {
                List<XYPT> coords = new List<XYPT>();
                List<int> deletes = new List<int>();
                var steels = steelNodes.FindAll(x => x.SegmentIDList.Contains(segmentCount[i]));

                if (steels.Count > 0)
                {

                    for (int j = 0; j < steels.Count; j++)
                    {
                        if (!coords.Contains(steels[j].Coord))
                            coords.Add(steels[j].Coord);
                        else
                            deletes.Add(j);
                    }
                    for (int j = deletes.Count - 1; j >= 0; j--)
                    {
                        steels.RemoveAt(deletes[j]);
                    }

                    Line bounding = new Line(coords);
                    bounding.SortPoints();

                    var lineXsegments = (int)Math.Abs(bounding.BoundingPoints[1].x - bounding.BoundingPoints[0].x) / MeshSize;
                    var lineYsegments = (int)Math.Abs(bounding.BoundingPoints[1].y - bounding.BoundingPoints[0].y) / MeshSize;
                    var actualCount = Math.Max(lineXsegments, lineYsegments);
                    var shorter = Math.Min(lineXsegments, lineYsegments);

                    if (steels.Count > actualCount)
                    {
                        for (int j = 0; j < actualCount; j++)
                        {
                            List<Node> removed = new List<Node>();

                            if (shorter == lineXsegments)
                                removed = steels.FindAll(x => x.Coord.YY == bounding.BoundingPoints[0].YY + (j + 1) * MeshSize);
                            else
                                removed = steels.FindAll(x => x.Coord.XX == bounding.BoundingPoints[0].XX + (j + 1) * MeshSize);

                            if (removed != null && removed.Count > 1)
                            {
                                for (int k = 0; k < removed.Count - 1; k++)
                                {
                                    if (removed.Any(x=>x.SegmentIDList.Count > 1))
                                    {
                                        Node node = removed.Find(x => x.SegmentIDList.Count < 2);
                                        if (node != null)
                                        {
                                            node.Material = eMaterial.Concrete;
                                            node.RebarCount = 0;
                                            node.RebarSize = 0;
                                        }
                                    }
                                    else
                                    {
                                        removed[0].Material = eMaterial.Concrete;
                                        removed[0].RebarCount = 0;
                                        removed[0].RebarSize = 0;
                                    }
                                }
                            }
                        }
                    }
                }

            }

        }


        private static void AdjustPoints(List<Node> tempList, List<Node> steels)
        {
            List<XYPT> coords = new List<XYPT>();
            List<int> deletes = new List<int>();
            for (int i = 0; i < steels.Count; i++)
            {
                if (!coords.Contains(steels[i].Coord))
                    coords.Add(steels[i].Coord);
                else
                    deletes.Add(i);
            }
            for (int i = deletes.Count - 1; i >= 0 ; i--)
            {
                steels.RemoveAt(deletes[i]);
            }


            Line bounding = new Line(coords);
            bounding.SortPoints();

            var lineXsegments = (int)Math.Abs(bounding.BoundingPoints[1].x - bounding.BoundingPoints[0].x) / MeshSize;
            var lineYsegments = (int)Math.Abs(bounding.BoundingPoints[1].y - bounding.BoundingPoints[0].y) / MeshSize;
            var actualCount = Math.Max(lineXsegments, lineYsegments);
            var shorter = Math.Min(lineXsegments, lineYsegments);

            if (steels.Count > actualCount)
            {
                for (int i = 0; i < actualCount; i++)
                {
                    List<Node> removed = new List<Node>();

                    if (shorter == lineXsegments)
                        removed = steels.FindAll(x => x.Coord.YY == bounding.BoundingPoints[0].YY + (i + 1) * MeshSize);
                    else
                        removed = steels.FindAll(x => x.Coord.XX == bounding.BoundingPoints[0].XX + (i + 1) * MeshSize);

                    if (removed != null && removed.Count > 1)
                    {
                        for (int j = 0; j < removed.Count - 1; j++)
                        {
                            if (!(removed[0].Material == eMaterial.Concrete))
                                if (!removed.Any(x=>x.Material == eMaterial.Concrete))
                                {

                                    removed[0].Material = eMaterial.Concrete;
                                    removed[0].RebarCount = 0;
                                    removed[0].RebarSize = 0;

                                }
                        }
                    }
                }
            }
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
                    if (steelNeighbors.Count > 1)
                    {
                        steelNeighbors.OrderByDescending(x => x.Coord.DistTo(point.Coord));
                        var keep = steelNeighbors.Last();
                        steelNeighbors.Remove(keep);

                        foreach (var item in steelNeighbors.FindAll(x => x.Coord != keep.Coord))
                        {
                            var node = list.Find(x => x.Coord == item.Coord);
                            if (node!= null)
                            {
                                node.Material = eMaterial.Concrete;
                                node.RebarCount = 0;
                                node.RebarSize = 0;
                            }
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
                            GD.Nodes[i].SegmentIDList.Intersect(GD.Nodes[j].SegmentIDList).Any())
                        {

                            if (GD.Nodes[i].Coord.DistTo(GD.Nodes[j].Coord) < Math.Sqrt(GD.MeshSize * GD.MeshSize * 2) + 0.1)
                            {
                                {
                                    var rebar = new Rebar(GD.Nodes[i], GD.Nodes[j], GD.Nodes[i].RebarCount, GD.Nodes[j].RebarSize);
                                    rebar.Id = id;
                                    GD.Frames.Add(rebar);
                                }
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
        
        public static void RefactorFrames(GlobalDocument GD)
        {
            var listofnodes = GD.Nodes;

            var temp = listofnodes.Where(x => x.Material == eMaterial.Steel).ToList();
            for (int j = 0; j < temp.Count; j++)
            {
                var test = GD.Frames.FindAll(x=>x.Material == eMaterial.Steel && (x.EndNode == temp[j] || x.StartNode == temp[j]));
                //if (test.FindAll(x=>x.EndNode == temp[j]).Count >= 2 && test.FindAll(x => x.EndNode == temp[j]).Count < 3)
                if (test.FindAll(x=>x.EndNode == temp[j]).Count == 2  && test.FindAll(x => x.StartNode == temp[j]).Count == 1)
                {
                    //var node = GD.Nodes.Find(x => x.Material== eMaterial.Steel && x == test[0].EndNode);
                    var node = test[0].EndNode;
                    
                    node.Material = eMaterial.Concrete;
                    node.RebarCount = 0;
                    node.RebarSize = 0;

                    //listofnodes.Remove(test[0].EndNode);
                    //listofnodes.Add(node);

                    for (int i = 0; i < test.FindAll(x => x.EndNode == temp[j]).Count; i++)
                    {
                        IElement frame = GD.Frames.Find(x => x == test[i]);
                        if (node == frame.StartNode)
                        {
                            Concrete conc = new Concrete(node, frame.EndNode);
                            conc.Id = frame.Id;
                            GD.Frames.Remove(frame);
                            GD.Frames.Add(conc);
                        }
                        else
                        {
                            Concrete conc = new Concrete(frame.StartNode, node);
                            conc.Id = frame.Id;
                            GD.Frames.Remove(frame);
                            GD.Frames.Add(conc);
                        }
                    }
                }
            }
            //SortAndNumberNodeList(ref listofnodes);
            //GD.Clear();
            //GD.Nodes = listofnodes;
            //Engine.GenerateFramesByNodes(GD);
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

        private static void SortAndNumberNodeList(ref List<Node> list)
        {

            //list = list.DistinctBy(x => x.Coord).ToList();
            list = DistinctConcrete2(list);
            List<Node> orderedlist = new List<Node>();
            orderedlist = list.OrderBy(x => x.Coord.y).ToList();

            int id = 0;
            foreach (var item in orderedlist)
            {
                id++;
                item.Id = id;
            }

            list = orderedlist;
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

        private static void Segmenter(List<Node> nodes, List<Node> steels, Segment seg, float val)
        {
            int c = steels.Count;
            for (int i = 1; i < c; i++)
            {
                if (steels[i-1].Coord.DistTo(steels[i].Coord) > 2 * val)
                {
                    //Node node = nodes.First(x => x.Coord.DistTo(steels[i - 1].Coord) < 2 * val && x.Coord != steels[i-1].Coord && x.Coord != steels[i].Coord);
                    //Line line = new Line(steels[i - 1].Coord, steels[i].Coord);
                    Node node = nodes.FirstOrDefault(x => x.Coord.DistTo(steels[i - 1].Coord) < 2 * val &&  x.Coord.DistTo(steels[i].Coord) < 2 * val);
                    if (node!= null)
                    {
                        node.Material = eMaterial.Steel;
                        node.RebarCount = seg.Count;
                        node.RebarSize = seg.Size;
                        node.SegmentIDList.Add(seg.ID);
                    }
                }
            }
        }
    }
}
