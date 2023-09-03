using JRPG_Platform_Designer.Entities;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace JRPG_Platform_Designer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GameData.InitializeData();
            AppPrep();
            InitializeGUI();
        }

        #region Variables
        string CurrentPath = "";

        Image CheckIcon = new Image();
        Image AlertIcon = new Image();
        Image ErrorIcon = new Image();

        public string CurrentAction = "";
        List<Button> ModifyActionButtons = new List<Button>();

        bool IsMouseDown = false;

        #endregion

        #region PlatformProperties
        private void InitializeGUI()
        {
            ///Initializes the GUI
            ///Adds all the tiles to the combobox
            foreach (Tile tile in GameData.TileDictionary.Keys)
            {
                ComboboxTiles.Items.Add(tile.Type);
            }

            ComboboxTiles.SelectedIndex = 1;

            //Add all lootboxes to combobox
            ComboboxLootboxType.Items.Add("COMMON");
            ComboboxLootboxType.Items.Add("SPECIAL");
            ComboboxLootboxType.Items.Add("CURSED");
            ComboboxLootboxType.Items.Add("LEGENDARY");

            //Load all mapfoes in combobox
            ComboboxFoes.Items.Clear();
            foreach (MapFoe mf in GameData.MapFoes)
            {
                ComboboxFoes.Items.Add(mf.Name);
            }

            //Default save directory
            TxtDefaultDirectory.Text = string.IsNullOrEmpty(GameData.DefaultSavePath) ?
                "Current Default Save Directory:\n none" : $"Current Default Save Directory:\n {GameData.DefaultSavePath}";
        }

        private void AppPrep()
        {
            //prepare icons
            CheckIcon.Source = new BitmapImage(new Uri("Icons/check.png", UriKind.Relative));
            AlertIcon.Source = new BitmapImage(new Uri("Icons/alert.png", UriKind.Relative));
            ErrorIcon.Source = new BitmapImage(new Uri("Icons/error.png", UriKind.Relative));

            //Create border elements for all tiles
            for (int i = 0; i < GameData.TileDictionary.Count - 1; i++)
            {
                //Get tile on index i
                Tile tile = GameData.TileDictionary.Keys.ElementAt(i);

                //Create border element
                GameData.TileDictionary[tile] = GetBorder(tile);
            }

            //Add modify action buttons to list
            ModifyActionButtons.Add(BtnModifyTile);
            ModifyActionButtons.Add(BtnModifyLootbox);
            ModifyActionButtons.Add(BtnModifyPlayer);
            ModifyActionButtons.Add(BtnPlaceFoeTeam);

            //Disable all modify action buttons
            foreach (Button button in ModifyActionButtons)
            {
                button.IsEnabled = false;
            }

            //Focus on TxtName
            TxtName.Focus();
            TxtName.CaretIndex = TxtName.Text.Length;

            //Get application version
            TxtVersion.Text = $"Version: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private void ApplyPlatformProperties(object sender, RoutedEventArgs e)
        {
            ///Applies the platform properties
            //Check if all properties are valid
            try
            {
                int i = int.Parse(TxtColumns.Text);
                i = int.Parse(TxtRows.Text);
            }
            catch
            {
                PropertiesStatusIcon.Source = ErrorIcon.Source;
                MessageBox.Show("Column or Row not a valid number.");
                return;
            }

            if (TxtName.Text.Length < 3)
            {
                PropertiesStatusIcon.Source = ErrorIcon.Source;
                MessageBox.Show("Not a valid name given.");
                return;
            }

            //If platform already exists, notify user that it will be overwritten
            if (GameData.PlatformTileDictionary.Count > 1)
            {
                //Did user only change the name?
                if (TxtName.Text != PlatformProperties.Name 
                    && int.Parse(TxtColumns.Text) == PlatformProperties.Columns && int.Parse(TxtRows.Text) == PlatformProperties.Rows)
                {
                    //Update name
                    PlatformProperties.Name = TxtName.Text;
                    CollapsePanel("PlatformPropertiesPanel");
                    return;
                }

                //Else, ask user if he wants to overwrite current platform
                MessageBoxResult result = MessageBox.Show("Platform already exists. Do you want to overwrite it? This will completely reset the platform.", "Overwrite platform", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                    return;
            }

            //=> Apply properties
            PlatformProperties.Name = TxtName.Text;
            PlatformProperties.Columns = int.Parse(TxtColumns.Text);
            PlatformProperties.Rows = int.Parse(TxtRows.Text);

            //Check Icon
            PropertiesStatusIcon.Source = CheckIcon.Source;

            //Colapse panel
            CollapsePanel("PlatformPropertiesPanel");

            //Initialize preview grid
            IntializePreviewGrid();

            //Enable modify action buttons
            foreach (Button button in ModifyActionButtons)
            {
                button.IsEnabled = true;
            }

            //Disable update button
            BtnUpdate.IsEnabled = false;
        }

        private void CollapsePanel(string panel)
        {
            ///Collapses or expands a panel
            PlatformPropertiesPanel.MaxHeight = panel == "PlatformPropertiesPanel" ? 25 : PlatformPropertiesPanel.MaxHeight;
        }

        private void GroupBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Collapse or expand panel
            PlatformPropertiesPanel.MaxHeight = PlatformPropertiesPanel.MaxHeight == 25 ? 999 : 25;
        }

        private void IntializePreviewGrid()
        {
            ///Initializes the preview grid
            //Clear grid
            PlatformPreview.ColumnDefinitions.Clear();
            PlatformPreview.RowDefinitions.Clear();
            PlatformPreview.Children.Clear();

            //Clear lists
            GameData.PlatformTileDictionary.Clear();
            GameData.PlatformLootboxes.Clear();
            GameData.PlatformFoes.Clear();

            //Create columns
            for (int i = 0; i < PlatformProperties.Columns; i++)
            {
                PlatformPreview.ColumnDefinitions.Add(new ColumnDefinition());
            }

            //Create rows
            for (int i = 0; i < PlatformProperties.Rows; i++)
            {
                PlatformPreview.RowDefinitions.Add(new RowDefinition());
            }

            //#Add tiles to platformtiles list
            int currentX = 0;
            int currentY = 0;

            //Add empty tiles to platform tilelist
            Tile referenceTile = GameData.TileDictionary.FirstOrDefault(x => x.Key.Type == "EMPTY").Key;
            for (int i = 0; i < PlatformProperties.Rows; i++)
            {
                for (int j = 0; j < PlatformProperties.Columns; j++)
                {
                    Tile newTile = new Tile();
                    newTile.CopyTile(referenceTile);
                    newTile.Position.X = currentX;
                    newTile.Position.Y = currentY;
                    newTile.TileElement = GetBorder(newTile);

                    //GameData.PlatformTiles.Add(newTile);
                    GameData.PlatformTileDictionary.Add($"{j}X{i}Y", newTile);
                    AddTileElementToGrid(newTile);
                    currentX++;
                }
                currentY++;
                currentX = 0;
            }
        }

        private void ModifyCurrentAction(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Name.Contains("Tile"))
            {
                CurrentAction = "Tile";
            }
            else if (btn.Name.Contains("Player"))
            {
                CurrentAction = "Player";
            }
            else if (btn.Name.Contains("Lootbox"))
            {
                CurrentAction = "Lootbox";
            }
            else if (btn.Name.Contains("PlaceFoe"))
            {
                CurrentAction = "PlaceFoe";
            }

            //Change button colors
            foreach (Button button in ModifyActionButtons)
            {
                if (button.Name.Contains(CurrentAction))
                {
                    button.Background = Brushes.Coral;
                }
                else
                {
                    button.Background = Brushes.LightGray;
                }
            }
        }

        #endregion

        #region Tiles

        private void AddTileElementToGrid(Tile tile)
        {
            //Add border from tile to grid
            Grid.SetColumn(tile.TileElement, tile.Position.X);
            Grid.SetRow(tile.TileElement, tile.Position.Y);
            PlatformPreview.Children.Add(tile.TileElement);
            return;
        }

        private void ReplaceTile(Tile currentTile)
        {
            //Get reference tile
            Tile referenceTile = GameData.TileDictionary.FirstOrDefault(x => x.Key.Type == (string)ComboboxTiles.SelectedValue).Key;

            //Replace tile
            currentTile.CopyTile(referenceTile);
        }

        private void ReplaceTileWith(Tile currentTile, Tile referenceTile)
        {
            //Replace tile
            currentTile.CopyTile(referenceTile);
        }

        private Border GetBorder(Tile tile)
        {
            //Assign default props for the border
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1);
            border.Background = tile.TileColor;
            border.CornerRadius = new CornerRadius(2);
            border.Tag = tile.TileColor;
            border.MouseDown += Border_MouseDown;
            border.MouseUp += Border_MouseUp;
            border.MouseEnter += Border_MouseEnter;
            border.MouseLeave += Border_MouseLeave;
            border.MouseMove += Border_MouseMove;

            return border;
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown) { }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            //Revert back to original colour.
            Border border = (Border)sender;

            Brush tileColor = GameData.PlatformTileDictionary[GetCoordinates(border)].TileColor;

            border.Background = tileColor;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            //Change colour to yellow.
            Border border = (Border)sender;
            border.Background = Brushes.LightCoral;

            //Display tile info
            DisplayTileInfo(border);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            Border border = (Border)sender;

            //What is action?
            switch (CurrentAction)
            {
                case "Tile":
                    {
                        if (e.ChangedButton == MouseButton.Left)
                            ReplaceTile(GameData.PlatformTileDictionary[GetCoordinates(border)]);
                        //else if (e.ChangedButton == MouseButton.Right)
                        //    AssignNewTile(border, true);
                        break;
                    }
                case "Player":
                    {
                        AssignPlayerPosition(border);
                        break;
                    }
                case "Lootbox":
                    {
                        if (e.ChangedButton == MouseButton.Left)
                            AssignLootboxPosition(border, false);
                        else if (e.ChangedButton == MouseButton.Right)
                            AssignLootboxPosition(border, true);
                        break;
                    }
                case "PlaceFoe":
                    {
                        if (e.ChangedButton == MouseButton.Left)
                        {
                            PlaceFoeParty(border);
                        }
                        else if (e.ChangedButton == MouseButton.Right)
                        {
                            RemoveFoe(border);
                        }
                        break;
                    }
            }

            //Display tile info
            DisplayTileInfo(border);
        }

        private void LoadNewTiles(List<Tile> newTileList)
        {
            //[1] Copy tiles
            for (int i = 0; i < newTileList.Count; i++)
            {
                ReplaceTileWith(GameData.PlatformTileDictionary.ElementAt(i).Value, newTileList[i]);
            }

            //[2] Copy player
            AssignPlayerPosition(GameData.PlatformTileDictionary.FirstOrDefault(x => x.Value.Player != null).Value.TileElement);

            //[3] Copy lootboxes
            foreach (Tile tile in GameData.PlatformTileDictionary.Values.Where(x => !string.IsNullOrEmpty(x.TypeLootbox)))
            {
                ComboboxLootboxType.SelectedItem = tile.TypeLootbox;

                //Assign lootbox
                AssignLootboxPosition(tile.TileElement, false);
            }

            //[4] Copy foes
            foreach (Tile tile in GameData.PlatformTileDictionary.Values.Where(x => x.Foe != null))
            {
                //[?] Does foe party exist in linup?
                if (GameData.DoesPartyExist(tile.Foe))
                {
                    //Put combobox on correct value
                    ComboboxFoes.SelectedItem = tile.Foe.Name;

                    //Assign foe party
                    PlaceFoeParty(tile.TileElement);
                    continue;
                }
                else
                {
                    //Create new foe party
                    MapFoe newParty = new MapFoe();
                    newParty.CopyFrom(tile.Foe);

                    //Add to lineup
                    GameData.MapFoes.Add(newParty);

                    //Add to combobox
                    ComboboxFoes.Items.Add(newParty.Name);
                    ComboboxFoes.SelectedIndex = ComboboxFoes.Items.Count - 1;

                    //Assign foe party
                    PlaceFoeParty(tile.TileElement);
                }
            }
        }

        private void BtnFillAllTiles_Click(object sender, RoutedEventArgs e)
        {
            //If empty is selected, replace all tiles with empty tiles
            if (ComboboxTiles.SelectedItem.ToString() == "EMPTY")
            {
                foreach (Tile tile in GameData.PlatformTileDictionary.Values)
                {
                    ReplaceTile(tile);
                }

                return;
            }

            //Loop through all empty tiles
            foreach (Tile tile in GameData.PlatformTileDictionary.Values.Where(x => x.Type == "EMPTY"))
            {
                //Replace tile
                ReplaceTile(tile);
            }
        }

        private void DisplayTileInfo(Border border)
        {
            //Get tile
            Tile selectedTile = GameData.PlatformTileDictionary[GetCoordinates(border)];

            //Display tile info
            if (selectedTile is null)
            {
                TxtTileType.Text = "Tile Type: Void";
                TxtIsWalkable.Text = "Is Walkable: False";
                TxtLootboxInfo.Text = "Lootbox: None";
                TxtFoeInfo.Text = "Foe Party: None";
                return;
            }

            TxtTileType.Text = "Tile Type: " + selectedTile.Type;
            TxtIsWalkable.Text = "Is Walkable: " + selectedTile.IsWalkable;
            TxtLootboxInfo.Text = "Lootbox: " + selectedTile.TypeLootbox;
            TxtFoeInfo.Text = "Foe Party: " + selectedTile.Foe?.Name;
        }

        #endregion

        #region Player

        private void AssignPlayerPosition(Border border)
        {
            //Check if platform has been created
            if (GameData.PlatformTileDictionary.Count == 0)
            {
                MessageBox.Show("Please create a platform first!");
                return;
            }


            //Add player to list
            Tile newTile = GameData.PlatformTileDictionary[GetCoordinates(border)];
            if (newTile is null || !newTile.IsWalkable || newTile.Foe != null || newTile.TypeLootbox != null)
            {
                MessageBox.Show("Placed player on an invalid tile!");
                return;
            }

            //Remove player from the current tile
            if (GameData.PlatformTileDictionary.Any(x => x.Value.Player != null))
                GameData.PlatformTileDictionary.Values.FirstOrDefault(tl => tl.Player != null).Player = null;

            //Remove Image from grid
            if (PlatformPreview.Children.Contains(GameData.Player.Icon))
                PlatformPreview.Children.Remove(GameData.Player.Icon);

            //Add player to new tile
            newTile.Player = GameData.Player;

            //Adjust player positions
            GameData.Player.Position = newTile.Position;

            //Add player to grid
            Grid.SetColumn(GameData.Player.Icon, GameData.Player.Position.X);
            Grid.SetRow(GameData.Player.Icon, GameData.Player.Position.Y);
            PlatformPreview.Children.Add(GameData.Player.Icon);

            //Disable player icon hit test
            GameData.Player.Icon.IsHitTestVisible = false;
        }

        #endregion

        #region Lootbox
        private void AssignLootboxPosition(Border border, bool removeItem)
        {
            Tile selectedTile = GameData.PlatformTileDictionary[GetCoordinates(border)];

            if (removeItem)
            {
                //Return if tile has no lootbox
                if (!GameData.PlatformLootboxes.ContainsKey(GetCoordinates(border)))
                    return;

                //Get image from platformpreview at same position as selected tile with the name "ImgLootbox"
                Image lootboxImage = PlatformPreview.Children.Cast<UIElement>()
                    .Where(x => x is Image)
                    .Where(x => ((Image)x).Name == $"ImgLootbox{GetCoordinates(border)}")
                    .FirstOrDefault() as Image;

                //-Remove lootbox image
                PlatformPreview.Children.Remove(lootboxImage);

                //Remove lootbox from platformlootboxes
                GameData.PlatformLootboxes.Remove(GetCoordinates(border));

                //Remove lootbox from tile
                selectedTile.TypeLootbox = null;
                return;
            }

            //Check for a valid tile
            if (!selectedTile.IsWalkable || selectedTile.Player != null || selectedTile.Foe != null)
            {
                MessageBox.Show("Place lootbox on a valid tile!");
                return;
            }

            //Add lootbox to tile
            selectedTile.TypeLootbox = ComboboxLootboxType.Text;

            //[!] Check if platformlootboxes contains a key with the same position.
            if (GameData.PlatformLootboxes.ContainsKey(GetCoordinates(border)))
                return;

            //Add lootbox image
            Image imgBox = new Image();
            imgBox.Source = new BitmapImage(new Uri(@"../../Icons/lootbox.png", UriKind.Relative));
            imgBox.Name = $"ImgLootbox{GetCoordinates(border)}"; //Add coordinates to easily find the image
            imgBox.IsHitTestVisible = false;

            Grid.SetColumn(imgBox, Grid.GetColumn(border));
            Grid.SetRow(imgBox, Grid.GetRow(border));
            PlatformPreview.Children.Add(imgBox);

            //Add lootbox to dictionary
            GameData.PlatformLootboxes.Add(GetCoordinates(border), selectedTile.TypeLootbox);
        }

        #endregion

        #region Foes
        private void BtnCreateNewFoeParty_Click(object sender, RoutedEventArgs e)
        {
            //Open new window to allow user to create a new foe party
            FoePartyWindow foePartyWindow = new FoePartyWindow();
            foePartyWindow.ShowDialog();

            //Load all mapfoes in combobox
            ComboboxFoes.Items.Clear();
            foreach (MapFoe mf in GameData.MapFoes)
            {
                ComboboxFoes.Items.Add(mf.Name);
            }

            //Select the last added foe party
            ComboboxFoes.SelectedIndex = ComboboxFoes.Items.Count - 1;
        }

        private void PlaceFoeParty(Border border)
        {
            //If no foe party is selected, return
            if (ComboboxFoes.SelectedIndex == -1) { return; }

            //Create a new copy of the selected foe party
            MapFoe referenceParty = GameData.MapFoes.Find(x => x.Name == ComboboxFoes.SelectedItem.ToString());
            MapFoe mFoe = new MapFoe();
            mFoe.CopyFrom(referenceParty);

            //Get tile
            Tile selectedTile = GameData.PlatformTileDictionary[GetCoordinates(border)];

            //Check if tile is walkable
            if (!selectedTile.IsWalkable)
            {
                MessageBox.Show("You can't place a foe party on a non-walkable tile!");
                return;
            }
            else if (selectedTile.Player != null || selectedTile.TypeLootbox != null)
            {
                MessageBox.Show("Place the foe party on an unoccupied tile!");
                return;
            }

            //Add foe party to tile. Will also replace already existing ones.
            selectedTile.Foe = null;
            selectedTile.Foe = mFoe;

            //Set foe position to tile position
            mFoe.Position.New(selectedTile.Position.X, selectedTile.Position.Y);

            //If tile already contains a foe image, return
            if (GameData.PlatformFoes.ContainsKey(GetCoordinates(border)))
            {
                //[!] Replace foe party in dictionary
                GameData.PlatformFoes[GetCoordinates(border)] = mFoe;
                return;
            }

            //Add foe party image
            mFoe.Icon.Name = $"ImgFoe{GetCoordinates(border)}";
            Grid.SetColumn(mFoe.Icon, selectedTile.Position.X);
            Grid.SetRow(mFoe.Icon, selectedTile.Position.Y);
            PlatformPreview.Children.Add(mFoe.Icon);
            mFoe.Icon.IsHitTestVisible = false;

            //Add foe party to dictionary
            GameData.PlatformFoes.Add(GetCoordinates(border), mFoe);
        }

        private void RemoveFoe(Border border)
        {
            //Return if tile doesn't contain foe
            if (!GameData.PlatformFoes.ContainsKey(GetCoordinates(border)))
                return;

            //Remove foe from tile
            GameData.PlatformFoes.Remove(GetCoordinates(border));
            GameData.PlatformTileDictionary[GetCoordinates(border)].Foe = null;

            //Get image from platformpreview at same position as selected tile with the name "ImgFoe"
            Image foeImage = PlatformPreview.Children
                .Cast<UIElement>()
                .Where(x => x is Image)
                .Where(x => ((Image)x).Name == $"ImgFoe{GetCoordinates(border)}")
                .FirstOrDefault() as Image;
            PlatformPreview.Children.Remove(foeImage);
        }

        private void BtnModifyFoeParty_Click(object sender, RoutedEventArgs e)
        {
            if (ComboboxFoes.SelectedIndex == -1)
            {
                MessageBox.Show("No foe party selected.", "Error");
                return;
            }

            //Get foe party and send to foe party window
            MapFoe selectedFoe = GameData.MapFoes.Find(x => x.Name == ComboboxFoes.SelectedItem.ToString());
            FoePartyWindow foePartyWindow = new FoePartyWindow(selectedFoe);
            foePartyWindow.ShowDialog();
        }

        private void BtnDeleteFoeParty_Click(object sender, RoutedEventArgs e)
        {
            //If no foe party is selected, return
            if (ComboboxFoes.SelectedIndex == -1) { return; }

            //Ask user if they are sure they want to delete the foe party
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this foe party?", "Delete Foe Party", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) { return; }

            //Get selected foe party
            MapFoe selectedFoe = GameData.MapFoes.Find(x => x.Name == ComboboxFoes.SelectedItem.ToString());

            //Remove foe party from list
            GameData.MapFoes.Remove(selectedFoe);

            //Remove foe party from combobox
            ComboboxFoes.Items.Remove(selectedFoe.Name);
        }

        #endregion

        #region File

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            //Check if platform is valid
            if (!GameData.IsPlatformValid())
            {
                MessageBox.Show("Platform is not valid. Make sure the platform has atleast 1 tile, no empty tiles, and contains the player.", "Error"
                    , MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            //Pick a destionation path
            string path = GetDestinationPath();
            if (path == null) { return; }

            //settings
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            //Export
            ExportPlatform(path);
        }

        private void ExportPlatform(string path)
        {
            //Get a list of all platform tiles
            List<Tile> platformTiles = GameData.PlatformTileDictionary.Values.ToList();

            //Serialize platformTiles
            string json = JsonConvert.SerializeObject(platformTiles);

            //Write json to file
            File.WriteAllText(path, json);

            //Show success message
            MessageBox.Show("Platform exported successfully!", "Succes :)");
        }

        private void LoadPlatform_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Pick a file
                string filePath = "";
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Jasooon Deruuulooo (*.json)|*json";
                dialog.Title = "Select a file";

                if (!string.IsNullOrEmpty(GameData.DefaultSavePath))
                    dialog.InitialDirectory = GameData.DefaultSavePath;
                else
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                //Open dialog
                if (dialog.ShowDialog() == true)
                    filePath = dialog.FileName;
                else
                    return;

                //Save path
                this.CurrentPath = filePath;

                //Read json from file
                string json = File.ReadAllText(filePath);

                //Deserialize json
                List<Tile> tileList = JsonConvert.DeserializeObject<List<Tile>>(json);

                //Set platform properties
                TxtName.Text = Path.GetFileName(filePath).Split('.')[0];
                TxtColumns.Text = tileList.Max(x => x.Position.X + 1).ToString();
                TxtRows.Text = tileList.Max(x => x.Position.Y + 1).ToString();

                //Load platform
                ApplyPlatformProperties(sender, e);

                //Set tiles
                LoadNewTiles(tileList);

                //Show success message
                MessageBox.Show("Platform loaded successfully!", "Succes :)");

                //Enable update button
                BtnUpdate.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading the platform. Aborting operation.", "Error");
                Console.WriteLine(ex.Message);
            }
        }

        private string GetDestinationPath()
        {
            //Open file dialog for user to select a folder
            FileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Jasooon Deruuulooo (*.json)|*json";
            //Add json file extension
            dialog.FileName = PlatformProperties.Name + ".json";
            dialog.Title = "Select a folder";

            if (GameData.DefaultSavePath != null)
                dialog.InitialDirectory = GameData.DefaultSavePath;
            else
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //Open dialog
            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            //Just restart the app lol >:-)
            Close();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!GameData.IsPlatformValid())
            {
                System.Windows.MessageBox.Show("Platform is not valid. Make sure the platform has atleast 1 tile, no empty tiles, and contains the player.", "Error"
                                       , MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }

            //save current platform to FilePath
            ExportPlatform(CurrentPath);
        }

        private void BtnDefaultDirectory(object sender, RoutedEventArgs e)
        {
            //Open file dialog for user to select a folder
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();

            //Get dialog result
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            //Open dialog
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Save path
                GameData.AssignNewDefaultDirectory(dialog.SelectedPath);
            }

            //Default save directory
            TxtDefaultDirectory.Text = string.IsNullOrEmpty(GameData.DefaultSavePath) ?
                "Current Default Save Directory:\n none" : $"Current Default Save Directory:\n {GameData.DefaultSavePath}";
        }

        #endregion

        private string GetCoordinates(Border border)
        {
            return $"{Grid.GetColumn(border)}X{Grid.GetRow(border)}Y";
        }

        private void TxtBox_Focus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
            //textBox.CaretIndex = textBox.Text.Length;
        }

        private void Gitlink_Click(object sender, MouseButtonEventArgs e)
        {
            //Open github page
            System.Diagnostics.Process.Start("https://github.com/ZenShouko/JRPG-Platform-Designer");

            //Set link color to purple
            ((TextBlock)sender).Foreground = new SolidColorBrush(Color.FromRgb(128, 0, 128));
        }
    }
}
