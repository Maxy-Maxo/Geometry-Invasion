using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Geometry_Invasion
{
    public partial class GameScreen : UserControl
    {
        bool upKey, downKey, leftKey, rightKey, leftClick, rightClick, Zkey, Xkey, Ckey, pauseGame;
        int wave, enemiesLeft, enemyTimer, tick, delayTimer, gameState, minStrength, mouseX, mouseY;
        double score, spawnPower, spawnTime;
        public static int idCounter = 0;
        Random random = new Random();

        List<Enemy> enemies = new List<Enemy>();
        List<Enemy> spawnList = new List<Enemy>();
        List<Powerup> powerups = new List<Powerup>();
        List<int> typeList = new List<int>();
        List<int> powerupCounters = new List<int>();
        int[] typeRarities = { 0, 0, 20, 60, 40, 60, 0, 50, 60, 60, 50, 50, 40, 50 }; // The chance of the type of enemy chosen being rerolled
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush greyBrush = new SolidBrush(Color.Gray);
        SolidBrush darkGreyBrush = new SolidBrush(Color.FromArgb(30, 30, 30));
        Pen barPen = new Pen(Color.White);

        Color[] powerupColours =
        {
            Color.Violet,
            Color.IndianRed,
            Color.Gold,
            Color.DodgerBlue,
            Color.Gray,
            Color.DarkBlue,
            Color.ForestGreen,
            Color.Indigo,
            Color.DarkOrange,
            Color.MediumSpringGreen,
            Color.SteelBlue,
            Color.DarkOliveGreen,
            Color.YellowGreen
        };
        int[] powerupDuration =
        {
            50,
            200,
            150,
            200,
            3,
            2,
            150,
            0,
            1,
            150,
            2,
            200,
            0
        };
        Bitmap[] powerupIcons =
        {
            Properties.Resources.health,
            Properties.Resources.homing,
            Properties.Resources.up,
            Properties.Resources.speed,
            Properties.Resources.mine,
            Properties.Resources.swap,
            Properties.Resources.multi,
            Properties.Resources.shield,
            Properties.Resources.fireball,
            Properties.Resources.reload,
            Properties.Resources.boost,
            Properties.Resources.resistance,
            Properties.Resources.triangle
        };
        public GameScreen()
        {
            InitializeComponent();
            NewGame();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (!pauseGame)
            {
                if (enemies[0].health > 0) // Player controls (player must be alive)
                {
                    if (enemies[0].speed == 7)
                    {
                        if (upKey != downKey)
                        {
                            double movement;
                            if (leftKey != rightKey)
                            {
                                movement = Math.Sqrt(Math.Pow(enemies[0].speed, 2) / 2);
                            }
                            else
                            {
                                movement = enemies[0].speed;
                            }
                            if (powerupCounters[3] > 0)
                            {
                                movement *= 1.3;
                            }
                            if (upKey)
                            {
                                enemies[0].y -= movement;
                            }
                            else
                            {
                                enemies[0].y += movement;
                            }
                        }
                        if (leftKey != rightKey)
                        {
                            double movement;
                            if (upKey != downKey)
                            {
                                movement = Math.Sqrt(Math.Pow(enemies[0].speed, 2) / 2);
                            }
                            else
                            {
                                movement = enemies[0].speed;
                            }
                            if (powerupCounters[3] > 0)
                            {
                                movement *= 1.3;
                            }
                            if (leftKey)
                            {
                                enemies[0].x -= movement;
                            }
                            else
                            {
                                enemies[0].x += movement;
                            }
                        }
                        enemies[0].direction = Convert.ToInt16(Form1.GetDirection(mouseX - enemies[0].x, mouseY - enemies[0].y));
                    }
                    else
                    {
                        MoveEnemy(enemies[0]);
                    }
                    // Powerup controls
                    if (leftClick)
                    {
                        Shoot(enemies[0]);
                    }
                    if (Zkey)
                    {
                        UsePowerup(4, 200);
                    }
                    if (Xkey)
                    {
                        UsePowerup(5, 201);
                    }
                    if (Ckey)
                    {
                        UsePowerup(8, 203);
                    }
                    if (rightClick && powerupCounters[10] > 0 && enemies[0].reloadTimer == 0)
                    {
                        enemies[0].speed = 25;
                        enemies[0].reloadTimer = enemies[0].reload;
                        powerupCounters[10]--;
                    }
                }

                if (gameState == 0)
                {
                    enemyTimer++;
                }
                else if (gameState == 2)
                {
                    delayTimer++;
                    if (delayTimer >= 60)
                    {
                        delayTimer = 0;
                        wave++;
                        NewWave(wave);
                    }
                }

                tick++;
                if (tick >= 360)
                {
                    tick = 0;
                }

                if (enemyTimer >= spawnTime && gameState == 0)
                {
                    enemyTimer = 0;
                    enemies.Add(spawnList[0]);
                    if (spawnList[0].type == 8 || spawnList[0].type == 9)
                    {
                        int rand = random.Next(0, 6);
                        int segs = 4;
                        while (rand == 0)
                        {
                            segs++;
                            rand = random.Next(0, 6);
                        }
                        enemies[enemies.Count - 1].segments = segs;
                    }
                    spawnList.RemoveAt(0);
                    if (spawnList.Count == 0)
                    {
                        gameState = 1;
                    }
                }

                foreach (Enemy en in enemies)
                {
                    if (en.team == 1 && en.colour != Color.Blue)
                    {
                        en.colour = Color.Blue;
                    }
                    if (en.segments > 0)
                    {
                        if (en.type == 9)
                        {
                            for (int i = 0; i < en.segments; i++)
                            {
                                Enemy newClone = new Enemy(en.x, en.y, en.type, en.strength - 1, en.direction, en.team)
                                {
                                    target = en.id,
                                    targetType = 1,
                                    homing = 4
                                };
                                enemies.Add(newClone);
                            }
                            en.segments = 0;
                        }
                        else
                        {
                            Enemy newSegment = new Enemy(en.x, en.y, en.type, en.strength, en.direction, en.team)
                            {
                                segments = en.segments - 1,
                                target = en.id,
                                targetType = 3
                            };
                            enemies.Add(newSegment);
                            en.segments = 0;
                        }
                        break;
                    }
                    if (en.id != 0 && en.targetType != 3)
                    {
                        EnemyAI(en);
                    }
                    else
                    {
                        CheckCollisions(en);
                    }
                    if (en.reload > 0 & en.reloadTimer > 0)
                    {
                        en.reloadTimer--;
                    }
                    if ((en.type >= 100 && en.type != 202 && (en.x > 810 || en.x < -10 || en.y > 810 || en.y < -10)) || en.health <= 0)
                    {
                        if (gameState != -1)
                        {
                            FindEnemiesLeft(en.team);
                        }
                        if (en.id != 0)
                        {
                            if (en.type == 5)
                            {
                                enemies.Add(new Enemy(en.x, en.y, 6, en.strength, en.direction, en.team));
                                enemies.Add(new Enemy(en.x, en.y, 6, en.strength, en.direction + 180, en.team));
                                FindEnemiesLeft(en.team);
                            }
                            else if (en.type == 202 || en.type == 14)
                            {
                                if (en.type == 202)
                                {
                                    powerupCounters[7]--;
                                }
                                else
                                {
                                    powerupCounters[12]--;
                                }
                            }
                            if (en.team == 0)
                            {
                                score += en.scoreValue;
                                if (en.type < 100) // Drop a powerup randomly
                                {
                                    List<int> dropsAvailable = new List<int>();
                                    List<int> dropratesAvailable = new List<int>();
                                    List<Powerup> finalDrops = new List<Powerup>();

                                    dropsAvailable = en.drops.ToList();
                                    dropratesAvailable = en.droprates.ToList();
                                    while (dropsAvailable.Count > 0)
                                    {
                                        int drop = random.Next(0, dropsAvailable.Count);
                                        int dropStrength = en.strength - 4;
                                        while (random.Next(1, 101) > Math.Round(100 * Math.Pow((100 - dropratesAvailable[drop] * 5) / 100.0, 1 + en.strength - dropStrength)))
                                        {
                                            dropStrength++;
                                        }
                                        if (dropStrength >= Form1.playerStrength - 1 && dropStrength >= 0)
                                        {
                                            finalDrops.Add(new Powerup(Convert.ToInt16(en.x), Convert.ToInt16(en.y), dropsAvailable[drop], dropStrength));
                                        }
                                        dropsAvailable.RemoveAt(drop);
                                        dropratesAvailable.RemoveAt(drop);
                                    }
                                    if (finalDrops.Count > 1)
                                    {
                                        for (int i = 0; i < finalDrops.Count; i++)
                                        {
                                            finalDrops[i].x += Convert.ToInt16(en.size * Math.Pow(1.1, en.strength - minStrength) * Math.Sin(i * 360 / finalDrops.Count * Math.PI / 180));
                                            finalDrops[i].y += Convert.ToInt16(en.size * Math.Pow(1.1, en.strength - minStrength) * Math.Cos(i * 360 / finalDrops.Count * Math.PI / 180));
                                            if (Math.Abs(finalDrops[i].x - 400) > 400)
                                            {
                                                finalDrops[i].x = Math.Sign(finalDrops[i].x - 400) * 400 + 400;
                                            }
                                            if (Math.Abs(finalDrops[i].y - 400) > 400)
                                            {
                                                finalDrops[i].y = Math.Sign(finalDrops[i].y - 400) * 400 + 400;
                                            }
                                        }
                                    }
                                    foreach (Powerup p in finalDrops)
                                    {
                                        powerups.Add(p);
                                    }
                                }
                            }
                            enemies.Remove(en);
                            break;
                        }
                    }
                    if ((en.type == 11 && en.speed > 4) || (en.type == 0 && en.speed > 7))
                    {
                        en.speed--;
                    }
                }
                foreach (Enemy en in enemies)
                {
                    if (en.id != 0)
                    {
                        if (Shoot(en))
                        {
                            break;
                        }
                        if (en.isBoss && en.bossTimer > 0)
                        {
                            en.bossTimer--;
                            if (en.bossTimer == 0)
                            {
                                FindEnemiesLeft(0);
                                if (enemiesLeft < 7)
                                {
                                    SpecialAttack(en, 0, 2, 200);
                                    break;
                                }
                            }
                        }
                    }
                }

                foreach (Powerup p in powerups)
                {
                    p.timer--;
                    if (p.timer == 0)
                    {
                        powerups.Remove(p);
                        break;
                    }
                    double distHit = enemies[0].size * Math.Pow(1.1, enemies[0].strength - minStrength) + 20 * Math.Pow(1.1, p.strength - minStrength);
                    if (Math.Abs(p.x - enemies[0].x) < distHit && Math.Abs(p.y - enemies[0].y) < distHit)
                    {
                        powerupCounters[p.type] += Convert.ToInt16(Math.Round(powerupDuration[p.type] * Math.Pow(1.5, p.strength - enemies[0].strength)));
                        if (p.type == 7 || p.type == 12)
                        {
                            if (p.type == 7)
                            {
                                enemies.Add(new Enemy(enemies[0].x, enemies[0].y, 202, p.strength, enemies[0].direction, 1));
                                powerupCounters[7]++;
                            }
                            else
                            {
                                enemies.Add(new Enemy(enemies[0].x, enemies[0].y, 14, p.strength, enemies[0].direction, 1));
                                enemies.Add(new Enemy(enemies[0].x, enemies[0].y, 14, p.strength, enemies[0].direction, 1));
                                powerupCounters[12] += 2;
                            }
                        }
                        powerups.Remove(p);
                        break;
                    }
                }
                for (int i = 0; i < powerupCounters.Count; i++) // Powerup abilities
                {
                    if (powerupCounters[i] > 0 && gameState != 2)
                    {
                        if (!(i == 4 || i == 5 || i == 7 || i == 8 || i == 10 || i == 12))
                        {
                            powerupCounters[i]--;
                        }
                        switch (i)
                        {
                            case 0:
                                if (enemies[0].health > 0)
                                {
                                    enemies[0].health += 8 * Math.Pow(1.5, enemies[0].strength);
                                    if (enemies[0].health > enemies[0].maxHealth)
                                    {
                                        enemies[0].health = enemies[0].maxHealth;
                                        powerupCounters[i]++;
                                    }
                                }
                                break;
                            case 2:
                                if (enemies[0].strength == Form1.playerStrength)
                                {
                                    double healthPercent = enemies[0].health / enemies[0].maxHealth;
                                    enemies[0].strength++;
                                    enemies[0].ShapeSetup();
                                    enemies[0].health = healthPercent * enemies[0].maxHealth;
                                }
                                if (powerupCounters[2] == 0)
                                {
                                    double healthPercent = enemies[0].health / enemies[0].maxHealth;
                                    enemies[0].strength = Form1.playerStrength;
                                    enemies[0].ShapeSetup();
                                    enemies[0].health = healthPercent * enemies[0].maxHealth;
                                }
                                break;
                            case 6:
                                if (enemies[0].shots == 1)
                                {
                                    enemies[0].shots = 4;
                                }
                                if (powerupCounters[6] == 0)
                                {
                                    enemies[0].shots = 1;
                                }
                                break;
                            case 9:
                                if (enemies[0].reload == 10)
                                {
                                    enemies[0].reload = 6;
                                }
                                if (powerupCounters[9] == 0)
                                {
                                    enemies[0].reload = 10;
                                }
                                break;
                            case 11:
                                if (enemies[0].resistance == 0)
                                {
                                    enemies[0].resistance = 7;
                                }
                                if (powerupCounters[11] == 0)
                                {
                                    enemies[0].resistance = 0;
                                }
                                break;
                        }
                    }
                }
            }

            Refresh();
        }
        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            Font gameFont = new Font("VT323", 20);
            Font gameFontSmall = new Font("VT323", 15);
            Font gameFontTiny = new Font("VT323", 9);
            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center
            };
            SolidBrush shapeBrush = new SolidBrush(Color.FromArgb(50, 50, 50));

            foreach (Enemy en in enemies)
            {
                if (en.health > 0)
                {
                    int drawSize = Convert.ToInt16(en.size * Math.Pow(1.1, en.strength - minStrength));
                    if (en.team == 1)
                    {
                        shapeBrush.Color = Color.FromArgb(0, 0, 50);
                    }
                    else if (en.team == 0)
                    {
                        shapeBrush.Color = Color.FromArgb(50, 0, 0);
                    }
                    e.Graphics.FillEllipse(shapeBrush, Convert.ToInt16(en.x - drawSize - 5), Convert.ToInt16(en.y - drawSize - 5), drawSize * 2 + 10, drawSize * 2 + 10);
                }
            }
            foreach (Enemy en in enemies)
            {
                if (en.health > 0)
                {
                    int drawSize = Convert.ToInt16(en.size * Math.Pow(1.1, en.strength - minStrength));
                    Color shapeColour = en.colour;
                    if (en.damageFlash)
                    {
                        shapeColour = Color.White;
                        en.damageFlash = false;
                    }
                    shapeBrush.Color = shapeColour;
                    switch (en.type)
                    {
                        case 0:
                            e.Graphics.FillEllipse(shapeBrush,  Convert.ToInt16(en.x - drawSize), Convert.ToInt16(en.y - drawSize), Convert.ToInt16(drawSize * 2), Convert.ToInt16(drawSize * 2));
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x + (10 + drawSize) * Math.Sin(en.direction * Math.PI / 180)), Convert.ToInt16(en.y + (10 + drawSize) * Math.Cos(en.direction * Math.PI / 180)), 8, en.direction, shapeColour, e);
                            break;
                        case 1:
                            Form1.FillShape(4, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction + 45, shapeColour, e);
                            break;
                        case 2:
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            break;
                        case 3:
                            Form1.FillShape(5, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            break;
                        case 4:
                            Form1.FillShape(5, 2, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction + tick * 10, shapeColour, e);
                            break;
                        case 5:
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction + 180, shapeColour, e);
                            break;
                        case 6:
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            break;
                        case 7:
                            Form1.FillShape(5, 2, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            break;
                        case 8:
                            Form1.FillShape(6, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            shapeBrush.Color = Color.Black;
                            e.Graphics.FillEllipse(shapeBrush, Convert.ToInt16(en.x - drawSize * 0.5), Convert.ToInt16(en.y - drawSize * 0.5), drawSize, drawSize);
                            break;
                        case 9:
                            Form1.FillShape(7, 2, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction + tick * 5, shapeColour, e);
                            Form1.FillShape(7, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize * 0.5), en.direction + tick * -5, shapeColour, e);
                            break;
                        case 10:
                            Form1.FillShape(8, 3, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            break;
                        case 11:
                            Form1.FillShape(6, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            break;
                        case 12:
                            Form1.FillShape(8, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            break;
                        case 13:
                            Form1.FillShape(4, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction + 45, shapeColour, e);
                            shapeColour = Color.Purple;
                            Form1.FillShape(4, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize * 0.5), en.direction + 45, shapeColour, e);
                            break;
                        case 14:
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            break;
                        // missiles
                        case 100:
                            e.Graphics.FillEllipse(shapeBrush, Convert.ToInt16(en.x - drawSize), Convert.ToInt16(en.y - drawSize), Convert.ToInt16(drawSize * 2), Convert.ToInt16(drawSize * 2));
                            break;
                        case 107:
                            List<Point> vertices = new List<Point>
                            {
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin(en.direction * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos(en.direction * Math.PI / 180)))),
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 144) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 144) * Math.PI / 180)))),
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 216) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 216) * Math.PI / 180))))
                            };

                            Point[] points = vertices.ToArray();
                            e.Graphics.FillPolygon(shapeBrush, points);
                            break;
                        case 110:
                            List<Point> vertices2 = new List<Point>
                            {
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin(en.direction * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos(en.direction * Math.PI / 180)))),
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 135) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 135) * Math.PI / 180)))),
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 225) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 225) * Math.PI / 180))))
                            };

                            Point[] points2 = vertices2.ToArray();
                            e.Graphics.FillPolygon(shapeBrush, points2);
                            break;
                        case 113:
                            Form1.FillShape(8, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction, shapeColour, e);
                            break;
                        // Powerups
                        case 200:
                            e.Graphics.FillEllipse(shapeBrush, Convert.ToInt16(en.x - drawSize), Convert.ToInt16(en.y - drawSize), Convert.ToInt16(drawSize * 2), Convert.ToInt16(drawSize * 2));
                            Form1.FillShape(8, 3, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize * 1.3), en.direction, shapeColour, e);
                            break;
                        case 201:
                            Form1.FillShape(7, 3, Convert.ToInt16(en.x), Convert.ToInt16(en.y), drawSize, en.direction + tick * 20, shapeColour, e);
                            break;
                        case 202:
                            e.Graphics.FillEllipse(darkGreyBrush, Convert.ToInt16(en.x - drawSize), Convert.ToInt16(en.y - drawSize), Convert.ToInt16(drawSize * 2), Convert.ToInt16(drawSize * 2));
                            break;
                        case 203:
                            for (int i = 0; i < 6; i++)
                            {
                                Form1.FillShape(5, 1, Convert.ToInt16(en.x - (i * drawSize * 0.6 * Math.Sin(en.direction * Math.PI / 180))), Convert.ToInt16(en.y - (i * drawSize * Math.Cos(en.direction * Math.PI / 180))), Convert.ToInt16(drawSize * 1.6 * Math.Pow(0.7, i)), random.Next(0, 360), Color.DarkOrange, e);
                            }
                            shapeBrush.Color = Color.DarkRed;
                            e.Graphics.FillEllipse(shapeBrush, Convert.ToInt16(en.x - drawSize), Convert.ToInt16(en.y - drawSize), Convert.ToInt16(drawSize * 2), Convert.ToInt16(drawSize * 2));
                            break;
                    }
                }
            }
            foreach (Enemy en in enemies)
            {
                if (en.health > 0)
                {
                    int drawSize = Convert.ToInt16(en.size * Math.Pow(1.1, en.strength - minStrength));
                    if (en.type < 100 || en.type >= 200)
                    {
                        if (en.targetType != 3 && en.type < 200 || en.type == 202)
                        {
                            e.Graphics.DrawString($"Lvl {en.strength + 1}", Font, whiteBrush, Convert.ToInt16(en.x), Convert.ToInt16(en.y + drawSize + 10), stringFormat);
                            if (en.team == 0 && en.health <= 3000 * Math.Pow(1.5, enemies[0].strength) && en.strength - enemies[0].strength <= 6 && powerupCounters[5] > 0)
                            {
                                e.Graphics.DrawImage(powerupIcons[5], Convert.ToInt16(en.x - 5), Convert.ToInt16(en.y + drawSize + 27), 10, 10);
                            }
                        }
                        if (en.health != en.maxHealth)
                        {
                            if (en.id == 0)
                            {
                                if (en.health + 8 * Math.Pow(1.5, en.strength) * powerupCounters[0] < en.maxHealth)
                                {
                                    e.Graphics.FillRectangle(greyBrush, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 22), Convert.ToInt16(60 * (en.health + 8 * Math.Pow(1.5, en.strength) * powerupCounters[0]) / en.maxHealth), 6);
                                }
                                else
                                {
                                    e.Graphics.FillRectangle(greyBrush, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 22), 60, 6);
                                }
                            }
                            SolidBrush healthBrush = new SolidBrush(Color.Red);
                            if (Convert.ToInt16(100 * en.health / en.maxHealth) > 65)
                            {
                                healthBrush.Color = Color.Green;
                            }
                            else if (Convert.ToInt16(100 * en.health / en.maxHealth) > 30)
                            {
                                healthBrush.Color = Color.Yellow;
                            }
                            else if (Convert.ToInt16(100 * en.health / en.maxHealth) > 15)
                            {
                                healthBrush.Color = Color.Orange;
                            }
                            e.Graphics.FillRectangle(healthBrush, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 22), Convert.ToInt16(60 * en.health / en.maxHealth), 6);
                            e.Graphics.DrawRectangle(barPen, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 22), 60, 6);
                        }
                    }
                }
            }

            foreach (Powerup p in powerups)
            {
                if ((tick % 3 == 0 && p.timer < 50) || (tick % 3 >= 1 && p.timer >= 50) || p.timer > 150)
                {
                    int drawSize = Convert.ToInt16(20 * Math.Pow(1.1, p.strength - minStrength));
                    Form1.FillShape(5, 1, p.x, p.y, drawSize, tick * 5, powerupColours[p.type], e);
                    Form1.FillShape(5, 1, p.x, p.y, drawSize, tick * -5, powerupColours[p.type], e);
                    e.Graphics.DrawImage(powerupIcons[p.type], Convert.ToInt16(p.x - 12 * Math.Pow(1.1, p.strength - minStrength)), Convert.ToInt16(p.y - 12 * Math.Pow(1.1, p.strength - minStrength)), Convert.ToInt16(24 * Math.Pow(1.1, p.strength - minStrength)), Convert.ToInt16(24 * Math.Pow(1.1, p.strength - minStrength)));
                    e.Graphics.DrawString($"Lvl {p.strength + 1}", Font, whiteBrush, Convert.ToInt16(p.x), Convert.ToInt16(p.y + drawSize + 10), stringFormat);
                }
            }
            e.Graphics.DrawString($"Wave {wave + 1}", gameFont, whiteBrush, 400, 30, stringFormat);
            e.Graphics.DrawString($"Score: {Math.Round(score * 10) / 10}", gameFontSmall, whiteBrush, 35, 755);
            if (gameState == 2)
            {
                e.Graphics.DrawRectangle(barPen, 350, 70, 100, 30);
                e.Graphics.FillRectangle(greyBrush, 350, 70, (60 - delayTimer) * 5 / 3, 30);
                e.Graphics.DrawString("Next wave", gameFontSmall, whiteBrush, 400, 75, stringFormat);
            }
            e.Graphics.FillRectangle(darkGreyBrush, 750, 5, 45, 45);

            int x = 0;
            for (int i = 0; i < powerupCounters.Count; i++)
            {
                if (powerupCounters[i] > 0)
                {
                    shapeBrush.Color = powerupColours[i];
                    string keyPress = "";
                    switch (i)
                    {
                        case 4:
                            keyPress = "Z";
                            break;
                        case 5:
                            keyPress = "X";
                            break;
                        case 8:
                            keyPress = "C";
                            break;
                        case 10:
                            keyPress = "R-Click";
                            break;
                    }
                    if (powerupCounters[i] >= Math.Round((double)powerupDuration[i] / 3) || tick % 10 >= 5)
                    {
                        e.Graphics.FillRectangle(shapeBrush, 35 + 30 * x, 705, 25, 25);
                        e.Graphics.DrawImage(powerupIcons[i], 35 + 30 * x, 705, 25, 25);
                    }
                    e.Graphics.DrawString($"{powerupCounters[i]}", Font, whiteBrush, 47 + 30 * x, 685, stringFormat);
                    if (keyPress != "")
                    {
                        e.Graphics.DrawString(keyPress, (i == 10) ? gameFontTiny : Font, whiteBrush, 47 + 30 * x, 732, stringFormat);
                    }
                    x++;
                }
            }

            if (pauseGame || gameState == -1)
            {
                e.Graphics.FillRectangle(darkGreyBrush, 200, 200, 400, 400);

                string message = "";
                if (pauseGame)
                {
                    message = "Pause Menu";
                    button1.Visible = true;
                    button2.Visible = true;
                    button1.Text = "RESUME";
                    button2.Text = "EXIT";
                }
                else if (gameState == -1)
                {
                    message = "Game Over";
                    button1.Visible = true;
                    button2.Visible = true;
                    button1.Text = "PLAY AGAIN";
                    button2.Text = "EXIT";
                }
                e.Graphics.DrawString(message, gameFont, whiteBrush, 400, 220, stringFormat);
            }
            else if (button1.Visible)
            {
                button1.Visible = false;
                button2.Visible = false;
            }
        }
        private void GameScreen_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
        }
        private void GameScreen_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    leftClick = true;
                    break;
                case MouseButtons.Right:
                    rightClick = true;
                    break;

            }
        }
        private void GameScreen_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    leftClick = false;
                    break;
                case MouseButtons.Right:
                    rightClick = false;
                    break;

            }
        }
        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    upKey = false;
                    break;
                case Keys.S:
                    downKey = false;
                    break;
                case Keys.A:
                    leftKey = false;
                    break;
                case Keys.D:
                    rightKey = false;
                    break;
                case Keys.Z:
                    Zkey = false;
                    break;
                case Keys.X:
                    Xkey = false;
                    break;
                case Keys.C:
                    Ckey = false;
                    break;
            }
        }
        private void PauseLabel_Click(object sender, EventArgs e)
        {
            if (pauseGame || gameState == -1)
            {
                pauseGame = false;
            }
            else
            {
                pauseGame = true;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            pauseGame = false;
            if (gameState == -1)
            {
                NewGame();
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // Save data to XML file
            //XmlWriter writer = XmlWriter.Create("GeoXML.xml");
            
            Form1.ChangeScreen(this, new TitleScreen());
        }
        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    upKey = true;
                    break;
                case Keys.S:
                    downKey = true;
                    break;
                case Keys.A:
                    leftKey = true;
                    break;
                case Keys.D:
                    rightKey = true;
                    break;
                case Keys.Z:
                    Zkey = true;
                    break;
                case Keys.X:
                    Xkey = true;
                    break;
                case Keys.C:
                    Ckey = true;
                    break;
                case Keys.Space:
                    int rand1 = random.Next(0, 801);
                    int rand2 = random.Next(1, 5);
                    int xSpawn = 0, ySpawn = 0;
                    switch (rand2)
                    {
                        case 1:
                            xSpawn = rand1;
                            ySpawn = 0;
                            break;
                        case 2:
                            xSpawn = 0;
                            ySpawn = rand1;
                            break;
                        case 3:
                            xSpawn = rand1;
                            ySpawn = 800;
                            break;
                        case 4:
                            xSpawn = 800;
                            ySpawn = rand1;
                            break;
                    }
                    spawnList.Add(new Enemy(xSpawn, ySpawn, 13, 1, random.Next(0, 360), 1));
                    break;
            }
        }

        private void NewGame() // Reset everything and start a new game
        {
            wave = Form1.startingWave - 1;
            score = 0;
            enemyTimer = 0;
            delayTimer = 0;
            tick = 0;
            gameState = 2;
            idCounter = 0;
            enemies.Clear();
            spawnList.Clear();
            powerups.Clear();
            powerupCounters.Clear();
            enemies.Add(new Enemy(400, 400, 0, Form1.playerStrength, 0, 1));
            for (int i = 0; i < powerupDuration.Length; i++)
            {
                powerupCounters.Add(0);
            }
            powerups.Add(new Powerup(200, 200, 0, 4));
        }
        private void EnemyAI(Enemy enemy) // Any object (except powerups) that isn't a player will do this
        {
            MoveEnemy(enemy);
            if (enemy.homing > 0 && enemy.target >= 0)
            {
                bool targetDead = true;
                foreach (Enemy target in enemies)
                {
                    if (target.id == enemy.target)
                    {
                        double targetAngle = (Form1.GetDirection(target.x - enemy.x, target.y - enemy.y) - enemy.direction + 540) % 360 - 180;
                        enemy.direction += Math.Sign(targetAngle) * enemy.homing;
                        targetDead = false;
                        break;
                    }
                }
                if (targetDead)
                {
                    FindTarget(enemy);
                    if (enemy.type == 9)
                    {
                        enemy.targetType = 0;
                    }
                }
            }
            if (enemy.targetType < 1 && (tick % 60 == 0 && random.Next(0, 2) == 1 || enemy.x > 790 || enemy.x < 10 || enemy.y > 790 || enemy.y < 10))
            {
                FindTarget(enemy);
            }
        }
        private void CheckCollisions(Enemy en1) // Object checks whether it's colliding with another, and then takes damage if the teams don't match
        {
            if (en1.targetType == 3) // Attatches itself to its target
            {
                bool collision = false;
                foreach (Enemy en2 in enemies)
                {
                    if (en1.target == en2.id)
                    {
                        double distHit = en1.size * Math.Pow(1.1, en1.strength - minStrength) + en2.size * Math.Pow(1.1, en2.strength - minStrength);
                        double dirHit = Form1.GetDirection(en2.x - en1.x, en2.y - en1.y);

                        en1.x = en2.x - distHit * Math.Sin(dirHit * Math.PI / 180);
                        en1.y = en2.y - distHit * Math.Cos(dirHit * Math.PI / 180);
                        en1.direction = Convert.ToInt16(dirHit);
                        if (en1.team != en2.team)
                        {
                            en1.health -= en2.damage;
                            en2.health -= en1.damage;
                        }
                        collision = true;
                        break;
                    }
                }
                if (!collision)
                {
                    en1.targetType = 0;
                    FindTarget(en1);
                }
            }
            foreach (Enemy en2 in enemies) // Bounces away from the collision
            {
                if (!(en1.x == en2.x && en1.y == en2.y) && !((en1.type >= 100 || en2.type >= 100) && en1.team == en2.team) && en1.health > 0 && en2.health > 0 && !((en1.targetType == 3 && en1.target == en2.id) || (en2.targetType == 3 && en2.target == en1.id)) && !(en1.team == en2.team && (en1.target == en2.id || en2.target == en1.id)))
                {
                    double distX = Math.Abs(en2.x - en1.x);
                    double distY = Math.Abs(en2.y - en1.y);
                    double distHit = en1.size * Math.Pow(1.1, en1.strength - minStrength) + en2.size * Math.Pow(1.1, en2.strength - minStrength);
                    if (Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2)) < distHit)
                    {
                        double centreX = (en1.x + en2.x) / 2;
                        double centreY = (en1.y + en2.y) / 2;
                        double dirHit = Form1.GetDirection(centreX - en1.x, centreY - en1.y);
                        double push = 1;
                        if (en1.weight != en2.weight) // Calculation for weight ratio
                        {
                            if (en1.weight > en2.weight)
                            {
                                push = en2.weight / en1.weight;
                            }
                            else
                            {
                                push = (2 - en1.weight / en2.weight);
                            }
                        }

                        en1.x += (centreX - distHit * Math.Sin(dirHit * Math.PI / 180) / 2 - en1.x) * push;
                        en1.y += (centreY - distHit * Math.Cos(dirHit * Math.PI / 180) / 2 - en1.y) * push;
                        en2.x += (centreX + distHit * Math.Sin(dirHit * Math.PI / 180) / 2 - en2.x) * (2 - push);
                        en2.y += (centreY + distHit * Math.Cos(dirHit * Math.PI / 180) / 2 - en2.y) * (2 - push);
                        if (en1.team != en2.team)
                        {
                            if (en1.type != 201)
                            {
                                int rand1 = random.Next(1, 11);
                                int rand2 = random.Next(1, 11);
                                if (rand1 > en1.resistance)
                                {
                                    en1.health -= en2.damage;
                                    en1.damageFlash = true;
                                }
                                if (rand2 > en2.resistance)
                                {
                                    en2.health -= en1.damage;
                                    en2.damageFlash = true;
                                }
                            }
                            else
                            {
                                if (en1.health >= en2.health && enemies[0].health > 0 && en2.strength - en1.strength <= 6)
                                {
                                    ConvertConnected(en2.id, en1.team);
                                    en2.team = en1.team;
                                    en1.health -= en2.health;
                                    FindEnemiesLeft(0);
                                }
                            }
                        }
                    }
                }
            }
            if (Math.Abs(en1.x - 400) > 400 && en1.type < 100)
            {
                en1.x = Math.Sign(en1.x - 400) * 400 + 400;
            }
            if (Math.Abs(en1.y - 400) > 400 && en1.type < 100)
            {
                en1.y = Math.Sign(en1.y - 400) * 400 + 400;
            }
        }
        private bool Shoot(Enemy enemy) // Shape will shoot if it's able to
        {
            bool canShoot = false;
            if (enemy.reload > 0 && enemy.reloadTimer == 0)
            {
                for (int i = 0; i < enemy.shots; i++)
                {
                    canShoot = true;
                    enemies.Add(new Enemy(enemy.x, enemy.y, enemy.type + 100, enemy.strength, enemy.direction + (360 * i / enemy.shots), enemy.team));
                    enemies[enemies.Count - 1].colour = enemy.colour;
                    if (enemies[enemies.Count - 1].targetType < 2)
                    {
                        FindTarget(enemies[enemies.Count - 1]);
                    }
                    if (powerupCounters[1] > 0 && enemies[enemies.Count - 1].type == 100)
                    {
                        enemies[enemies.Count - 1].targetType = 1;
                        enemies[enemies.Count - 1].homing = 5;
                        double distance = -1;
                        for (int j = 0; j < enemies.Count; j++)
                        {
                            if (enemies[j].team != enemy.team && enemies[j].type < 100)
                            {
                                double distX = enemies[j].x - mouseX;
                                double distY = enemies[j].y - mouseY;
                                if (Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2)) < distance || distance == -1)
                                {
                                    distance = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));
                                    enemies[enemies.Count - 1].target = enemies[j].id;
                                }
                            }
                        }
                    }
                }
                if (enemy.type == 11)
                {
                    FindTarget(enemy);
                    if (enemy.target >= 0)
                    {
                        enemy.speed = 20;
                    }
                }
                if (enemy.type == 113)
                {
                    enemy.health = 0;
                }
                enemy.reloadTimer = enemy.reload;
            }
            return canShoot;
        }
        private void Shoot(Enemy enemy, int type, int strength) // Shape will shoot a specific shape if it's able to
        {
            enemies.Add(new Enemy(enemy.x, enemy.y, type, strength, enemy.direction, enemy.team));
            enemies[enemies.Count - 1].colour = enemy.colour;
            if (enemies[enemies.Count - 1].targetType < 2)
            {
                FindTarget(enemies[enemies.Count - 1]);
            }
        }
        void FindTarget(Enemy enemy) // Change direction and find the closest enemy if possible
        {
            if (enemy.type <= 100 || enemy.target == -1)
            {
                enemy.target = -1;
                if (enemy.targetType != 1) // Point in a random direction
                {
                    enemy.direction = random.Next(0, 360);
                }
                double distance = -1;
                for (int i = 0; i < enemies.Count; i++) // Change target to closest enemy, excluding missiles and powerup shapes
                {
                    if (enemies[i].team != enemy.team && enemies[i].type < 100 && enemies[i].health > 0)
                    {
                        double distX = enemies[i].x - enemy.x;
                        double distY = enemies[i].y - enemy.y;
                        if (Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2)) < distance || distance == -1)
                        {
                            distance = Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));
                            enemy.target = enemies[i].id;
                        }
                    }
                }
                foreach (Enemy target in enemies) // Point toward the target
                {
                    if (target.id == enemy.target && !(enemy.homing > 0 && enemy.type >= 100))
                    {
                        enemy.direction = Convert.ToInt16(Math.Round(Form1.GetDirection(target.x - enemy.x, target.y - enemy.y)));
                        break;
                    }
                }
            }
        }
        void UsePowerup(int pType, int type) // Spawn powerup shape if there are any of that type available
        {
            if (enemies[0].reloadTimer == 0 && powerupCounters[pType] > 0)
            {
                enemies.Add(new Enemy(enemies[0].x, enemies[0].y, type, enemies[0].strength, enemies[0].direction, enemies[0].team));
                enemies[0].reloadTimer = enemies[0].reload;
                powerupCounters[pType]--;
            }
        }
        void NewWave(int waveNumber) // Determine which shapes are available on this wave and randomly select them based on their rarity
        {
            gameState = 0;

            typeList.Clear();
            typeList.Add(1);
            if (wave >= 1)
            {
                typeList.Add(2);
                if (wave >= 2)
                {
                    typeList.Add(3);
                    if (wave >= 5)
                    {
                        typeList.Add(4);
                        typeList.Add(11);
                        if (wave >= 6)
                        {
                            typeList.Add(5);
                            typeList.Add(7);
                            if (wave >= 7)
                            {
                                typeList.Add(8);
                                typeList.Add(9);
                                if (wave >= 9)
                                {
                                    typeList.Add(12);
                                    if (wave >= 11)
                                    {
                                        typeList.Add(10);
                                        typeList.Add(13);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (wave >= 6)
            {
                minStrength = Convert.ToInt16(Math.Floor(Convert.ToDouble(waveNumber / 3)) - 1);
            }

            spawnPower = 10 * Math.Pow(1.2, waveNumber);
            while (spawnPower > 0)
            {
                double enemyStrength = Math.Floor(Convert.ToDouble(waveNumber / 3));
                if (random.Next(0, 4 - waveNumber % 3) == 1)
                {
                    enemyStrength++;
                }
                if (random.Next(0, 2) == 1 && Math.Pow(1.5, enemyStrength) <= spawnPower)
                {
                    while (random.Next(0, 3) < 1 && Math.Pow(1.5, enemyStrength + 1) <= spawnPower)
                    {
                        enemyStrength++;
                    }
                }
                else
                {
                    while ((random.Next(0, 3) < 1 || Math.Pow(1.5, enemyStrength) > spawnPower) && enemyStrength > minStrength)
                    {
                        enemyStrength--;
                    }
                }
                spawnPower -= Math.Pow(1.5, enemyStrength);
                int rand1 = random.Next(0, 801);
                int rand2 = random.Next(1, 5);
                int xSpawn = 0, ySpawn = 0;
                switch (rand2)
                {
                    case 1:
                        xSpawn = rand1;
                        ySpawn = 0;
                        break;
                    case 2:
                        xSpawn = 0;
                        ySpawn = rand1;
                        break;
                    case 3:
                        xSpawn = rand1;
                        ySpawn = 800;
                        break;
                    case 4:
                        xSpawn = 800;
                        ySpawn = rand1;
                        break;
                }
                int chosenType = typeList[random.Next(0, typeList.Count)];
                rand1 = random.Next(0, 101);
                while (rand1 < typeRarities[chosenType])
                {
                    chosenType = typeList[random.Next(0, typeList.Count)];
                }
                spawnList.Add(new Enemy(xSpawn, ySpawn, chosenType, Convert.ToInt16(enemyStrength), random.Next(0, 360), 0));
            }
            spawnTime = 500 / spawnList.Count;
            if (waveNumber % 5 == 4)
            {
                Enemy boss = spawnList[random.Next(0, spawnList.Count)];
                boss.strength = Convert.ToInt16(Math.Round(Math.Log10(10 * Math.Pow(1.2, waveNumber)) / Math.Log10(1.5)));
                boss.isBoss = true;
                boss.ShapeSetup();
                spawnList.Clear();
                spawnList.Add(boss);
                spawnTime = 100;
            }
        }
        void FindEnemiesLeft(int team) // Find the amount of shapes left on a certain team, if there are none on the team, either game over or wave complete
        {
            if (gameState != -1)
            {
                enemiesLeft = 0;
                foreach (Enemy en in enemies)
                {
                    if (en.type < 100 && en.team == team && en.health > 0)
                    {
                        enemiesLeft++;
                    }
                }
                if (enemiesLeft == 0 && (gameState == 1 || team == 1))
                {
                    if (team == 0)
                    {
                        gameState = 2;
                    }
                    else
                    {
                        gameState = -1;
                        if (wave > 3 && wave - 3 > Form1.startingWave)
                        {
                            Form1.startingWave = wave - 3;
                        }
                    }
                }
            }
        }
        void ConvertConnected(int id, int team) // When an shape changes its team, every shape connected to it also switches
        {
            foreach (Enemy en in enemies)
            {
                if (en.target == id && en.team != team)
                {
                    ConvertConnected(en.id, team);
                    en.team = team;
                }
            }
        }
        void MoveEnemy(Enemy enemy) // Shape moves in its direction and checks for collisions if there are shapes within range
        {
            bool collisionPossible = false;
            foreach (Enemy en in enemies)
            {
                if (Math.Sqrt(Math.Pow(en.x - enemy.x, 2) + Math.Pow(en.y - enemy.y, 2)) < enemy.size * Math.Pow(1.1, enemy.strength - minStrength) + en.size * Math.Pow(1.1, en.strength - minStrength) + enemy.speed || enemy.targetType == 3)
                {
                    collisionPossible = true;
                    break;
                }
            }
            if (collisionPossible)
            {
                for (int i = 0; i < enemy.speed; i++)
                {
                    enemy.x += Math.Sin(enemy.direction * Math.PI / 180);
                    enemy.y += Math.Cos(enemy.direction * Math.PI / 180);
                    if (i % 5 == 0)
                    {
                        CheckCollisions(enemy);
                    }
                }
            }
            else
            {
                enemy.x += Math.Sin(enemy.direction * Math.PI / 180) * enemy.speed;
                enemy.y += Math.Cos(enemy.direction * Math.PI / 180) * enemy.speed;
            }
        }
        void SpecialAttack(Enemy enemy, int attackNumber, int attackNumber2, int reload)
        {
            switch (attackNumber)
            {
                case 0:
                    for (int i = 0; i < 5; i++)
                    {
                        Shoot(enemy, attackNumber2, enemy.strength - 5);
                    }
                    break;
            }
            enemy.bossTimer = reload;
        }
    }
}
