using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Platform_Designer.Entities
{
    public abstract class BaseItem
    {
        public string ID { get; set; }
        public int Level { get; set; } = 1; //Level of the item
        public string Rarity { get; set; } //Rarity of the item
        public string Name { get; set; }
        public string Description { get; set; } //Description of the item
        public string ImageName { get; set; } //Name of the image, used when deserializing
        public int Value { get; set; } //Value of the item

        [JsonIgnore]
        public Image ItemImage { get; set; } //Image of the item
    }
}
