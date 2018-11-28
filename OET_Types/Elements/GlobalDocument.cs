using OET_Types.Entities;
using OET_Types.Elements;
using OET_Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

public enum eUnit : byte
{
    kN_m = 0,
    N_mm = 1,
}

public enum eBoundary : byte
{
    fix = 0,
    moving = 1
}

namespace OET_Types.Elements
{
    [Serializable()]
    public class GlobalDocument : ISerializable
    {
        #region Ctor

        public GlobalDocument()
        {
            _nod            = 0.5f;                     
            _gridSize       = 10;
            _meshSize       = 10f;
            _horizon        = 14.2f;
            _gridWidth      = 100;
            _gridHeight     = 100;
            _unitType       = eUnit.kN_m;

            _listOfNodes    = new List<Node>();
            _listOfFrames   = new List<IElement>();
            _listOfEntities = new List<IEntity>();
            _bigPolygon     = new Polygon();
        }
        //serialize Constructur
        public GlobalDocument(SerializationInfo info, StreamingContext context)
        {
            _nod            = (float)info.GetValue("nod", typeof(float));
            _gridSize       = (int)info.GetValue("gridSize", typeof(int));
            _meshSize       = (float)info.GetValue("meshSize", typeof(float));
            _horizon        = (float)info.GetValue("horizon", typeof(float));
            _gridWidth      = (float)info.GetValue("gridWidth", typeof(float));
            _gridHeight     = (float)info.GetValue("gridHeight", typeof(float));
            _fileName       = (string)info.GetValue("filename", typeof(string));
            _folderName     = (string)info.GetValue("foldername", typeof(string));
            _exportFolder   = (string)info.GetValue("exportfolder", typeof(string));

            _listOfNodes = (List<Node>)info.GetValue("nodes", typeof(List<Node>));
            _listOfFrames   = (List<IElement>)info.GetValue("frames", typeof(List<IElement>));
            _listOfEntities = (List<IEntity>)info.GetValue("entities", typeof(List<IEntity>));
        }
        
        #endregion
        
        #region private Fields

        private float           _nod;
        private int             _gridSize;
        private float           _meshSize;
        private float           _horizon;
        private float           _gridWidth ;
        private float           _gridHeight;
        private float           _increment;
        private eBoundary       _boundaryType;
        private eUnit           _unitType;
        
        private List<IEntity>   _listOfEntities;
        private List<Node>      _listOfNodes;
        private List<IElement>  _listOfFrames;
        private Polygon         _bigPolygon;

        private string          _fileName;
        private string          _folderName;
        private string          _exportFolder;

        #endregion
        
        #region Public Properties

        /// <summary>
        /// circle size used for drawing end points
        /// </summary>
        public float            Nod
        {
            get
            {
                return _nod;
            }

            set
            {
                _nod = value;
            }
        }

        public int              GridSize
        {
            get
            {
                return _gridSize;
            }

            set
            {
                _gridSize = value;
            }
        }
        public float            MeshSize
        {
            get
            {
                return _meshSize;
            }

            set
            {
                _meshSize = value;
            }
        }
        public float            Horizon
        {
            get
            {
                return _horizon;
            }

            set
            {
                _horizon = value;
            }
        }
        public float            GridWidth
        {
            get
            {
                return _gridWidth;
            }

            set
            {
                _gridWidth = value;
            }
        }
        public float            GridHeight
        {
            get
            {
                return _gridHeight;
            }

            set
            {
                _gridHeight = value;
            }
        }
        public float            Increment
        {
            get
            {
                return _increment;
            }

            set
            {
                _increment = value;
            }
        }
        public eBoundary        BoundaryType
        {
            get
            {
                return _boundaryType;
            }

            set
            {
                _boundaryType = value;
            }
        }
        public float            TotalArea
        {
            get
            {
                float a = 0;
                foreach (Area entity in Entities.FindAll(x=>x.EntityType == eEntityType.area))
                {
                    a += entity.AreaEntity;
                }
                return a;
            }
        }

        public List<Node>       Nodes
        {
            get
            {
                return _listOfNodes;
            }

            set
            {
                _listOfNodes = value;
            }
        }
        public List<IElement>   Frames
        {
            get
            {
                return _listOfFrames;
            }

            set
            {
                _listOfFrames = value;
            }
        }
        public List<IEntity>    Entities
        {
            get
            {
                return _listOfEntities;
            }

            set
            {
                _listOfEntities = value;
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }

            set
            {
                _fileName = value;
            }
        }
        public string FolderName
        {
            get
            {
                return _folderName;
            }

            set
            {
                _folderName = value;
            }
        }
        public string ExportFolder
        {
            get
            {
                return _exportFolder;
            }

            set
            {
                _exportFolder = value;
            }
        }
        public string FileNameWithoutPath
        {
            get
            {
                return (Path.GetFileNameWithoutExtension(_fileName));
            }
        }
        public eUnit Unit
        {
            get
            {
                return _unitType;
            }

            set
            {
                _unitType = value;
            }
        }

        public double Nmm
        {
            get
            {
                if (Unit == eUnit.kN_m)
                    return 0.001;
                else if (Unit == eUnit.N_mm)
                    return 1;
                else
                    return 1;
            }
        }

        #endregion

        #region Public Methods

