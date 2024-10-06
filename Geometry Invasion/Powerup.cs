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
         * 4 = mines
         * 5 = team switcher
         * 6 = multishot
         * 7 = shield
         * 8 = fireball
         * 9 = fast reload
         * 10 = dash
         * 11 = resistance
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
