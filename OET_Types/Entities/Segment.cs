using OET_Math;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OET_Types.Entities
{
    [Serializable()]
    public class Segment : IEntity
    {
        public Segment(List<PointF> pts)
        {
            _points = pts;
            _count  = 3;
            _size   = 10;
            _color  = Color.FromArgb(255, 252, 207, 59);
        }

        private List<PointF>    _points;
        private bool            _selected;
        private int             _ID;
        private byte            _count;
        private float           _size;
        private Color           _color;

        #region Public Properties

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
        public eEntityType EntityType
        {
            get
            {
                return eEntityType.segment;
            }
        }
        public byte Count
        {
            get
            {
                return _count;
            }

            set
            {
                _count = value;
            }
        }
        public float Size
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;
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
