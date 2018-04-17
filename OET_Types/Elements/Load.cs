using System;

namespace OET_Types.Elements
{
    [Serializable()]
    public class Load 
    {
        #region Private Fields

        private int m_id;

        #endregion

        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

    }
}
