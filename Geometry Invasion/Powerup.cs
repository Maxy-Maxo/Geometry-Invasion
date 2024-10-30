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
         * 12 = triangle helpers
         */
        public int x, y, type, strength, timer;
        public Powerup(int _x, int _y, int _type, int _strength)
        {
            x = _x;
            y = _y;
            type = _type;
            strength = _strength;
            timer = 300;
            if (strength > Form1.playerStrength)
            {
                timer *= Convert.ToInt16(Math.Round(Math.Pow(1.2, strength -  Form1.playerStrength)));
            }
        }
    }
}
