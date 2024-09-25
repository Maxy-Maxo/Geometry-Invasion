using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Geometry_Invasion
{
    internal class Enemy
    {
        /* targetType
         * 0 = default targeting AI
         * 1 = homing only, does not change targets, unless the target dies and the shape is not a missile
         * 2 = does not target
         * 3 = attatched to target, and when target dies, targetType changes to 0
         */

        public int type, strength, direction, speed, team, size, reload = 0, shots = 1, reloadTimer, id = -10, target = -1, homing = 0, targetType = 0, segments = 0;
        public double x, y, health, maxHealth, damage, weight = 1, scoreValue = 1;
        public Color colour = Color.White;
        public Enemy(double _x, double _y, int _type, int _strength, int _direction, int _team)
        {
            x = _x;
            y = _y;
            type = _type;
            strength = _strength;
            direction = _direction;
            team = _team;
            if (type < 100)
            {
                id = GameScreen.idCounter;
                GameScreen.idCounter++;
                if (GameScreen.idCounter >= 10000)
                {
                    GameScreen.idCounter = 1;
                }
            }

            switch (type)
            {
                case 0: // Player Circle
                    colour = Color.Blue;
                    maxHealth = 1000;
                    speed = 7;
                    damage = 5;
                    size = 25;
                    reload = 10;
                    break;
                case 1: // Basic Square
                    colour = Color.Goldenrod;
                    maxHealth = 1000;
                    speed = 7;
                    damage = 5;
                    size = 22;
                    break;
                case 2: // Fast Triangle
                    colour = Color.Lime;
                    maxHealth = 700;
                    speed = 10;
                    damage = 3;
                    size = 17;
                    homing = 1;
                    weight = 0.5;
                    break;
                case 3: // Heavy Pentagon
                    colour = Color.Purple;
                    maxHealth = 2000;
                    speed = 4;
                    damage = 7;
                    size = 27;
                    homing = 1;
                    scoreValue = 2;
                    weight = 5;
                    break;
                case 4: // Dangerous Pentagram
                    colour = Color.HotPink;
                    maxHealth = 800;
                    speed = 6;
                    damage = 15;
                    size = 25;
                    homing = 5;
                    scoreValue = 1.2;
                    break;
                case 5: // Splitting Hexagram
                    colour = Color.OrangeRed;
                    maxHealth = 1500;
                    speed = 7;
                    damage = 10;
                    size = 27;
                    homing = 2;
                    scoreValue = 1.8;
                    weight = 2;
                    break;
                case 6: // Split Triangle
                    colour = Color.OrangeRed;
                    maxHealth = 750;
                    speed = 7;
                    damage = 10;
                    size = 27;
                    homing = 2;
                    break;
                case 7: // Small Shooting Pentagram
                    colour = Color.Yellow;
                    maxHealth = 700;
                    speed = 4;
                    damage = 2;
                    size = 14;
                    reload = 30;
                    weight = 0.1;
                    break;
                case 8: // Centipede Hexagons
                    colour = Color.DarkCyan;
                    maxHealth = 600;
                    speed = 6;
                    damage = 5;
                    size = 25;
                    homing = 3;
                    scoreValue = 0.7;
                    break;
                case 9: // Clustered Heptagrams
                    colour = Color.BlueViolet;
                    maxHealth = 800;
                    speed = 6;
                    damage = 4;
                    size = 16;
                    homing = 2;
                    weight = 0.5;
                    break;
                case 10: // Quad-Shooting Octogram
                    colour = Color.Magenta;
                    maxHealth = 800;
                    speed = 4;
                    damage = 6;
                    size = 26;
                    reload = 120;
                    shots = 4;
                    break;
                case 11: // Charging Hexagon
                    colour = Color.DarkRed;
                    maxHealth = 1200;
                    speed = 4;
                    damage = 10;
                    size = 27;
                    weight = 3;
                    scoreValue = 1.2;
                    reload = 120;
                    shots = 0;
                    break;
                // missiles
                case 100:
                    colour = Color.Blue;
                    maxHealth = 6;
                    speed = 20;
                    damage = 200;
                    size = 10;
                    targetType = 2;
                    break;
                case 107:
                    colour = Color.Yellow;
                    maxHealth = 2;
                    speed = 25;
                    damage = 70;
                    size = 7;
                    targetType = 1;
                    break;
                case 110:
                    colour = Color.Magenta;
                    maxHealth = 80;
                    speed = 7;
                    damage = 3;
                    size = 10;
                    homing = 3;
                    targetType = 1;
                    break;
                // Powerups
                case 200:
                    maxHealth = 1;
                    speed = 0;
                    damage = 500;
                    size = 15;
                    targetType = 2;
                    break;
                case 201:
                    maxHealth = 3000;
                    speed = 25;
                    damage = 0;
                    size = 10;
                    targetType = 2;
                    break;
            }

            maxHealth *= Math.Pow(1.5, strength);
            damage *= Math.Pow(1.5, strength);
            scoreValue *= Math.Pow(1.5, strength);
            weight *= Math.Pow(1.5, strength);
            health = maxHealth;
            reloadTimer = reload;
            if (type >= 100)
            {
                scoreValue = 0;
                weight = 0;
            }
        }
    }
}
