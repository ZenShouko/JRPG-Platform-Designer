using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace JRPG_Platform_Designer.Entities
{
    public class MapFoe : MapObject
    {
        public string IconNames { get; set; }
        public bool HasDetectedPlayer { get; set; }
        public string MovementBehaviour { get; set; }
        public List<Character> FoeTeam { get; set; } = new List<Character>();
        public Coordinates Poistion { get; set; }
        public string Name { get; set; }

        public MapFoe()
        {
            Icon.Source = new System.Windows.Media.Imaging.BitmapImage(
                new Uri("../../Icons/foe-neutral.png", UriKind.RelativeOrAbsolute));
            Icon.Name = "ImgFoe";
        }

        public void CopyFrom(MapFoe reference)
        {
            //Copy all the properties
            MovementBehaviour = reference.MovementBehaviour;

            foreach (Character c in reference.FoeTeam)
            {
                Character newCharacter = new Character();
                newCharacter.CopyFrom(c);
                FoeTeam.Add(newCharacter);
            }

            IconNames = reference.IconNames;
            Name = reference.Name;
            Icon.Source = reference.Icon.Source;
            //Position.CopyFrom(reference.Position); //No need as it's a reference and has no position
        }

        /// <summary>
        /// Compares the teams to see if they're the same.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public bool Compare(MapFoe reference)
        {
            //Check count
            if (FoeTeam.Count != reference.FoeTeam.Count)
                return false;

            //Check movement behaviour
            if (MovementBehaviour != reference.MovementBehaviour)
                return false;

            //Check name
            if (Name != reference.Name)
                return false;

            //Compare each character
            for (int i = 0; i < FoeTeam.Count; i++)
            {
                if (!FoeTeam[i].Compare(reference.FoeTeam[i]))
                    return false;
            }

            return true;
        }
    }
}