        public void Clear()
        {
            Nodes.Clear();
            Frames.Clear();
        }

        //export Region

        public void GenerateInputMesh()
        {
            List<string> mesh = new List<string>();

            string firstline = Convert.ToString(Nodes.Count) + " " + 
                               Convert.ToString(Frames.Count) + " " +
                               Convert.ToString((double)(TotalArea * Nmm * Nmm)) + " " + 
                               Convert.ToString((double)(MeshSize * Nmm));

            // number of nodes, number of elements, area of a default member, segment (dx);
            mesh.Add(firstline);

            // node id, node.x , node.y 
            for (int i = 0; i < Nodes.Count; i++)
            {
                mesh.Add(Convert.ToString(Nodes[i].Id) + " " + 
                         Convert.ToString((double)(Nodes[i].Coord.x * Nmm)) + " " +
                         Convert.ToString((double)(Nodes[i].Coord.y * Nmm)) + " " + 
                         Convert.ToString(0));
            }

            // frame id, first node, second node, steel relation, steel area, material type
            for (int i = 0; i < Frames.Count; i++)
            {
                mesh.Add(Convert.ToString(Frames[i].Id) + " " +
                         Convert.ToString(Frames[i].StartNode.Id) + " " +
                         Convert.ToString(Frames[i].EndNode.Id) + " " +
                         Convert.ToString(Frames[i].Material == eMaterial.Steel ? 1 : 0) + " " +
                         Convert.ToString(Frames[i].Material == eMaterial.Steel ? (double)(Frames[i].StartNode.TotalRebarArea * Nmm * Nmm) : 0) + " " +
                         Convert.ToString(Frames[i].Material == eMaterial.Steel ? 2 : 1));
            }

            File.WriteAllLines(_exportFolder + "\\"+ FileNameWithoutPath +  @".mesh.inp", mesh);
        }

        public void GenerateInputBoundary()
        {
            var dots = Entities.FindAll(x => x.EntityType == eEntityType.dot);

            List <string> boundary = new List<string>();

            for (int i = 0; i < dots.Count; i++)
            {
                Dot dot = (Dot)dots[i];
                var node1 =  Nodes.Find(x => x.Coord == new XYPT(dot.Points[0].X, dot.Points[0].Y));
                var node2 =  Nodes.Find(x => x.Coord == new XYPT(dot.Points[0].X + GridSize, dot.Points[0].Y));
                var restX = dot.XRestrained ? 0 : 1;
                var restY = dot.YRestrained ? 0 : 1;
                var lastLine = i == dots.Count - 1 ? 0 : 1;
                if (node1 != null && node2 != null)
                {
                boundary.Add(node1.Id.ToString() + " " + 
                             node2.Id.ToString() + " " + 
                             Increment.ToString() + " "
                             + restX + " " + restY + " " + lastLine);
                }
            }
            if (_boundaryType == eBoundary.moving)
            {
                boundary.Add("1");

                // ADD MOVING BOUNDARY CONDITIONS

            }
            else if(_boundaryType == eBoundary.fix)
                boundary.Add("0");

            File.WriteAllLines(_exportFolder + "\\" + FileNameWithoutPath + @".boundary.inp", boundary);

        }

        public void GenerateNodeThicknes()
        {
            List<string> thicknessList = new List<string>();

            for (int i = 0; i < Nodes.Count; i++)
            {
                thicknessList.Add(Convert.ToString(Nodes[i].Id) + " " + Convert.ToString((double)(Nodes[i].Thickness * Nmm)));
            }

            File.WriteAllLines(_exportFolder + "\\" + FileNameWithoutPath + @".ThicknessList.inp", thicknessList);

        }

        public void GenerateGnuPlotData()
        {
            List<string> plotData = new List<string>();
            string colorDat;
            for (int i = 0; i < Frames.Count; i++)
            {
                plotData.Add(Convert.ToString((double)(Frames[i].StartNode.Coord.x * Nmm)) + " " +
                             Convert.ToString((double)(Frames[i].StartNode.Coord.y * Nmm)) + " " + (colorDat = Frames[i].Material == eMaterial.Concrete ? "3" : "7"));
                plotData.Add(Convert.ToString((double)(Frames[i].EndNode.Coord.x * Nmm  )) + " " +                                                               
                             Convert.ToString((double)(Frames[i].EndNode.Coord.y * Nmm  )) + " " + (colorDat = Frames[i].Material == eMaterial.Concrete ? "3" : "7"));
                plotData.Add("\n");
            }

            File.WriteAllLines(_exportFolder + "\\" + FileNameWithoutPath + @".plotData.dat", plotData);

        }

        // Deserialize method
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("nod", Nod);
            info.AddValue("gridSize", GridSize);
            info.AddValue("meshSize", MeshSize);
            info.AddValue("horizon", Horizon);
            info.AddValue("gridWidth", GridWidth);
            info.AddValue("gridHeight", GridHeight);
            info.AddValue("filename", FileName);
            info.AddValue("foldername", FolderName);
            info.AddValue("exportfolder", ExportFolder);
            info.AddValue("unit", Unit);

            info.AddValue("nodes", Nodes);
            info.AddValue("frames", Frames);
            info.AddValue("entities", Entities);

        }

        #endregion

    }
}
