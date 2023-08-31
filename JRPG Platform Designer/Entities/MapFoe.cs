using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Platform_Designer.Entities
{
    public class MapFoe : MapObject
    {
        public string IconNames { get; set; }
        public bool HasDetectedPlayer { get; set; }
        public string MovementBehaviour { get; set; }
        public List<Character> FoeTeam { get; set; } = new List<Character>();
        public Coordinates Poistion { get; set; }

        [JsonIgnore]
        public string Name { get; set; }
    }
}
