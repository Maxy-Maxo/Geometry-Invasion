using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Geometry_Invasion
{
    internal class Powerup
    {
        /* type
         * 0 = instant health
         * 1 = homing missiles
         * 2 = temporary level up
         * 3 = speed boost
         * 4 = give mines
         * 5 = switch an enemy's team
         * 6 = multishot
         * 7 = shield
         */
        public int x, y, type, strength;
        public Powerup(int _x, int _y, int _type, int _strength)
        {
            x = _x;
            y = _y;
            type = _type;
            strength = _strength;
        }
    }
}
