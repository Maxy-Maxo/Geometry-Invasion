using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Invasion
{
    internal class Enemy
    {
        /* targetType
         * 0 = default targeting AI
         * 1 = homing only, does not change targets
         * 2 = does not target
         * 3 = attatched to target, and when target dies, targetType changes to 0
         */

        public int type, strength, direction, speed, team, reload = 0, shots = 1, reloadTimer, id = -10, target = -1, homing = 0, targetType = 0, segments = 0;
        public double x, y, health, maxHealth, damage, size;
        public Enemy(double _x, double _y, int _type, int _strength, int _direction, int _team)
        {
            x = _x;
            y= _y;
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
                case 0:
                    maxHealth = 1000;
                    speed = 7;
                    damage = 5;
                    size = 25;
                    reload = 10;
                    break;
                case 1:
                    maxHealth = 1000;
                    speed = 7;
                    damage = 5;
                    size = 22;
                    break;
                case 2: 
                    maxHealth = 700;
                    speed = 10;
                    damage = 3;
                    size = 17;
                    homing = 1;
                    break;
                case 3:
                    maxHealth = 2000;
                    speed = 4;
                    damage = 7;
                    size = 27;
                    homing = 1;
                    break;
                case 4:
                    maxHealth = 800;
                    speed = 6;
                    damage = 15;
                    size = 25;
                    homing = 5;
                    break;
                case 5:
                    maxHealth = 1500;
                    speed = 7;
                    damage = 10;
                    size = 27;
                    homing = 2;
                    break;
                case 6:
                    maxHealth = 750;
                    speed = 7;
                    damage = 10;
                    size = 27;
                    homing = 2;
                    break;
                case 7:
                    maxHealth = 700;
                    speed = 4;
                    damage = 2;
                    size = 14;
                    reload = 30;
                    break;
                case 8:
                    maxHealth = 600;
                    speed = 6;
                    damage = 5;
                    size = 25;
                    homing = 3;
                    break;
                case 9:
                    maxHealth = 800;
                    speed = 6;
                    damage = 4;
                    size = 16;
                    break;
                case 10:
                    maxHealth = 800;
                    speed = 4;
                    damage = 6;
                    size = 26;
                    reload = 120;
                    shots = 4;
                    break;
                // missles
                case 100:
                    maxHealth = 6;
                    speed = 20;
                    damage = 200;
                    size = 10;
                    targetType = 2;
                    break;
                case 107:
                    maxHealth = 2;
                    speed = 25;
                    damage = 70;
                    size = 7;
                    targetType = 1;
                    break;
                case 110:
                    maxHealth = 80;
                    speed = 7;
                    damage = 3;
                    size = 10;
                    homing = 3;
                    targetType = 1;
                    break;
            }

            maxHealth *= Math.Pow(1.5, strength);
            damage *= Math.Pow(1.5, strength);
            size *= Math.Pow(1.1, strength);
            health = maxHealth;
            reloadTimer = reload;
        }
    }
}
