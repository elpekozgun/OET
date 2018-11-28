using OET_Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OET_Types.Entities
{
    [Serializable()]
    public class Area : IEntity
    {
        public Area(List<PointF> pts)
        {
            _points     = pts;
            _thickness  = 20;
            _color      = Color.FromArgb(160, 204, 230, 255);
        }

        private List<PointF>    _points;
        private bool            _selected;
        private int             _ID;
        private float           _thickness;
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
        public float Thickness
        {
            get
            {
                return _thickness;
            }

            set
            {
                _thickness = value;
            }
        }
        public eEntityType EntityType
        {
            get
            {
                return eEntityType.area;
            }
        }

        public float AreaEntity
        {
            get
            {
                Points.Add(Points[0]);
                return Math.Abs(Points.Take(Points.Count - 1).Select((p, i) => (p.X * Points[i + 1].Y) - (p.Y * Points[i + 1].X)).Sum() / 2);
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
