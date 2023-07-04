using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Platform_Designer.Entities
{
    public class Stats
    {
        public int HP { get; set; } //Health Points
        public int DMG { get; set; } //Damage
        public int DEF { get; set; } //Defense
        public int SPD { get; set; } //Speed
        public int CRC { get; set; } //Critical Hit Chance
        public int CRD { get; set; } //Critical Hit Damage
        public int STA { get; set; } //Stamina
        public int STR { get; set; } //Stamina Regeneration
        public int XP { get; set; } //Experience Points
    }
}
