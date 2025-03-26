using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Geometry_Invasion
{
    internal class Enemy
    {
        /* targetType
         * 0 = default targeting AI
         * 1 = lock on to target until it dies. If the shape is not a bullet, find new target
         * 2 = does not target/passive
         * 3 = attatched to target. When target dies, targetType changes to 0
         *
         * bossAttacks
         * 0 = 5 Fast Triangles
         * 1 = teleport behind
         * 2 = 5 mines
         * 3 = Laser
         * 4 = 2 Dangerous Triangles
         * 5 = 15 Team Switchers
         * 6 = 3 lower lvl shapes
         */

        public int type, strength, resistance, resistHits = 0, direction, speed, team, size, reload, shots, reloadTimer, id, target, shotBy = -1, homing, targetType, segments = 0, damageFlash = 0;
        public float x, y, health, maxHealth, damage, weight, scoreValue;
        public bool isBoss = false;
        public Color colour = Color.White;
        public Point tp = new Point(0, 0);
        public int[] drops;
        public int[] droprates;
        public int[] bossAttacks = { -1 };
        public Poison poisonDamage = new Poison();
        public List<Poison> poisonTaken = new List<Poison>();
        public Polygon[] design = { };
        public struct Poison
        {
            public float damage;
            public int duration;
        };
        public struct Polygon
        {
            public int sides;
            public int level;
            public float size; // Size relative to actual size
            public int rotation;
            public int spin;
            public Color shapeColour;
        };
        public Enemy(float _x, float _y, int _type, int _strength, int _direction, int _team)
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
            poisonDamage.damage = 0;
            poisonDamage.duration = 0;
            switch (type)
            {
                case 0: // Player Circle
                    SetStats(Color.Blue, 1000, 7, 5, 25);
                    design = new Polygon[] { NewPolygon(1, 1) };
                    reload = 10;
                    break;
                case 1: // Basic Square
                    SetStats(Color.Goldenrod, 1000, 7, 5, 22, new int[] { 0, 2 }, new int[] { 3 }, new int[] { 1, 2, 6 });
                    design = new Polygon[] { NewPolygon(4, 1, 45, 0) };
                    break;
                case 2: // Fast Triangle
                    SetStats(Color.Lime, 700, 10, 3, 17, new int[] { 3, 9, 10 }, new int[] { 2, 1, 1 }, new int[] { 0, 3, 4 });
                    design = new Polygon[] { NewPolygon(3, 1) };
                    homing = 1;
                    weight = 0.5f;
                    break;
                case 3: // Heavy Pentagon
                    SetStats(Color.Purple, 2000, 4, 7, 27, new int[] { 0, 2, 7 }, new int[] { 6, 4, 2 }, new int[] { 0, 1, 2, 4 });
                    design = new Polygon[] { NewPolygon(5, 1) };
                    homing = 1;
                    scoreValue = 2;
                    weight = 5;
                    break;
                case 4: // Dangerous Pentagram
                    SetStats(Color.HotPink, 800, 6, 15, 25, new int[] { 1, 13, 2, 3 }, new int[] { 5, 3, 2, 2 }, new int[] { 0, 2, 6 });
                    design = new Polygon[] { NewPolygon(5, 2, 0, 10) };
                    homing = 5;
                    scoreValue = 1.2f;
                    break;
                case 5: // Splitting Hexagram
                    SetStats(Color.OrangeRed, 1500, 6, 10, 27, new int[] { 2, 1, 12, 0 }, new int[] { 5, 3, 3, 2 }, new int[] { 0, 3, 4 });
                    design = new Polygon[] { NewPolygon(3, 1), NewPolygon(3, 1, 180, 0) };
                    homing = 2;
                    scoreValue = 1.8f;
                    weight = 2;
                    break;
                case 6: // Split Triangle
                    SetStats(Color.OrangeRed, 750, 6, 10, 27, new int[] { 4, 1, 13 }, new int[] { 4, 2, 1 }, new int[] { 3 });
                    design = new Polygon[] { NewPolygon(3, 1) };
                    homing = 2;
                    break;
                case 7: // Small Shooting Pentagram
                    SetStats(Color.Yellow, 700, 4, 2, 14, new int[] { 9, 1, 6 }, new int[] { 2, 2, 1 }, new int[] { 2, 3 });
                    design = new Polygon[] { NewPolygon(5, 2) };
                    reload = 30;
                    weight = 0.1f;
                    break;
                case 8: // Centipede Hexagons
                    SetStats(Color.DarkCyan, 600, 6, 5, 25, new int[] { 0, 4 }, new int[] { 1 }, new int[] { 3 });
                    design = new Polygon[] { NewPolygon(6, 1), NewPolygon(1, 1, 0, 0, 0.5f, Color.Black) };
                    homing = 3;
                    scoreValue = 0.6f;
                    break;
                case 9: // Clustered Heptagrams
                    SetStats(Color.BlueViolet, 800, 6, 4, 16, new int[] { 5, 12 }, new int[] { 3, 1 }, new int[] { 2, 3, 5, 6 });
                    design = new Polygon[] { NewPolygon(7, 2, 0, 5), NewPolygon(7, 1, 0, -5, 0.5f, colour) };
                    homing = 2;
                    weight = 0.5f;
                    scoreValue = 0.8f;
                    break;
                case 10: // Quad-Shooting Octagram
                    SetStats(Color.Magenta, 800, 4, 6, 26, new int[] { 6, 1, 9 }, new int[] { 5, 4, 1 }, new int[] { 2, 3, 5 });
                    design = new Polygon[] { NewPolygon(8, 3) };
                    reload = 120;
                    shots = 4;
                    targetType = 2;
                    break;
                case 11: // Charging Hexagon
                    SetStats(Color.DarkRed, 1200, 4, 10, 27, new int[] { 10, 13, 2, 8 }, new int[] { 4, 4, 2, 1 }, new int[] { 2, 3, 4 });
                    design = new Polygon[] { NewPolygon(6, 1) };
                    weight = 3;
                    scoreValue = 1.2f;
                    reload = 120;
                    shots = 0;
                    break;
                case 12: // Resistant Octagon
                    SetStats(Color.Olive, 500, 8, 2, 20, new int[] { 11, 3, 7 }, new int[] { 4, 3, 1 }, new int[] { 1, 2, 4 });
                    design = new Polygon[] { NewPolygon(8, 1) };
                    homing = 1;
                    weight = 0.5f;
                    scoreValue = 1.2f;
                    resistance = 3;
                    break;
                case 13: // Barrier-Placing Square
                    SetStats(Color.Brown, 800, 7, 3, 30, new int[] { 4, 11, 7 }, new int[] { 4, 2, 1 }, new int[] { 3, 4, 5 });
                    design = new Polygon[] { NewPolygon(4, 1, 45, 0), NewPolygon(4, 1, 45, 0, 0.5f, Color.Black) };
                    reload = 120;
                    break;
                case 14: // Dangerous Triangle
                    SetStats(Color.Red, 1200, 7, 20, 20, new int[] { 12, 13 }, new int[] { 6, 4 }, new int[] { 0, 3, 4 });
                    design = new Polygon[] { NewPolygon(3, 1, 0, 15), NewPolygon(3, 1, 0, 15, 0.8f, Color.Black), NewPolygon(3, 1, 0, 15, 0.4f, colour) };
                    homing = 1;
                    weight = 0.5f;
                    scoreValue = 1.2f;
                    break;
                case 15: // Small Laser-Shooting Heptagon
                    SetStats(Color.LightSkyBlue, 700, 4, 2, 14, new int[] { 1, 5 }, new int[] { 3, 2 }, new int[] { 1, 2, 5 });
                    design = new Polygon[] { NewPolygon(7, 1) };
                    reload = 50;
                    weight = 0.1f;
                    break;
                case 16: // Rare Powerful Decagram
                    SetStats(Color.White, 3000, 5, 20, 30, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }, new int[] { 1 }, new int[] { 0, 1, 2, 3, 4, 5, 6 });
                    design = new Polygon[] { NewPolygon(10, 3, 0, 5) };
                    homing = 5;
                    weight = 5;
                    scoreValue = 5;
                    break;
                case 17: // Poisonous Heptagram
                    SetStats(Color.LimeGreen, 800, 6, 5, 22, new int[] { 14, 1 }, new int[] { 3 }, new int[] { 1, 5, 6 });
                    design = new Polygon[] { NewPolygon(7, 3, 0, 10) };
                    homing = 4;
                    poisonDamage.damage = 0.4f;
                    poisonDamage.duration = 50;
                    break;
                case 18: // Teleporting Pentagon
                    SetStats(Color.Navy, 500, 9, 5, 20, new int[] { 3, 10 }, new int[] { 2 }, new int[] { 1, 2, 6 });
                    design = new Polygon[] { NewPolygon(5, 1, 0, 3) };
                    homing = 2;
                    reload = 80;
                    shots = 0;
                    break;
                case 19: // Phasing Hexagram
                    SetStats(Color.DarkGray, 1000, 6, 2, 25, new int[] { 15, 0 }, new int[] { 2 }, new int[] { 1, 3, 6 });
                    design = new Polygon[] { NewPolygon(3, 1), NewPolygon(3, 1, 180, 0), NewPolygon(3, 1, 0, 0, 0.8f, Color.Black), NewPolygon(3, 1, 180, 0, 0.8f, Color.Black) };
                    homing = 3;
                    weight = -1;
                    scoreValue = 1.2f;
                    break;

                // bullets
                case 100: // Player Circle
                    SetStats(6, 20, 150, 10);
                    design = new Polygon[] { NewPolygon(1, 1) };
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
                    design = new Polygon[] { NewPolygon(8, 1) };
                    reload = 600;
                    shots = 0;
                    weight = 10;
                    target = -2;
                    break;
                case 115: // Small Laser-Shooting Heptagon
                    SetStats(1, 200, 60, 5);
                    design = new Polygon[] { NewPolygon(1, 1) };
                    target = -2;
                    break;
                // Powerup shapes
                case 200: // Mine
                    SetStats(1, 0, 750, 15);
                    design = new Polygon[] { NewPolygon(1, 1), NewPolygon(8, 3, 0, 0, 1.3f, colour) };
                    targetType = 2;
                    break;
                case 201: // Team Switcher
                    SetStats(3000, 25, 0, 15);
                    design = new Polygon[] { NewPolygon(7, 3, 0, 10) };
                    weight = 0;
                    targetType = 2;
                    resistance = 1000;
                    break;
                case 202: // Shield
                    SetStats(1000, 4, 4, 75);
                    design = new Polygon[] { NewPolygon(1, 1, 0, 0, 1, Color.FromArgb(50, 50, 60)) };
                    homing = 20;
                    target = 0;
                    weight = 10;
                    resistance = 20;
                    targetType = 2;
                    break;
                case 203: // Fireball
                    SetStats(2000, 15, 100, 35);
                    design = new Polygon[] { NewPolygon(1, 1, 0, 0, 1, Color.DarkRed) };
                    weight = -10;
                    resistance = 3;
                    targetType = 2;
                    break;
            }

            maxHealth *= (float)Math.Pow(1.5, strength);
            damage *= (float)Math.Pow(1.5, strength);
            poisonDamage.damage *= (float)Math.Pow(1.5, strength);
            scoreValue *= (float)Math.Pow(1.5, strength);
            weight *= (float)Math.Pow(1.5, strength);
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
        public Polygon NewPolygon(int _sides, int _level, int _rotation, int _spin, float _size, Color _colour)
        {
            Polygon p = new Polygon()
            {
                sides = _sides,
                level = _level,
                size = _size,
                rotation = _rotation,
                spin = _spin,
                shapeColour = _colour
            };
            return p;
        }
        public Polygon NewPolygon(int _sides, int _level)
        {
            Polygon p = new Polygon()
            {
                sides = _sides,
                level = _level,
                size = 1,
                rotation = 0,
                shapeColour = colour
            };
            return p;
        }
        public Polygon NewPolygon(int _sides, int _level, int _rotation, int _spin)
        {
            Polygon p = new Polygon()
            {
                sides = _sides,
                level = _level,
                size = 1,
                rotation = _rotation,
                spin = _spin,
                shapeColour = colour
            };
            return p;
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
        public void FillShape(Polygon shape, float baseSize, int spin, PaintEventArgs e)
        {
            SolidBrush shapeBrush = new SolidBrush((shape.shapeColour == colour && damageFlash > 0) ? Color.White : shape.shapeColour);
            float drawSize = shape.size * baseSize;
            if (shape.sides > 2) // Less than 3 sides makes a circle
            {
                float counter = shape.rotation + direction + spin;

                Point[] points = new Point[shape.sides];
                for (int i = 0; i < shape.sides; i++)
                {
                    points[i] = (new Point(Convert.ToInt16(Math.Round(x + drawSize * Math.Sin(counter * Math.PI / 180))), Convert.ToInt16(Math.Round(y + drawSize * Math.Cos(counter * Math.PI / 180)))));
                    counter += 360f * shape.level / shape.sides;
                }
                e.Graphics.FillPolygon(shapeBrush, points);
            }
            else
            {
                e.Graphics.FillEllipse(shapeBrush, x - drawSize, y - drawSize, drawSize * 2, drawSize * 2);
            }
        }
        public void SetColour(Color newColour)
        {
            for (int i = 0; i < design.Length; i++)
            {
                if (design[i].shapeColour == colour)
                {
                    design[i].shapeColour = newColour;
                }
            }
            colour = newColour;
        }
    }
}
