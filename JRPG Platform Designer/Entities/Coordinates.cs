using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JRPG_Platform_Designer
{
    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int DirectionX { get; set; }
        public int DirectionY { get; set; }

        public void CopyFrom(Coordinates reference)
        {
            X = reference.X;
            Y = reference.Y;
            DirectionX = reference.DirectionX;
            DirectionY = reference.DirectionY;
        }

        public void New(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
