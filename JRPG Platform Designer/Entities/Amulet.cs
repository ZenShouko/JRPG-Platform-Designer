using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Platform_Designer.Entities
{
    public class Amulet : BaseItem
    {
        public Stats Stats { get; set; } //Stats of the amulet
        public string CharmID { get; set; } //ID of the charm
    }
}
