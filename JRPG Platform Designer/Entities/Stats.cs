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

        public void CopyFrom(Stats reference)
        {
            HP = reference.HP;
            DMG = reference.DMG;
            DEF = reference.DEF;
            SPD = reference.SPD;
            CRC = reference.CRC;
            CRD = reference.CRD;
            STA = reference.STA;
            STR = reference.STR;
            XP = reference.XP;
        }

        /// <summary>
        /// Compares stats to see if they're the same.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public bool Compare(Stats reference)
        {
            if (HP != reference.HP)
                return false;

            if (DMG != reference.DMG)
                return false;

            if (DEF != reference.DEF)
                return false;

            if (SPD != reference.SPD)
                return false;

            if (CRC != reference.CRC)
                return false;

            if (CRD != reference.CRD)
                return false;

            if (STA != reference.STA)
                return false;

            if (STR != reference.STR)
                return false;

            if (XP != reference.XP)
                return false;

            return true;
        }
    }
}
