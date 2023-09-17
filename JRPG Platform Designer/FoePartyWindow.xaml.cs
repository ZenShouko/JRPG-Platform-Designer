using JRPG_Platform_Designer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace JRPG_Platform_Designer
{
    /// <summary>
    /// Interaction logic for FoePartyWindow.xaml
    /// </summary>
    public partial class FoePartyWindow : Window
    {
        public FoePartyWindow()
        {
            InitializeComponent();
            Initialize();
            RefreshGui();
        }

        public FoePartyWindow(MapFoe mf)
        {
            InitializeComponent();
            LineUp = mf.FoeTeam;
            Initialize();
            RefreshGui();
            BtnCreateParty.Content = "Save Changes";
            Modifying = true;
            MapFoeInQuestion = mf;
        }

        List<Character> LineUp = new List<Character>();
        string MovementBehaviour = "";
        List<string> MovementBehaviours = new List<string>() { "Straight Forward", "Random" };
        Dictionary<string, TextBlock> StatTextblocks = new Dictionary<string, TextBlock>();
        string CurrentlyModifyingStat = "";

        bool Modifying = false;
        MapFoe MapFoeInQuestion = null;

        private void Initialize()
        {
            //Add stat textblocks to dictionary
            StatTextblocks.Add("HP", TxtHp);
            StatTextblocks.Add("DEF", TxtDef);
            StatTextblocks.Add("DMG", TxtDmg);
            StatTextblocks.Add("SPD", TxtSpd);
            StatTextblocks.Add("STA", TxtSta);
            StatTextblocks.Add("STR", TxtStr);
            StatTextblocks.Add("CRC", TxtCrc);
            StatTextblocks.Add("CRD", TxtCrd);

            //Add movement behaviours to combobox
            foreach (string behaviour in MovementBehaviours)
            {
                ComboMovement.Items.Add(behaviour);
            }

            MovementBehaviour = MovementBehaviours[0];
        }

        private void RefreshGui()
        {
            //Refresh combobox
            int index = ComboLineUp.SelectedIndex;
            ComboLineUp.Items.Clear();
            foreach (Character foe in LineUp)
            {
                ComboLineUp.Items.Add($"[{ComboLineUp.Items.Count + 1}] {foe.Name}");
            }

            //Set selected index
            if (index >= 0 && index < LineUp.Count)
                ComboLineUp.SelectedIndex = index;
            else if (ComboLineUp.Items.Count > 0)
                ComboLineUp.SelectedIndex = 0;

            //Button enable/disable. [5 is the limit]
            BtnRemoveFoe.IsEnabled = LineUp.Count > 0;
            BtnAddFoe.IsEnabled = LineUp.Count < 5;
            BtnSwapPosition.IsEnabled = LineUp.Count > 1;

            if (LineUp.Count == 0)
                FoeStatsBox.IsEnabled = false;

            if (LineUp.Count > 0 && LineUp.Count < 5)
                BtnDuplicate.IsEnabled = true;
            else
                BtnDuplicate.IsEnabled = false;

            //Display foes in lineup
            DisplayFoesInLineUp();
        }

        private void DisplayFoesInLineUp()
        {
            //Clear stackpanel
            StackLineUp.Children.Clear();

            //Add foes to stackpanel
            foreach (Character foe in LineUp)
            {
                StackLineUp.Children.Add(GetFoeBorderElement(foe));
            }
        }

        private Border GetFoeBorderElement(Character foe)
        {
            //Border
            Border border = new Border();
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness(1, 1, 2, 2);
            border.CornerRadius = new CornerRadius(2);
            border.Margin = new Thickness(5, 4, 5, 4);
            border.Padding = new Thickness(6, 4, 6, 4);
            border.Background = Brushes.White;
            border.HorizontalAlignment = HorizontalAlignment.Center;
            border.Width = 150;
            //-> Mouse events
            border.MouseUp += (sender, e) => BorderClick(border);
            border.MouseEnter += (sender, e) => border.Background = Brushes.FloralWhite;
            border.MouseLeave += (sender, e) => border.Background = Brushes.White;
            border.Cursor = Cursors.Hand;

            //Stackpanel
            StackPanel stackPanel = new StackPanel();
            border.Child = stackPanel;

            //Name
            TextBlock txtName = new TextBlock();
            txtName.Text = foe.Name;
            txtName.FontSize = 16;
            txtName.FontWeight = FontWeights.Bold;
            stackPanel.Children.Add(txtName);

            //Level
            TextBlock txtLevel = new TextBlock();
            txtLevel.Text = $"Level: {foe.Level}";
            txtLevel.Foreground = Brushes.RoyalBlue;
            txtLevel.FontSize = 12;
            TxtLevel.Margin = new Thickness(2, 0, 0, 0);
            stackPanel.Children.Add(txtLevel);

            return border;
        }

        private void BorderClick(Border border)
        {
            //Get index of border
            int index = StackLineUp.Children.IndexOf(border);

            //Set selected index
            ComboLineUp.SelectedIndex = -1;
            ComboLineUp.SelectedIndex = index;
        }

        private void BtnAddFoe_Click(object sender, RoutedEventArgs e)
        {
            Character foe = new Character();

            //Open foelist window
            FoeListWindow window = new FoeListWindow();
            if (window.ShowDialog() == true)
            {
                //Copy selected foe
                foe.Name = window.SelectedFoe.Name;
                foe.Level = window.SelectedFoe.Level;
                foe.Stats.HP = window.SelectedFoe.Stats.HP;
                foe.Stats.DEF = window.SelectedFoe.Stats.DEF;
                foe.Stats.DMG = window.SelectedFoe.Stats.DMG;
                foe.Stats.SPD = window.SelectedFoe.Stats.SPD;
                foe.Stats.STA = window.SelectedFoe.Stats.STA;
                foe.Stats.STR = window.SelectedFoe.Stats.STR;
                foe.Stats.CRC = window.SelectedFoe.Stats.CRC;
                foe.Stats.CRD = window.SelectedFoe.Stats.CRD;
                foe.ID = window.SelectedFoe.ID;
                foe.Description = window.SelectedFoe.Description;
            }
            else
                return;

            //Add foe to lineup
            LineUp.Add(foe);

            //Refresh gui
            RefreshGui();

            //Set selected index on last foe
            ComboLineUp.SelectedIndex = LineUp.Count - 1;
        }

        private void BtnRemoveFoe_Click(object sender, RoutedEventArgs e)
        {
            //Remove selected foe from lineup
            LineUp.RemoveAt(ComboLineUp.SelectedIndex);
            RefreshGui();
        }

        private void BtnSwapPosition_Click(object sender, RoutedEventArgs e)
        {
            //Throws selected foe to first in lineup
            Character foe = LineUp[ComboLineUp.SelectedIndex];
            LineUp.RemoveAt(ComboLineUp.SelectedIndex);
            LineUp.Insert(0, foe);
            RefreshGui();

            ComboLineUp.SelectedIndex = 0;
        }

        private void Stats_Click(object sender, MouseButtonEventArgs e)
        {
            //Get sender
            TextBlock txt = (TextBlock)sender;

            //Reset fontweight of all textblocks
            foreach (TextBlock textBlock in StatTextblocks.Values)
            {
                textBlock.FontWeight = FontWeights.Normal;
            }

            //Make font bold
            txt.FontWeight = FontWeights.Bold;

            //Get stat
            Character foe = LineUp[ComboLineUp.SelectedIndex];
            if (txt.Name.Contains("Hp"))
                TxtNewValue.Text = foe.Stats.HP.ToString();
            else if (txt.Name.Contains("Def"))
                TxtNewValue.Text = foe.Stats.DEF.ToString();
            else if (txt.Name.Contains("Dmg"))
                TxtNewValue.Text = foe.Stats.DMG.ToString();
            else if (txt.Name.Contains("Spd"))
                TxtNewValue.Text = foe.Stats.SPD.ToString();
            else if (txt.Name.Contains("Sta"))
                TxtNewValue.Text = foe.Stats.STA.ToString();
            else if (txt.Name.Contains("Str"))
                TxtNewValue.Text = foe.Stats.STR.ToString();
            else if (txt.Name.Contains("Crc"))
                TxtNewValue.Text = foe.Stats.CRC.ToString();
            else if (txt.Name.Contains("Crd"))
                TxtNewValue.Text = foe.Stats.CRD.ToString();

            //Enable Stack Modify value
            StackModifyValue.IsEnabled = true;

            //Tell which stat is being modified
            CurrentlyModifyingStat = StatTextblocks.FirstOrDefault(x => x.Value == txt).Key;
            TxtCurrentModifiedStat.Text = $"(Currently Modifying {CurrentlyModifyingStat})";

            //Select all text in textbox
            TxtNewValue.SelectAll();
        }

        private void RefreshFoeStats(Character foe)
        {
            if (foe is null)
            {
                //Reset
                TxtHp.Text = $"x Hp";
                TxtDef.Text = $"x Def";
                TxtDmg.Text = $"x Dmg";
                TxtSpd.Text = $"x Spd";
                TxtSta.Text = $"x Sta";
                TxtStr.Text = $"x Str";
                TxtCrc.Text = $"x Crc";
                TxtCrd.Text = $"x Crd";

                TxtName.Text = "No Foe Selected";
                TxtLevel.Text = "/";
                return;
            }


            TxtHp.Text = $"{foe.Stats.HP} Hp";
            TxtDef.Text = $"{foe.Stats.DEF} Def";
            TxtDmg.Text = $"{foe.Stats.DMG} Dmg";
            TxtSpd.Text = $"{foe.Stats.SPD} Spd";
            TxtSta.Text = $"{foe.Stats.STA} Sta";
            TxtStr.Text = $"{foe.Stats.STR} Str";
            TxtCrc.Text = $"{foe.Stats.CRC} Crc";
            TxtCrd.Text = $"{foe.Stats.CRD} Crd";
            TxtName.Text = foe.Name;
            TxtLevel.Text = $"{foe.Level}";
        }

        private void BtnApplyValue_Click(object sender, RoutedEventArgs e)
        {
            //Did user enter a proper value?
            int newValue = 0;
            try
            {
                newValue = int.Parse(TxtNewValue.Text);
            }
            catch
            {
                MessageBox.Show("Vul een geldig waarde in!");
                return;
            }

            //Apply new value
            Character foe = LineUp[ComboLineUp.SelectedIndex];
            if (CurrentlyModifyingStat == "HP")
                foe.Stats.HP = newValue;
            else if (CurrentlyModifyingStat == "DEF")
                foe.Stats.DEF = newValue;
            else if (CurrentlyModifyingStat == "DMG")
                foe.Stats.DMG = newValue;
            else if (CurrentlyModifyingStat == "SPD")
                foe.Stats.SPD = newValue;
            else if (CurrentlyModifyingStat == "STA")
                foe.Stats.STA = newValue;
            else if (CurrentlyModifyingStat == "STR")
                foe.Stats.STR = newValue;
            else if (CurrentlyModifyingStat == "CRC")
                foe.Stats.CRC = newValue;
            else if (CurrentlyModifyingStat == "CRD")
                foe.Stats.CRD = newValue;

            //Refresh stats
            RefreshFoeStats(foe);
            RefreshGui();

            //Disable stackpanel
            StackModifyValue.IsEnabled = false;

            //Reset fontweight
            StatTextblocks[CurrentlyModifyingStat].FontWeight = FontWeights.Normal;

            //Reset text
            TxtNewValue.Text = "";
            TxtCurrentModifiedStat.Text = "";

            foe.IsModified = true;
        }

        private void ComboLineUp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //return if no foe is selected
            if (ComboLineUp.SelectedIndex == -1)
            {
                RefreshFoeStats(null);
                FoeStatsBox.IsEnabled = false;
                FoeStatsBox.Background = Brushes.LightGray;
                MiscellaneousBox.IsEnabled = false;
                MiscellaneousBox.Background = Brushes.LightGray;
                ComboMovement.SelectedIndex = -1;
                return;
            }

            //Refresh stats
            RefreshFoeStats(LineUp[ComboLineUp.SelectedIndex]);
            FoeStatsBox.IsEnabled = true;
            FoeStatsBox.Background = Brushes.White;
            MiscellaneousBox.IsEnabled = true;
            MiscellaneousBox.Background = Brushes.White;

            ComboMovement.SelectedIndex = MovementBehaviours.IndexOf(MovementBehaviour);
        }

        private void StatTxt_MouseEnter(object sender, MouseEventArgs e)
        {
            //get sender
            TextBlock txt = (TextBlock)sender;

            //make background yellow
            txt.Background = Brushes.NavajoWhite;
        }

        private void StatTxt_MouseLeave(object sender, MouseEventArgs e)
        {
            //get sender
            TextBlock txt = (TextBlock)sender;

            //make background transparent
            txt.Background = Brushes.Transparent;
        }

        private void BtnResetValue_Click(object sender, RoutedEventArgs e)
        {
            //Reset value
            Character foe = LineUp[ComboLineUp.SelectedIndex];
            Character foeOriginal = GameData.FoeList.FirstOrDefault(x => x.ID == foe.ID);

            //Reset value to default
            if (CurrentlyModifyingStat == "HP")
                foe.Stats.HP = foeOriginal.Stats.HP;
            else if (CurrentlyModifyingStat == "DEF")
                foe.Stats.DEF = foeOriginal.Stats.DEF;
            else if (CurrentlyModifyingStat == "DMG")
                foe.Stats.DEF = foeOriginal.Stats.DEF;
            else if (CurrentlyModifyingStat == "SPD")
                foe.Stats.SPD = foeOriginal.Stats.SPD;
            else if (CurrentlyModifyingStat == "STA")
                foe.Stats.STA = foeOriginal.Stats.STA;
            else if (CurrentlyModifyingStat == "STR")
                foe.Stats.STR = foeOriginal.Stats.STR;
            else if (CurrentlyModifyingStat == "CRC")
                foe.Stats.CRC = foeOriginal.Stats.CRC;
            else if (CurrentlyModifyingStat == "CRD")
                foe.Stats.CRD = foeOriginal.Stats.CRD;

            //Refresh stats
            RefreshFoeStats(foe);
            RefreshGui();

            //Disable stackpanel
            StackModifyValue.IsEnabled = false;
            TxtCurrentModifiedStat.Text = "";
        }

        private void ApplyMisschelenious_Click(object sender, RoutedEventArgs e)
        {
            int newValue = 0;
            try
            {
                newValue = int.Parse(TxtLevel.Text);
            }
            catch
            {
                MessageBox.Show("Vul een geldig waarde in!");
                return;
            }

            if (TxtName.Text.Length < 2)
            {
                MessageBox.Show("Vul een geldige naam in!");
                return;
            }

            Character foe = LineUp[ComboLineUp.SelectedIndex];
            foe.Name = TxtName.Text.Trim();
            foe.Level = newValue;

            MovementBehaviour = MovementBehaviours[ComboMovement.SelectedIndex];

            RefreshFoeStats(foe);
            RefreshGui();
        }

        private void BtnCreateParty_Click(object sender, RoutedEventArgs e)
        {
            //Check if party is valid
            if (LineUp.Count < 1)
            {
                MessageBox.Show("Add atleast 1 foe to the line up!");
                return;
            }

            //[->] Check if we're modifying or creating
            if (Modifying)
            {
                ModifyTeam();
                return;
            }
            
            if (TxtTeamName.Text.Trim().Length < 2 || GameData.MapFoes.Any(x => x.Name == TxtTeamName.Text))
            {
                MessageBox.Show("Either invalid team name or already used.");
                return;
            }

            //Create MapFoe
            MapFoe mapFoe = new MapFoe();
            mapFoe.FoeTeam = LineUp;
            mapFoe.MovementBehaviour = MovementBehaviour;
            mapFoe.Name = TxtTeamName.Text.Trim();

            //Add to list
            GameData.AddMapFoe(mapFoe);
            Close();
        }

        private void ModifyTeam()
        {
            //Do we have a valid name?
            if (TxtName.Text.Length > 0 && TxtName.Text.Length < 2)
            {
                MessageBox.Show("Provide a valid team name.");
                return;
            }

            //Apply name changes
            MapFoeInQuestion.Name = TxtName.Text.Length > 0 ? TxtName.Text.Trim() : MapFoeInQuestion.Name;

            //Close. Changes are already made.
            Close();
        }

        private void BtnDuplicate_Click(object sender, RoutedEventArgs e)
        {
            //Get selected foe
            Character foe = LineUp[ComboLineUp.SelectedIndex];

            //Copy foe
            Character newFoe = new Character();
            newFoe.CopyFrom(foe);

            //Add to lineup
            LineUp.Add(newFoe);

            //Refresh gui
            RefreshGui();
        }
    }
}
