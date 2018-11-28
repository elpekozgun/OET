using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OET_Types.Entities
{
    public enum eEntityType : byte
    {
        invalid = 0,
        segment = 1,
        area    = 2,
        dot     = 3
    }

    public interface IEntity
    {
        List<PointF>    Points      { get; set; }
        int             ID          { get; set; }
        bool            Selected    { get; set; }
        eEntityType     EntityType  { get; }
        Color           Color       { get; set; }
    }
}
