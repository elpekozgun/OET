using OET_Math;
using System;

namespace OET_Types.Elements
{
    [Serializable()]
    public class Concrete : IElement
    {
        #region Ctor

        public Concrete(Node startNode, Node EndNode)
        {
            m_startNode   =   startNode;
            m_endNode     =   EndNode;
        }

        #endregion

        #region Private Fields

        private int         m_id;
        private Node        m_startNode;
        private Node        m_endNode;

        #endregion

        #region ICountable Implementation

        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        #endregion

        #region IElement Implementation

        public Node StartNode
        {
            get { return m_startNode; }
            set { m_startNode = value; }
        }

        public Node EndNode
        {
            get { return m_endNode; }
            set { m_endNode = value; }
        }

        public float Length
        {
            get { return (float)m_startNode.Coord.DistTo(m_endNode.Coord); }
        }

        public eMaterial Material
        {
            get { return eMaterial.Concrete; }

        }

        public IElement Clone()
        {
            return new Concrete(this.m_startNode, this.m_endNode);
        }

        #endregion
    }
}
