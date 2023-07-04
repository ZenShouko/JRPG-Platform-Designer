using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Platform_Designer.Entities
{
    public class lootbox
    {
        public string Type { get; set; }
        public int CollectableOdd { get; set; }
        public int EquipableOdd { get; set; }
        public int CommonOdd { get; set; }
        public int SpecialOdd { get; set; }
        public int CursedOdd { get; set; }
        public int LegendaryOdd { get; set; }
    }
}
