using JRPG_Platform_Designer.Entities;
using Newtonsoft.Json;
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

        public void CopyTile(Tile referenceTile)
        {
            //[!] Reference tiles don't have coordinates, so don't copy them
            //Copy all properties from reference tile
            Type = referenceTile.Type;
            TileColor = referenceTile.TileColor;
            IsWalkable = referenceTile.IsWalkable;
            TypeLootbox = referenceTile.TypeLootbox;

            //Safely copy player and foe. Remove first, then add.
            if (referenceTile.Player != null)
            {
                referenceTile.Player = null;
                Player = GameData.Player;
            }

            if (referenceTile.Foe != null)
            {
                MapFoe mf = referenceTile.Foe;
                referenceTile.Foe = null;
                Foe = mf;
            }

            //Copy tile element background color
            if (TileElement != null)
                TileElement.Background = TileColor;
        }
    }
}
