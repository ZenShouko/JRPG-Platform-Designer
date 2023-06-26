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

namespace JRPG_Platform_Designer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //prepare icons
            CheckIcon.Source = new BitmapImage(new Uri("Icons/check.png", UriKind.Relative));
            AlertIcon.Source = new BitmapImage(new Uri("Icons/alert.png", UriKind.Relative));
            ErrorIcon.Source = new BitmapImage(new Uri("Icons/error.png", UriKind.Relative));

            //Prepare Platform Properties
            EmptyTile.BorderBrush = Brushes.Black;
            EmptyTile.BorderThickness = new Thickness(1);
            EmptyTile.Background = Brushes.LightGray;
            EmptyTile.CornerRadius = new CornerRadius(1);

            //Prepare Data
            InitializeData();
        }
        Image CheckIcon = new Image();
        Image AlertIcon = new Image();
        Image ErrorIcon = new Image();

        Border EmptyTile = new Border();

        List<Tile> AvailableTiles = new List<Tile>() 
        {
            new Tile() { Type = "Empty", Code = "EMT", IsWalkable = false, TileColor = Brushes.LightGray },
        };
        List<Tile> PlatformTiles = new List<Tile>();

        private void InitializeData()
        {
            string json = File.ReadAllText("../../Data/TileList.json");
            foreach (Tile tile in JsonConvert.DeserializeObject<List<Tile>>(json))
            {
                AvailableTiles.Add(tile);
            }

            InitializeGUI();
        }

        private void InitializeGUI()
        {
            foreach (Tile tile in AvailableTiles)
            {
                ComboboxTiles.Items.Add(tile.Type);
            }
            ComboboxTiles.SelectedIndex = 0;
        }

        private void BtnTiles_Click(object sender, RoutedEventArgs e)
        {
            string json = null;
            try
            {
                json = System.IO.File.ReadAllText(GetFilePath());
            }
            catch 
            {
                MessageBox.Show("An error occured while trying to load the file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AvailableTiles = JsonConvert.DeserializeObject<List<Tile>>(json);

            if (AvailableTiles.Count > 0)
            {
                TxtTileStatus.Text = "Tiles loaded 🙂";
            }
            else
            {
                TxtTileStatus.Text = "No tiles found!";
            }
        }

        private string GetFilePath()
        {
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

            //Apply properties
            PlatformProperties.Name = TxtName.Text;
            PlatformProperties.Columns = int.Parse(TxtColumns.Text);
            PlatformProperties.Rows = int.Parse(TxtRows.Text);

            //icon
            PropertiesStatusIcon.Source = CheckIcon.Source;

            //Colapse panel
            CollapsePanel("PlatformPropertiesPanel");

            //Initialize preview grid
            IntializePreviewGrid();
        }

        private void CollapsePanel(string panel)
        {
            PlatformPropertiesPanel.MaxHeight = panel == "PlatformPropertiesPanel" ? 25 : PlatformPropertiesPanel.MaxHeight;
        }

        private void GroupBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Collapse or expand panel
            PlatformPropertiesPanel.MaxHeight = PlatformPropertiesPanel.MaxHeight == 25 ? 999 : 25;
        }

        private void IntializePreviewGrid()
        {
            PlatformPreview.ColumnDefinitions.Clear();
            PlatformPreview.RowDefinitions.Clear();

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

            int currentX = 0;
            int currentY = 0;

            //Add EMT tiles to tilelist
            for (int i = 0; i < PlatformProperties.Columns; i++)
            {
                for (int j = 0; j < PlatformProperties.Rows; j++)
                {
                    Tile tile = new Tile();
                    tile.Type = "Empty";
                    tile.Code = "EMT";
                    tile.IsWalkable = false;
                    tile.TileColor = Brushes.LightGray;
                    tile.Position.X = currentX;
                    tile.Position.Y = currentY;

                    PlatformTiles.Add(tile);

                    currentX++;
                }
                currentY++;
                currentX = 0;
            }

            //Add default tiles to grid
            AddDefaultTilesToGrid();
        }

        private void AddDefaultTilesToGrid()
        {
            foreach(Tile tile in PlatformTiles)
            {
                Border border = new Border();
                border.Background = tile.TileColor;
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                border.CornerRadius = new CornerRadius(1);
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
            border.Background = Brushes.LightYellow;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Apply new tile to cell.
            Border border = (Border)sender;

            //Get tile from list
            Tile newTile = AvailableTiles.Find(x => x.Type == ComboboxTiles.SelectedItem.ToString());

            //Replace current tile with new tile
            Tile currentTile = PlatformTiles.Find(x => x.Position.X == Grid.GetColumn(border) && x.Position.Y == Grid.GetRow(border));
            
            //Assign new tile properties to current tile
            currentTile.Type = newTile.Type;
            currentTile.Code = newTile.Code;
            currentTile.IsWalkable = newTile.IsWalkable;
            currentTile.TileColor = newTile.TileColor;
            PlatformTiles.Add(currentTile);

            border.Background = currentTile.TileColor;
            border.Tag = currentTile.TileColor;
        }

        private void ExportPlatform(object sender, RoutedEventArgs e)
        {
            //Pick a destionation path
            string path = GetDestinationPath();
            if (path == null) { return; }

            //Serialize platformTiles
            string json = JsonConvert.SerializeObject(PlatformTiles);

            //Write json to file
            File.WriteAllText(path, json);

            //Show success message
            MessageBox.Show("Platform exported successfully!");
        }

        private string GetDestinationPath()
        {
            //Open file dialog for user to select a folder
            FileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Jsooon Deruuulooo (*.json)|*json";
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
    }
}
