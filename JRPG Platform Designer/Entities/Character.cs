using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Platform_Designer.Entities
{
    public class Character
    {
        #region Important Properties
        public int Level { get; set; } = 1;
        public Stats Stats { get; set; } = new Stats();
        #endregion

        public string EquipmentIDs { get; set; }
        public string ID { get; set; }
        public string Name { get; set; } //Unique ... I don't remember what I meant by this
        public string Description { get; set; }
        public string ImageName { get; set; }

        [JsonIgnore]
        public Image CharImage { get; set; }
        public bool IsModified { get; set; } = false;
    }
}
