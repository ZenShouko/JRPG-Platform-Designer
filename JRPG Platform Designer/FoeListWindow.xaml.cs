using JRPG_Platform_Designer.Entities;
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
using System.Windows.Shapes;

namespace JRPG_Platform_Designer
{
    /// <summary>
    /// Interaction logic for FoeListWindow.xaml
    /// </summary>
    public partial class FoeListWindow : Window
    {
        public FoeListWindow()
        {
            InitializeComponent();
            LoadFoes();
        }

        public Character SelectedFoe { get; set; }

        private void LoadFoes()
        {
            //Load all foes from foelist in gamedata
            foreach (Character foe in GameData.FoeList)
            {
                ListBoxItem listItem = new ListBoxItem();
                listItem.Content = foe.Name;
                listItem.Tag = foe.ID;
                FoeListbox.Items.Add(listItem);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Return if no foe is selected
            if (FoeListbox.SelectedIndex == -1)
                return;

            //Set selected foe
            SelectedFoe = GameData.FoeList.Where(x => x.ID == ((ListBoxItem)FoeListbox.SelectedItem).Tag.ToString()).FirstOrDefault();

            //Close window
            DialogResult = true;
            Close();
        }
    }
}
