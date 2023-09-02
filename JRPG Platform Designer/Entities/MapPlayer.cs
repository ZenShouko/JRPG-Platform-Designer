using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace JRPG_Platform_Designer.Entities
{
    public class MapPlayer : MapObject
    {
        public MapPlayer()
        {
            Icon.Source = new BitmapImage(new Uri(@"../../Icons/player.png", UriKind.Relative));
        }
    }
}
