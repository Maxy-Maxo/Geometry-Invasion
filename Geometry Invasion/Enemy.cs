﻿using System;
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
         * 1 = lock on to target until it dies. If the shape is not a missile, find new target
         * 2 = does not target/passive
         * 3 = attatched to target. When target dies, targetType changes to 0
         *
         * bossAttacks
         * 0 = 5 Fast Triangles
         * 1 = teleport behind
         * 2 = 8 mines
         * 3 = Laser
         * 4 = 2 Dangerous Triangles
         * 5 = 15 Team Switchers
         */

        public int type, strength, resistance, direction, speed, team, size, reload, shots, reloadTimer, id, target, shotBy = -1, homing, targetType, segments = 0, damageFlash = 0;
        public double x, y, health, maxHealth, damage, weight, scoreValue;
        public bool isBoss = false;
        public Color colour = Color.White;
        public Point tp = new Point(0, 0);
        public int[] drops;
        public int[] droprates;
        public int[] bossAttacks = { -1 };
        public Poison poisonDamage = new Poison() { damage = 0, duration = 0 };
        public List<Poison> poisonTaken = new List<Poison>();
        public struct Poison
        {
            public double damage;
            public int duration;
        };
        public Enemy(double _x, double _y, int _type, int _strength, int _direction, int _team)
        {
            x = _x;
            y = _y;
            type = _type;
            strength = _strength;
            direction = _direction;
            team = _team;
            id = GameScreen.idCounter;
            GameScreen.idCounter++;
            if (GameScreen.idCounter >= 100000)
            {
                GameScreen.idCounter = 1;
            }

            ShapeSetup();
        }
        public void ShapeSetup()
        {
            scoreValue = 1;
            weight = 1;
            target = -1;
            reload = 0;
            shots = 1;
            resistance = 0;
            homing = 0;
            targetType = 0;
            switch (type)
            {
                case 0: // Player Circle
                    SetStats(Color.Blue, 1000, 7, 5, 25);
                    reload = 10;
                    break;
                case 1: // Basic Square
                    SetStats(Color.Goldenrod, 1000, 7, 5, 22, new int[] { 0, 2 }, new int[] { 3 }, new int[] { 1, 2, 3 });
                    break;
                case 2: // Fast Triangle
                    SetStats(Color.Lime, 700, 10, 3, 17, new int[] { 3, 9, 10 }, new int[] { 2, 1, 1 }, new int[] { 0, 3, 4 });
                    homing = 1;
                    weight = 0.5;
                    break;
                case 3: // Heavy Pentagon
                    SetStats(Color.Purple, 2000, 4, 7, 27, new int[] { 0, 2, 7 }, new int[] { 6, 4, 2 }, new int[] { 0, 1, 2, 4 });
                    homing = 1;
                    scoreValue = 2;
                    weight = 5;
                    break;
                case 4: // Dangerous Pentagram
                    SetStats(Color.HotPink, 800, 6, 15, 25, new int[] { 1, 13, 2, 3 }, new int[] { 5, 3, 2, 2 }, new int[] { 0, 2 });
                    homing = 5;
                    scoreValue = 1.2;
                    break;
                case 5: // Splitting Hexagram
                    SetStats(Color.OrangeRed, 1500, 6, 10, 27, new int[] { 2, 1, 12, 0 }, new int[] { 5, 3, 3, 2 }, new int[] { 0, 3, 4 });
                    homing = 2;
                    scoreValue = 1.8;
                    weight = 2;
                    break;
                case 6: // Split Triangle
                    SetStats(Color.OrangeRed, 750, 6, 10, 27, new int[] { 4, 1, 13 }, new int[] { 4, 2, 1 }, new int[] { 3 });
                    homing = 2;
                    break;
                case 7: // Small Shooting Pentagram
                    SetStats(Color.Yellow, 700, 4, 2, 14, new int[] { 9, 1, 6 }, new int[] { 2, 2, 1 }, new int[] { 2, 3 });
                    reload = 30;
                    weight = 0.1;
                    break;
                case 8: // Centipede Hexagons
                    SetStats(Color.DarkCyan, 600, 6, 5, 25, new int[] { 0, 4 }, new int[] { 1 }, new int[] { 3 });
                    homing = 3;
                    scoreValue = 0.6;
                    break;
                case 9: // Clustered Heptagrams
                    SetStats(Color.BlueViolet, 800, 6, 4, 16, new int[] { 5, 12 }, new int[] { 3, 1 }, new int[] { 2, 3, 5 });
                    homing = 2;
                    weight = 0.5;
                    scoreValue = 0.8;
                    break;
                case 10: // Quad-Shooting Octagram
                    SetStats(Color.Magenta, 800, 4, 6, 26, new int[] { 6, 1, 9 }, new int[] { 5, 4, 1 }, new int[] { 2, 3, 5 });
                    reload = 120;
                    shots = 4;
                    targetType = 2;
                    break;
                case 11: // Charging Hexagon
                    SetStats(Color.DarkRed, 1200, 4, 10, 27, new int[] { 10, 13, 2, 8 }, new int[] { 4, 4, 2, 1 }, new int[] { 2, 3, 4 });
                    weight = 3;
                    scoreValue = 1.2;
                    reload = 120;
                    shots = 0;
                    break;
                case 12: // Resistant Octagon
                    SetStats(Color.Olive, 400, 8, 2, 20, new int[] { 11, 3, 7 }, new int[] { 4, 3, 1 }, new int[] { 1, 2, 4 });
                    homing = 1;
                    weight = 0.5;
                    scoreValue = 1.2;
                    resistance = 7;
                    break;
                case 13: // Barrier-Placing Square
                    SetStats(Color.Brown, 800, 7, 3, 30, new int[] { 4, 11, 7 }, new int[] { 4, 2, 1 }, new int[] { 3, 4, 5 });
                    reload = 120;
                    break;
                case 14: // Dangerous Triangle
                    SetStats(Color.Red, 1200, 7, 20, 20, new int[] { 12, 13 }, new int[] { 6, 4 }, new int[] { 0, 3, 4 });
                    homing = 1;
                    weight = 0.5;
                    scoreValue = 1.2;
                    break;
                case 15: // Small Laser-Shooting Heptagon
                    SetStats(Color.LightSkyBlue, 700, 4, 2, 14, new int[] { 1, 5 }, new int[] { 3, 2 }, new int[] { 1, 2, 5 });
                    reload = 50;
                    weight = 0.1;
                    break;
                case 16: // Rare Powerful Decagram
                    SetStats(Color.White, 3000, 5, 20, 30, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, new int[] { 1 }, new int[] { 0, 1, 2, 3, 4, 5 });
                    homing = 5;
                    weight = 5;
                    scoreValue = 5;
                    break;
                case 17: // Poisonous Heptagram
                    SetStats(Color.LimeGreen, 800, 6, 5, 22, new int[] { 14, 1 }, new int[] { 3 }, new int[] { 1, 4, 5 });
                    homing = 4;
                    poisonDamage.damage = 0.4;
                    poisonDamage.duration = 50;
                    break;
                case 18: // Teleporting Pentagon
                    SetStats(Color.Orange, 500, 9, 5, 20, new int[] { 3, 10 }, new int[] { 2 });
                    homing = 2;
                    reload = 80;
                    shots = 0;
                    break;
                // missiles
                case 100: // Player Circle
                    SetStats(6, 20, 200, 10);
                    target = -2;
                    break;
                case 107: // Small Shooting Pentagram
                    SetStats(1, 25, 40, 7);
                    break;
                case 110: // Quad-Shooting Octagram
                    SetStats(80, 7, 3, 10);
                    homing = 3;
                    break;
                case 113: // Barrier-Placing Square
                    SetStats(2000, 0, 3, 25);
                    reload = 600;
                    shots = 0;
                    weight = 10;
                    target = -2;
                    break;
                case 115: // Small Laser-Shooting Heptagon
                    SetStats(1, 200, 60, 5);
                    target = -2;
                    break;
                // Powerup shapes
                case 200: // Mine
                    SetStats(1, 0, 1000, 15);
                    targetType = 2;
                    break;
                case 201: // Team Switcher
                    SetStats(3000, 25, 0, 10);
                    weight = 0;
                    targetType = 2;
                    resistance = 10;
                    break;
                case 202: // Shield
                    SetStats(5000, 4, 4, 75);
                    homing = 5;
                    target = 0;
                    weight = 10;
                    resistance = 7;
                    targetType = 2;
                    break;
                case 203: // Fireball
                    SetStats(1, 15, 100, 35);
                    weight = 10;
                    resistance = 10;
                    targetType = 2;
                    break;
            }

            maxHealth *= Math.Pow(1.5, strength);
            damage *= Math.Pow(1.5, strength);
            poisonDamage.damage *= Math.Pow(1.5, strength);
            scoreValue *= Math.Pow(1.5, strength);
            weight *= Math.Pow(1.5, strength);
            health = maxHealth;
            reloadTimer = reload;
            if (type >= 100)
            {
                scoreValue = 0;
                targetType = 1;
                if (type != 113 && type < 200)
                {
                    weight = 0;
                }
                if (type >= 200 && type != 202)
                {
                    target = -2;
                }
            }
        }
        public void SetStats(Color _colour, int _health, int _speed, int _damage, int _size, int[] _drops, int[] _droprates)
        {
            colour = _colour;
            maxHealth = _health;
            speed = _speed;
            damage = _damage;
            size = _size;
            drops = _drops;
            droprates = _droprates;
        }
        public void SetStats(Color _colour, int _health, int _speed, int _damage, int _size, int[] _drops, int[] _droprates, int[] _bossAttacks)
        {
            colour = _colour;
            maxHealth = _health;
            speed = _speed;
            damage = _damage;
            size = _size;
            drops = _drops;
            droprates = _droprates;
            bossAttacks = _bossAttacks;
        }
        public void SetStats(Color _colour, int _health, int _speed, int _damage, int _size)
        {
            colour = _colour;
            maxHealth = _health;
            speed = _speed;
            damage = _damage;
            size = _size;
        }
        public void SetStats(int _health, int _speed, int _damage, int _size)
        {
            maxHealth = _health;
            speed = _speed;
            damage = _damage;
            size = _size;
        }
        public void TakePoisonDamage()
        {
            for (int i = 0; i < poisonTaken.Count; i++)
            {
                health -= poisonTaken[i].damage;
                poisonTaken[i] = new Poison { damage = poisonTaken[i].damage, duration = poisonTaken[i].duration - 1 };
                if (poisonTaken[i].duration == 0)
                {
                    poisonTaken.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
