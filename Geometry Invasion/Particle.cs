using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Invasion
{
    internal class Particle
    {
        public int x;
        public int y;
        public int direction;
        public int speed;
        public int size;
        public int shapeSides;
        public int shapeLvl;
        public Color colour;
        public Particle(int _x, int _y, int _direction, int _speed, int _size, int _sides, int _lvl, Color _colour)
        {
            x = _x;
            y = _y;
            direction = _direction;
            speed = _speed;
            size = _size;
            colour = _colour;
            shapeSides = _sides;
            shapeLvl = _lvl;
        }
    }
}
