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

        public void CopyFrom(Character reference)
        {
            //Copy all the properties
            Level = reference.Level;
            Stats.CopyFrom(reference.Stats);
            EquipmentIDs = reference.EquipmentIDs;
            ID = reference.ID;
            Name = reference.Name;
            Description = reference.Description;
            ImageName = reference.ImageName;
            IsModified = reference.IsModified;
        }

        /// <summary>
        /// Compares characters to see if they're the same.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public bool Compare(Character reference)
        {
            //Check level
            if (Level != reference.Level)
                return false;

            //Check stats
            if (!Stats.Compare(reference.Stats))
                return false;

            //Check ID
            if (ID != reference.ID)
                return false;

            //Check name and description
            if (Name != reference.Name || Description != reference.Description)
                return false;

            //Check image name
            if (ImageName != reference.ImageName)
                return false;

            //Check isModified
            if (IsModified != reference.IsModified)
                return false;

            return true;
        }
    }
}
