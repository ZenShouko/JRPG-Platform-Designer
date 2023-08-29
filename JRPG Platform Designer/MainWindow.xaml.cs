using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;
using JRPG_Platform_Designer.Entities;

namespace JRPG_Platform_Designer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppPrep();
            GameData.InitializeData();
            InitializeGUI();
        }
        Image CheckIcon = new Image();
        Image AlertIcon = new Image();
        Image ErrorIcon = new Image();
        Tile EmptyTile = new Tile();
        public string CurrentAction = "";
        List<Button> ModifyActionButtons = new List<Button>();
        bool IsMouseDown = false;

        private void InitializeGUI()
        {
            ///Initializes the GUI
            ///Adds all the tiles to the combobox
            foreach (Tile tile in GameData.AvailableTiles)
            {
                ComboboxTiles.Items.Add(tile.Type);
            }
            ComboboxTiles.SelectedIndex = 0;
        }

        private void AppPrep()
        {
            //prepare icons
            CheckIcon.Source = new BitmapImage(new Uri("Icons/check.png", UriKind.Relative));
            AlertIcon.Source = new BitmapImage(new Uri("Icons/alert.png", UriKind.Relative));
            ErrorIcon.Source = new BitmapImage(new Uri("Icons/error.png", UriKind.Relative));

            //Prepare an empty tile
            Border EmptyBorder = new Border();
            EmptyBorder.BorderBrush = Brushes.Black;
            EmptyBorder.BorderThickness = new Thickness(1);
            EmptyBorder.Background = Brushes.LightGray;
            EmptyBorder.CornerRadius = new CornerRadius(2);

            EmptyTile.TileElement = EmptyBorder;
            EmptyTile.Type = "Empty";
            EmptyTile.IsWalkable = false;
            EmptyTile.TileColor = Brushes.LightGray;

            //Player
            GameData.Player.Icon = new Image();
            GameData.Player.Icon.Source = new BitmapImage(new Uri("../../Icons/player.png", UriKind.Relative));

            //Add modify action buttons to list
            ModifyActionButtons.Add(BtnModifyTile);
            ModifyActionButtons.Add(BtnModifyLootbox);
            ModifyActionButtons.Add(BtnModifyPlayer);
        }

        private string GetFilePath()
        {
            ///Deprecated
            ///Opens a file dialog and returns the path of the selected file
            //Open file dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON Files (*.json)|*.json";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Title = "Select a JSON file";

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

            GameData.PlatformTiles.Clear();

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

            //Add EMT tiles to platform tilelist
            for (int i = 0; i < PlatformProperties.Rows; i++)
            {
                for (int j = 0; j < PlatformProperties.Columns; j++)
                {
                    Tile newTile = new Tile();
                    newTile.Type = EmptyTile.Type;
                    newTile.TileColor = EmptyTile.TileColor;
                    newTile.IsWalkable = EmptyTile.IsWalkable;
                    newTile.Position.X = currentX;
                    newTile.Position.Y = currentY;

                    AddEmptyTile(newTile);
                    GameData.PlatformTiles.Add(newTile);
                    currentX++;
                }
                currentY++;
                currentX = 0;
            }

            //Add default tiles to grid
            //AddDefaultTilesToGrid();
        }

        private void AddEmptyTile(Tile tile)
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

            //Assign border to tile
            tile.TileElement = border;

            //Add border to grid
            Grid.SetColumn(tile.TileElement, tile.Position.X);
            Grid.SetRow(tile.TileElement, tile.Position.Y);
            PlatformPreview.Children.Add(tile.TileElement);
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown) { }
        }

        private void AddDefaultTilesToGrid()
        {
            ///Deprecated
            //Adds the empty tiles to the grid. #Starting point
            foreach(Tile tile in GameData.PlatformTiles)
            {
                Border border = tile.TileElement;
                border.Tag = tile.TileColor;
                border.MouseDown += Border_MouseDown;
                border.MouseEnter += Border_MouseEnter;
                border.MouseLeave += Border_MouseLeave;

                Grid.SetColumn(border, tile.Position.X);
                Grid.SetRow(border, tile.Position.Y);
                PlatformPreview.Children.Add(border);
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            //Revert back to original colour.
            Border border = (Border)sender;

            border.Background = border.Tag == Brushes.LightGray ? Brushes.LightGray : (Brush)border.Tag;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            //Change colour to yellow.
            Border border = (Border)sender;
            border.Background = Brushes.LightCoral;
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
                        {
                            AssignNewTile(border, false);
                        }
                        else if (e.ChangedButton == MouseButton.Right)
                        {
                            AssignNewTile(border, true);
                        }
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
                        {
                            AssignLootboxPosition(border, false);
                        }
                        else if (e.ChangedButton == MouseButton.Right)
                        {
                            AssignLootboxPosition(border, true);
                        }
                        break;
                    }
            }
        }

        private void AssignNewTile(Border border, bool addDefaultTile)
        {
            Tile newTile = null;
            if (!addDefaultTile)
            {
                //Get tile from list
                newTile = GameData.AvailableTiles.Find(x => x.Type == ComboboxTiles.SelectedItem.ToString());
            }
            else
            {
                //Get tile from list
                newTile = EmptyTile;
            }

            //Replace current tile with new tile
            Tile currentTile = GameData.PlatformTiles.Find(x => x.Position.X == Grid.GetColumn(border) && x.Position.Y == Grid.GetRow(border));

            //Assign new tile properties to current tile
            currentTile.Type = newTile.Type;
            currentTile.IsWalkable = newTile.IsWalkable;
            currentTile.TileColor = newTile.TileColor;

            //Assign new tile properties to border
            border.Background = currentTile.TileColor;
            border.Tag = currentTile.TileColor;
        }

        private void ExportPlatform(object sender, RoutedEventArgs e)
        {
            //Pick a destionation path
            string path = GetDestinationPath();
            if (path == null) { return; }

            //settings
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            //Serialize platformTiles
            string json = JsonConvert.SerializeObject(GameData.PlatformTiles);

            //Write json to file
            File.WriteAllText(path, json);

            //Show success message
            MessageBox.Show("Platform exported successfully!");
        }

        private string GetDestinationPath()
        {
            //Open file dialog for user to select a folder
            FileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Jasooon Deruuulooo (*.json)|*json";
            //Add json file extension
            dialog.FileName = PlatformProperties.Name + ".json";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Title = "Select a folder";

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

        private void AssignPlayerPosition(Border border)
        {
            //Check if platform has been created
            if (GameData.PlatformTiles.Count == 0)
            {
                MessageBox.Show("Please create a platform first!");
                return;
            }


            //Add player to list
            Tile newTile = GameData.PlatformTiles.Find(t => t.TileElement == border);
            if (newTile is null || !newTile.IsWalkable)
            {
                MessageBox.Show("Placed player on an invalid tile!");
                return;
            }

            //Remove player from the current tile
            Tile oldTile = GameData.PlatformTiles.Find(tl => tl.Player != null);
            if (oldTile != null)
            {
                oldTile.Player = null;
            }

            //Remove old player
            if (PlatformPreview.Children.Contains(GameData.Player.Icon))
            {
                PlatformPreview.Children.Remove(GameData.Player.Icon);
            }

            //Add player to new tile
            newTile.Player = GameData.Player;

            //Adjust player positions
            GameData.Player.Position = newTile.Position;

            //Add player to grid
            Grid.SetColumn(GameData.Player.Icon, GameData.Player.Position.X);
            Grid.SetRow(GameData.Player.Icon, GameData.Player.Position.Y);
            PlatformPreview.Children.Add(GameData.Player.Icon);
        }

        private void AssignLootboxPosition(Border border, bool removeItem)
        {
            Tile selectedTile = GameData.PlatformTiles.Find(t => t.TileElement == border);
            
            if (removeItem)
            {
                //Return if tile has no lootbox
                if (string.IsNullOrEmpty(selectedTile.TypeLootbox)) { return; }

                //Get index of lootbox image
                int index = PlatformPreview.Children.IndexOf(PlatformPreview.Children.Cast<UIElement>().Where(x => x is Image).Where(x => ((Image)x).Tag == selectedTile).FirstOrDefault());

                //Remove lootbox image
                PlatformPreview.Children.RemoveAt(index);
                selectedTile.TypeLootbox = null;
                return;
            }

            //Add lootbox to tile
            selectedTile.TypeLootbox = ComboboxLootboxType.Text;

            //If tile already contains a lootbox image, return
            if (PlatformPreview.Children.Cast<UIElement>().Where(x => x is Image).Where(x => ((Image)x).Tag == selectedTile).FirstOrDefault() != null) { return; }
            
            //Add lootbox image
            Image imgBox = new Image();
            imgBox.Source = new BitmapImage(new Uri(@"../../Icons/lootbox.png", UriKind.Relative));
            imgBox.Tag = selectedTile;
            imgBox.IsHitTestVisible = false;

            Grid.SetColumn(imgBox, selectedTile.Position.X);
            Grid.SetRow(imgBox, selectedTile.Position.Y);
            PlatformPreview.Children.Add(imgBox);
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

        private void BtnFillAllTiles_Click(object sender, RoutedEventArgs e)
        {
            //Fill all empty tiles with selected tile
            Tile tile = GameData.AvailableTiles.Find(x => x.Type == ComboboxTiles.SelectedItem.ToString());

            //Loop through all tiles
            foreach (Border border in PlatformPreview.Children.Cast<UIElement>().Where(x => x is Border))
            {
                if (border.Tag == EmptyTile.TileColor)
                {
                    AssignNewTile(border, false);
                }
            }
        }

        private void BtnCreateNewFoeParty_Click(object sender, RoutedEventArgs e)
        {
            //Open new window to allow user to create a new foe party
            FoePartyWindow foePartyWindow = new FoePartyWindow();
            foePartyWindow.ShowDialog();

            //Load all mapfoes in combobox
            ComboboxFoes.Items.Clear();
            foreach (MapFoe mf in GameData.MapFoes)
            {
                ComboboxFoes.Items.Add($"Team []");
            }
        }
    }
}
