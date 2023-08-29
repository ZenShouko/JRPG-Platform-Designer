using JRPG_Platform_Designer.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace JRPG_Platform_Designer
{
    public class Tile
    {
        public string Type { get; set; }
        public Coordinates Position { get; set; } = new Coordinates();
        public Brush TileColor { get; set; }
        public bool IsWalkable { get; set; }

        //TODO: Add MapFoe
        public MapPlayer Player { get; set; }
        public MapFoe Foe { get; set; }
        public string TypeLootbox { get; set; }


        [JsonIgnore]
        public Border TileElement { get; set; }
    }
}
