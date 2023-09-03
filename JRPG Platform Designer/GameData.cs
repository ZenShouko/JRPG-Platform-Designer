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
using System.Collections;

namespace JRPG_Platform_Designer
{
    public static class GameData
    {
        /// <summary>
        /// Default save directory.
        /// </summary>
        public static string DefaultSavePath { get; set; }
        public static string RootDirectoryPath = "";

        /// <summary>
        /// Contains all the possible tiles. Read from the data file.
        /// <para>Used as reference to copy from.</para>
        /// </summary>
        public static Dictionary<Tile, Border> TileDictionary = new Dictionary<Tile, Border>();

        /// <summary>
        /// List of all the foe characters. Read from the data file.
        /// </summary>
        public static List<Character> FoeList = new List<Character>();

        /// <summary>
        /// Contains all the tiles on the platform. Keys= Coordinates [0;0]
        /// </summary>
        public static Dictionary<string, Tile> PlatformTileDictionary = new Dictionary<string, Tile>();

        /// <summary>
        /// Keeps track of all the lootboxes on the platform. Keys= Coordinates [0;0]
        /// </summary>
        public static Dictionary<string, string> PlatformLootboxes = new Dictionary<string, string>();

        /// <summary>
        /// Keeps track of all the foes on the platform. Keys= Coordinates [0;0]
        /// </summary>
        public static Dictionary<string, MapFoe> PlatformFoes = new Dictionary<string, MapFoe>();

        /// <summary>
        /// All the user created lineups. Used as reference to copy from when adding to the platform.
        /// </summary>
        public static List<MapFoe> MapFoes { get; private set; } = new List<MapFoe>();
        

        public static MapPlayer Player = new MapPlayer();

        public static void InitializeData()
        {
            //Assign root directory path
            RootDirectoryPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            //Read default save path file if it exists
            string path = Path.Combine(RootDirectoryPath + "/Data/DefaultSavePath.txt");
            if (File.Exists(path))
            {
                //Read the path
                DefaultSavePath = File.ReadAllText(path);

                //Check if default save path exists
                if (!Directory.Exists(DefaultSavePath))
                {
                    //Remove default save path
                    DefaultSavePath = "";
                    File.Delete(path);
                }
            }

            //Clear all lists
            PlatformTileDictionary.Clear();
            Player.Position.X = 0;
            Player.Position.Y = 0;

            //Tiles
            ///Reads the tilelist from the json file and adds it to the available tiles list
            string json = File.ReadAllText("../../Data/TileList.json");
            foreach (Tile tile in JsonConvert.DeserializeObject<List<Tile>>(json))
            {
                //Create tile element
                tile.TileElement = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Background = tile.TileColor,
                    CornerRadius = new CornerRadius(2)
                };
                TileDictionary.Add(tile, null);
            }

            //Foes
            ///Reads the foe list from the json file and adds it to the foe list
            json = File.ReadAllText("../../Data/FoeList.json");
            FoeList = JsonConvert.DeserializeObject<List<Character>>(json);

            //[Extra] Create a generice foe team
            MapFoe genericFoe = new MapFoe();
            genericFoe.Name = "Generic Team";
            genericFoe.IconNames = "foe-neutral.png";
            genericFoe.MovementBehaviour = "Straight Forward";

            //Add 3 characters
            Character character = new Character();
            character.CopyFrom(FoeList[0]);
            genericFoe.FoeTeam.Add(character);

            character = new Character();
            character.CopyFrom(FoeList[1]);
            genericFoe.FoeTeam.Add(character);

            character = new Character();
            character.CopyFrom(FoeList[0]);
            genericFoe.FoeTeam.Add(character);

            MapFoes.Add(genericFoe);
        }

        public static void AddMapFoe(MapFoe mf)
        {
            //Cancel if lineup already exists
            if (DoesPartyExist(mf))
                return;

            MapFoes.Add(mf);
        }

        /// <summary>
        /// Checks lineup to see if party exists.
        /// </summary>
        public static bool DoesPartyExist(MapFoe reference)
        {
            foreach (MapFoe lineup in GameData.MapFoes)
            {
                if (lineup.Compare(reference))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPlatformValid()
        {
            //Check if platform has atleast 1 tile
            if (PlatformTileDictionary.Count < 1)
                return false;

            //Check if platform does not contain empty tiles
            if (PlatformTileDictionary.Values.Any(x => x.Type == "EMPTY"))
                return false;

            //Check if platform has 1 player
            if (PlatformTileDictionary.Values.All(x => x.Player == null))
                return false;

            return true;
        }

        public static void AssignNewDefaultDirectory(string defaultPath)
        {
            //Assign new default path
            DefaultSavePath = defaultPath;

            //Create a hidden text file to store the default path
            string path = Path.Combine(RootDirectoryPath + "/Data/");
            File.WriteAllText(path + "DefaultSavePath.txt", defaultPath);

            //Open the file
            //System.Diagnostics.Process.Start(Path.Combine(RootDirectoryPath + "/Data/DefaultSavePath.txt"));
        }
    }
}
