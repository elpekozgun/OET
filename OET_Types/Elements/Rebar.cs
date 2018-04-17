using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OET_Math;

namespace OET_Types.Elements
{
    [Serializable()]
    public class Rebar : IElement 
    {
        #region Ctor

        public Rebar(Node startNode, Node endNode)
        {
            m_startNode   =   startNode;
            m_endNode     =   endNode;
        }

        public Rebar(Node startNode, Node endNode,byte rebarcount, float rebarsize)
        {
            m_startNode   =   startNode;
            m_endNode     =   endNode;
            m_count     =   rebarcount;
            m_size      =   rebarsize;
        }

        #endregion

        #region private Fields

        private int         m_id;
        private Node        m_startNode;
        private Node        m_endNode;
        private byte        m_count;
        private float       m_size;
        
        #endregion

        #region Public Properties

        public byte Count
        {
            get
            {
                return m_count;
            }

            set
            {
                m_count = value;
            }
        }

        public float Size
        {
            get
            {
                return m_size;
            }

            set
            {
                m_size = value;
            }
        }

        public float SteelArea
        {
            get { return m_count *  m_size * m_size * (float)Math.PI / 4; }
        }

        #endregion

        #region IElement Implementation

        public Node StartNode
        {
            get{ return m_startNode; }
            set{ m_startNode = value; }
        }

        public Node EndNode
        {
            get { return m_endNode; }
            set { m_endNode = value; }
        }

        public eMaterial Material
        {
            get { return eMaterial.Steel; }
        }

        public float Length
        {
            get { return (float)m_startNode.Coord.DistTo(m_endNode.Coord); }
        }

        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        public IElement Clone()
        {
            return new Rebar(this.m_startNode,this.m_endNode,this.m_count,this.m_size);
        }

        #endregion

    }
}
