using JRPG_Platform_Designer.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;

namespace JRPG_Platform_Designer
{
    public static class GameData
    {
        public static List<Tile> AvailableTiles = new List<Tile>(); //List of all available tiles
        public static List<Tile> PlatformTiles = new List<Tile>(); //List of all tiles on the platform
        public static List<Character> FoeList = new List<Character>(); //List of all foes
        public static List<MapFoe> MapFoes = new List<MapFoe>(); //List of all lineups
        

        public static MapPlayer Player = new MapPlayer();

        public static void InitializeData()
        {
            //Clear all lists
            AvailableTiles.Clear();
            PlatformTiles.Clear();
            Player = new MapPlayer();

            //Playericon
            Player.Icon.Source = new BitmapImage(new Uri(@"../../Icons/player.png", UriKind.Relative));

            //Tiles
            ///Reads the tilelist from the json file and adds it to the available tiles list
            string json = File.ReadAllText("../../Data/TileList.json");
            foreach (Tile tile in JsonConvert.DeserializeObject<List<Tile>>(json))
            {
                //Create tile element
                tile.TileElement = new Border();
                tile.TileElement.BorderBrush = Brushes.Black;
                tile.TileElement.BorderThickness = new Thickness(1);
                tile.TileElement.Background = tile.TileColor;
                tile.TileElement.CornerRadius = new CornerRadius(2);
                AvailableTiles.Add(tile);
            }

            //Foes
            ///Reads the foe list from the json file and adds it to the foe list
            json = File.ReadAllText("../../Data/FoeList.json");
            FoeList = JsonConvert.DeserializeObject<List<Character>>(json);
        }

        public static void AddMapFoe(MapFoe mf)
        {
            MapFoes.Add(mf);
        }
    }
}
