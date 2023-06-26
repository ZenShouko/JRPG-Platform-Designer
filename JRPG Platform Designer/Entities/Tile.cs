using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace JRPG_Platform_Designer
{
    internal class Tile
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public Coordinates Position { get; set; } = new Coordinates();
        public Brush TileColor { get; set; }
        public Border TileElement { get; set; }
        public bool IsWalkable { get; set; }

        //TODO: Add MapPlayer, MapItem and MapFoe
    }
}
