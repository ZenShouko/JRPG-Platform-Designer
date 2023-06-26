using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Platform_Designer.Entities
{
    internal class MapFoe : MapObject
    {
        public string IconNames { get; set; }
        public bool HasDetectedPlayer { get; set; }
        public string MovementBehavior { get; set; }
    }
}
