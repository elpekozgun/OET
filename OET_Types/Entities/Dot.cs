using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OET_Types.Entities
{
    [Serializable()]
    public class Dot : IEntity
    {
        public Dot(PointF point)
        {
            _points = new List<PointF>() { point };
            _color = Color.Green;
            _loadX = 0;
            _loadY = 0;
            _xRestrained = false;
            _yRestrained = false;
        }

        private List<PointF>    _points;
        private bool            _selected;
        private int             _ID;
        private float           _loadX;
        private float           _loadY;
        private bool            _xRestrained;
        private bool            _yRestrained;
        private Color           _color;

        #region Public Properties

        public eEntityType entityType
        {
            get
            {
                return eEntityType.dot;
            }
        }

        public int ID
        {
            get
            {
                return _ID;
            }

            set
            {
                _ID = value;
            }
        }

        public List<PointF> Points
        {
            get
            {
                return _points;
            }

            set
            {
                _points = value;
            }
        }

        public bool Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;
            }
        }

        public float LoadX
        {
            get
            {
                return _loadX;
            }

            set
            {
                _loadX = value;
            }
        }

        public float LoadY
        {
            get
            {
                return _loadY;
            }

            set
            {
                _loadY = value;
            }
        }

        public bool XRestrained
        {
            get
            {
                return _xRestrained;
            }

            set
            {
                _xRestrained = value;
            }
        }

        public bool YRestrained
        {
            get
            {
                return _yRestrained;
            }

            set
            {
                _yRestrained = value;
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
            }
        }

        #endregion
    }
}
