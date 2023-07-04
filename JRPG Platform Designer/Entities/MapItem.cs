using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JRPG_Platform_Designer.Entities
{
    public class MapItem
    {
        public string ReferenceId { get; set; }

        [JsonIgnore]
        public Image Icon { get; set; }

        public MapItem()
        {
            //Assign own image
            Icon = new Image();
            //Icon.Source = new BitmapImage(new Uri(@"../../Icons/lootbox.png", UriKind.Relative));
        }
    }
}
