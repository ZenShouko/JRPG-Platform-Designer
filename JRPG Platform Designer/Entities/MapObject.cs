﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JRPG_Platform_Designer.Entities
{
    public abstract class MapObject
    {
        public Coordinates Position { get; set; } = new Coordinates();

        [JsonIgnore]
        public Image Icon { get; set; } = new Image();
    }
}
