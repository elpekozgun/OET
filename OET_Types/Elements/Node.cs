using OET_Math;
using System;
using System.Collections.Generic;

namespace OET_Types.Elements
{
    public enum eNodeLocation : byte
    {
        inside = 0,
        edge = 1,
        outerCorner = 2,
        innerCorner = 3
    }

    [Serializable()]
    public class Node
    {
        #region Ctor

        public Node(XYPT xypt,eMaterial material)
        {
            m_coord     =   xypt;
            m_material  =   material;
            m_segmentIDList = new List<int>();
            m_areaIDList = new List<int>();
        }

        public Node(float x, float y,eMaterial material)
        {
            m_coord.x   =   x;
            m_coord.y   =   y;
            m_material  =   material;
            m_segmentIDList = new List<int>();
            m_areaIDList = new List<int>();
        }

        #endregion

        #region Private Fields

        private int             m_id;
        private float           m_thickness;
        private XYPT            m_coord;
        private byte            m_rebarCount;
        private float           m_rebarSize;
        private eMaterial       m_material;
        private List<int>       m_segmentIDList;
        private List<int>       m_areaIDList;

        #endregion

        #region Public Properties

        public float Thickness
        {
            get
            {
                return m_thickness;
            }

            set
            {
                m_thickness = value;
            }
        }

        public XYPT Coord
        {
            get
            {
                return m_coord;
            }

            set
            {
                m_coord = value;
            }
        }

        public eMaterial Material
        {
            get
            {
                return m_material;
            }

            set
            {
                m_material = value;
            }
        }

        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public byte RebarCount
        {
            get
            {
                if (this.Material == eMaterial.Concrete)
                    return 0;
                else
                    return m_rebarCount;
            }

            set
            {
                m_rebarCount = value;
            }
        }

        public float RebarSize
        {
            get
            {
                if (this.Material == eMaterial.Concrete)
                    return 0;
                else
                return m_rebarSize;
            }

            set
            {
                m_rebarSize = value;
            }
        }

        public float TotalRebarArea
        {
            get
            {
                return (float)(Math.Pow(RebarSize, 2) * RebarCount * Math.PI * 0.25);
            }
        }

        public List<int> SegmentIDList
        {
            get
            {
                return m_segmentIDList;
            }

            set
            {
                m_segmentIDList = value;
            }
        }

        public List<int> AreaIDList
        {
            get
            {
                return m_areaIDList;
            }

            set
            {
                m_areaIDList = value;
            }
        }


        #endregion

    }
}
