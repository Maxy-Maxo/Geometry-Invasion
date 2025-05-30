﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Geometry_Invasion
{
    public partial class GameScreen : UserControl
    {
        /* gameState
         * -1 = game over
         * 0 = shapes are spawning
         * 1 = shapes done spawning
         * 2 = next wave transition
         */
        const int NUM_OF_POWERUPS = 18;
        bool upKey, downKey, leftKey, rightKey, leftClick, rightClick, Zkey, Xkey, Ckey, Qkey, pauseGame;
        int wave, enemiesLeft, enemyTimer, tick, delayTimer, gameState, minStrength, mouseX, mouseY, bossAttack = -1, bossTimer = 100, enemyNumber = 0;
        float score, spawnPower, spawnTime;
        string menuMessage = "";
        public static int idCounter = 0;
        Random random = new Random();

        List<Enemy> enemies = new List<Enemy>();
        List<Enemy> spawnList = new List<Enemy>();
        List<Powerup> powerups = new List<Powerup>();
        List<Particle> particles = new List<Particle>();
        List<int> typeList = new List<int>();
        List<Button> currentButtons = new List<Button>();
        Bitmap crown = Properties.Resources.boss;
        Bitmap mystery = Properties.Resources.mystery;

        Button[] buttons =
        {
            new Button("RESUME", 0, 400, 350, 250, 85, Color.Black),
            new Button("PLAY AGAIN", 1, 400, 350, 250, 85, Color.Black),
            new Button("EXIT", 2, 400, 450, 250, 85, Color.Red)
        }; // Every possible button
        int[] typeChance =
        {
            0,
            20, // Basic Square
            17, // Fast Triangle
            10, // Heavy Pentagon
            15, // Dangerous Pentagram
            6,  // Splitting Hexagram
            0,
            10, // Small Shooting Pentagram
            6,  // Chained Hexagons
            8,  // Clustered Heptagrams
            10, // Quad-Shooting Octagram
            13, // Charging Hexagon
            15, // Resistant Octagon
            8,  // Barrier-Placing Square
            6,  // Dangerous Triangle
            10, // Small Laser-Shooting Heptagon
            2,  // Rare Powerful Decagram
            13, // Poisonous Heptagram
            16, // Teleporting Pentagon
            15, // Phasing Hexagram
            7,  // Small Chained Hexagons
        }; // The likelyhood of the shapes being chosen for the wave

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
            Color.YellowGreen,
            Color.DeepPink,
            Color.DarkOrchid,
            Color.Cyan,
            Color.Teal,
            Color.Maroon
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
            0,
            200,
            150,
            250,
            200,
            2
        }; // The duration can mean the amount of time or uses of the powerup, or it is zero if it is an instant powerup.
        int[] powerupCounters = new int[NUM_OF_POWERUPS];
        int[] powerupStrengths = new int[NUM_OF_POWERUPS];
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
            Properties.Resources.triangle,
            Properties.Resources.spikes,
            Properties.Resources.poison,
            Properties.Resources.phase,
            Properties.Resources.bullet_phase,
            Properties.Resources.explosion
        };
        int[] powerupCategories =
        {
            0,
            0,
            0,
            0,
            1,
            1,
            0,
            2,
            1,
            0,
            1,
            0,
            2,
            0,
            0,
            0,
            0,
            1
        }; // The way that the powerup is used. 0 = timer, 1 = at key press, 2 = instant effect
        string[] powerupInfo =
        {
            "Regenerates health gradually. It will deactivate\nif your health bar is full.",
            "Shoot with your mouse close to an enemy to let\nyour bullets lock on to them.",
            "Increases your level as well as the bullets you shoot.\nDoes not affect other powerups.",
            "Increases movement speed.\nUseful for escaping quick enemies.",
            "These have very high damage but very low health.",
            "Use these to capture an enemy\nand have it help you. The indicator below the enemy tells whether they\ncan be switched or not. Requires good aim.",
            "Good for clearing large groups of enemies.",
            "A large, high-health shape that follows you.\nGood at blocking bullets and low level enemies.",
            "A rare powerup. Use this wisely.\nLarge with high health and damage.",
            "Allows you to deal damage a little more quickly.",
            "A quick burst of movement that helps you\nescape fast enemies or avoid being surrounded.\nUse the mouse to control the direction of the boost.",
            "Allows you to take no damage from most hits,\nincreasing the chance of surviving high damage enemies.",
            "Summons two shapes with great stats that are\nvery reliable for taking out lower level enemies.",
            "Allows you to deal 8x your base damage.\nYou may want to have healing available, though.",
            "The bullets you shoot will now have slow but strong\npoison damage in addition to their regular damage.\nBest for taking down high health enemies.",
            "Allows you to phase through other non-phasing shapes.",
            ""
        }; // The text shown at the collection of an undiscovered powerup
        public GameScreen()
        {
            InitializeComponent();
            NewGame();
            //for (int i = 0; i < 100; i++)
            //{
            //    int i1 = random.Next(NUM_OF_POWERUPS);
            //    int i2 = random.Next(NUM_OF_POWERUPS);
            //    Swap(powerupColours, i1, i2);
            //    Swap(powerupIcons, i1, i2);
            //}
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
                            float movement;
                            if (leftKey != rightKey)
                            {
                                movement = (float)Math.Sqrt(Math.Pow(enemies[0].speed, 2) / 2);
                            }
                            else
                            {
                                movement = enemies[0].speed;
                            }
                            if (powerupCounters[3] > 0)
                            {
                                movement *= 1 + (0.3f * (powerupStrengths[3] + 1));
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
                            float movement;
                            if (upKey != downKey)
                            {
                                movement = (float)Math.Sqrt(Math.Pow(enemies[0].speed, 2) / 2);
                            }
                            else
                            {
                                movement = enemies[0].speed;
                            }
                            if (powerupCounters[3] > 0)
                            {
                                movement *= 1 + (0.3f * (powerupStrengths[3] + 1));
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
                        enemies[0].direction = Convert.ToInt16(Functions.GetDirection(mouseX - enemies[0].x, mouseY - enemies[0].y));
                    }
                    else
                    {
                        MoveEnemy(enemies[0]);
                    }
                    // Powerup controls
                    if (leftClick)
                    {
                        Shoot(enemies[0], enemies[0].shots);
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
                    if (Qkey && powerupCounters[17] > 0 && Shoot(enemies[0], 10 * (1 + powerupStrengths[17])))
                    {
                        powerupCounters[17] -= powerupStrengths[17] + 1;
                    }
                    if (rightClick && powerupCounters[10] > 0 && enemies[0].reloadTimer == 0)
                    {
                        enemies[0].direction = Convert.ToInt16(Functions.GetDirection(mouseX - enemies[0].x, mouseY - enemies[0].y));
                        enemies[0].speed = 25;
                        //particles.Add(new Particle((int)enemies[0].x, (int)enemies[0].y, 0, 0, DrawSize(enemies[0]) * 4, powerupColours[10]));
                        for (int i = 0; i < 4; i++)
                        {
                            particles.Add(new Particle((int)enemies[0].x, (int)enemies[0].y, enemies[0].direction, random.Next(enemies[0].speed - 15, enemies[0].speed - 4), DrawSize(enemies[0]) * 2, 3, 1, powerupColours[10]));
                        }
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
                        NewWave(++wave);
                    }
                }

                tick++;
                if (tick >= 360)
                {
                    tick = 0;
                }

                if (enemyTimer >= spawnTime && gameState == 0) // Spawn wave enemies
                {
                    enemyTimer = 0;
                    enemies.Add(spawnList[enemyNumber]);
                    FindTarget(enemies[enemies.Count - 1]);
                    int type = spawnList[enemyNumber].type;
                    if (type == 8 || type == 9 || type == 20)
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
                    if (++enemyNumber == spawnList.Count)
                    {
                        gameState = 1;
                        spawnList.Clear();
                    }
                }

                foreach (Enemy en in enemies) // Initialise multi-segmented shapes
                {
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
                                targetType = 3,
                                isBoss = en.isBoss
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
                    en.TakePoisonDamage();
                    // Shape dies or shape is bullet and falls off screen
                    if ((en.type >= 100 && en.type != 202 && (Math.Abs(en.x - 400) > 410 || Math.Abs(en.y - 400) > 410)) || en.health <= 0)
                    {
                        if (gameState != -1 && en.type != 5)
                        {
                            FindEnemiesLeft(en.team, true);
                        }
                        if (en.id != 0)
                        {
                            if (en.type == 5)
                            {
                                AddShape(en.x, en.y, 6, en.strength, en.direction, en.team);
                                enemies[enemies.Count - 1].isBoss = en.isBoss;
                                AddShape(en.x, en.y, 6, en.strength, en.direction + 180, en.team);
                                enemies[enemies.Count - 1].isBoss = en.isBoss;
                            }
                            else if (en.type == 202)
                            {
                                powerupCounters[7]--;
                            }
                            else if (en.type == 14 && en.shotBy == 0)
                            {
                                powerupCounters[12]--;
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
                                    int j = 0;
                                    while (dropratesAvailable.Count < dropsAvailable.Count)
                                    {
                                        dropratesAvailable.Add(en.droprates[j++]);
                                        if (j == en.droprates.Length)
                                        {
                                            j = 0;
                                        }
                                    }
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
                                        AddPowerup(p);
                                    }
                                }
                            }
                            if (en.type < 100 || en.type >= 200)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    particles.Add(new Particle((int)en.x, (int)en.y, random.Next(1, 360), 15, DrawSize(en), en.design[0].sides, en.design[0].level, en.colour));
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
                bool timerDecrease = false;
                foreach (Enemy en in enemies)
                {
                    if (en.id != 0)
                    {
                        if (Shoot(en, en.shots))
                        {
                            break;
                        }
                        if (en.isBoss && bossTimer > 0 && bossAttack != -1 && en.targetType != 3 && gameState != -1)
                        {
                            if (!timerDecrease)
                            {
                                bossTimer--;
                                timerDecrease = true;
                            }
                            if (bossTimer == 0)
                            {
                                SpecialAttack(en);
                                bossAttack = en.bossAttacks[random.Next(0, en.bossAttacks.Length)];
                                break;
                            }
                        }
                    }
                }

                foreach (Powerup p in powerups) // Powerups
                {
                    if (p.timer == 0)
                    {
                        particles.Add(new Particle(p.x, p.y, 0, 0, DrawSize(p), 1, 1, powerupColours[p.type]));
                        powerups.Remove(p);
                        break;
                    }
                    p.timer--;
                    double distHit = enemies[0].size * Math.Pow(1.1, enemies[0].strength - minStrength) + 20 * Math.Pow(1.1, p.strength - minStrength);
                    if (Math.Abs(p.x - enemies[0].x) < distHit && Math.Abs(p.y - enemies[0].y) < distHit)
                    {
                        if (p.type == -1)
                        {
                            p.type = random.Next(0, NUM_OF_POWERUPS);
                        }
                        powerupCounters[p.type] += Convert.ToInt32(Math.Round(powerupDuration[p.type] * Math.Pow(1.5, p.strength - Form1.playerStrength)));
                        if (powerupCategories[p.type] == 2)
                        {
                            if (p.type == 7)
                            {
                                AddShape(enemies[0].x, enemies[0].y, 202, p.strength, random.Next(0, 360), 1);
                                powerupCounters[7]++;
                            }
                            else
                            {
                                int spawnDir = random.Next(0, 360);
                                for (int i = 0; i < 2; i++)
                                {
                                    Shoot(enemies[0], 14, p.strength);
                                    float distance = Convert.ToInt16(enemies[0].size * Math.Pow(1.1, enemies[0].strength - minStrength)) + Convert.ToInt16(enemies[enemies.Count - 1].size * Math.Pow(1.1, enemies[enemies.Count - 1].strength - minStrength));
                                    enemies[enemies.Count - 1].direction = spawnDir;
                                    enemies[enemies.Count - 1].x += 2 * distance * (float)Math.Sin(enemies[enemies.Count - 1].direction * Math.PI / 180);
                                    enemies[enemies.Count - 1].y += 2 * distance * (float)Math.Cos(enemies[enemies.Count - 1].direction * Math.PI / 180);
                                    powerupCounters[12]++;
                                    spawnDir += 180;
                                }
                            }
                        }
                        p.timer = 0;
                    }
                }
                for (int i = 0; i < powerupCounters.Length; i++) // Powerup abilities
                {
                    if (powerupCounters[i] > 0 && gameState != 2 && enemies[0].health > 0 && powerupCategories[i] != 2 && i != 10)
                    {
                        powerupStrengths[i] = 0;
                        if (powerupCounters[i] >= powerupDuration[i] * 10)
                        {
                            powerupStrengths[i]++;
                            if (powerupCounters[i] >= powerupDuration[i] * 35)
                            {
                                powerupStrengths[i]++;
                            }
                        }
                        if (!(i == 0 || powerupCategories[i] != 0))
                        {
                            powerupCounters[i] -= powerupStrengths[i] + 1;
                        }
                        switch (i)
                        {
                            case 0:
                                float heal = 8 * (float)Math.Pow(1.5, Form1.playerStrength);
                                if (enemies[0].health <= enemies[0].maxHealth)
                                {
                                    for (int j = 0; j < powerupStrengths[0] + 1; j++)
                                    {
                                        enemies[0].health += heal;
                                        powerupCounters[0]--;
                                    }
                                }
                                break;
                            case 2:
                                if (enemies[0].strength != Form1.playerStrength + 1 + powerupStrengths[2])
                                {
                                    float healthPercent = enemies[0].health / enemies[0].maxHealth;
                                    enemies[0].strength = Form1.playerStrength + 1 + powerupStrengths[2];
                                    enemies[0].ShapeSetup();
                                    enemies[0].health = healthPercent * enemies[0].maxHealth;
                                }
                                if (powerupCounters[2] == 0)
                                {
                                    float healthPercent = enemies[0].health / enemies[0].maxHealth;
                                    enemies[0].strength = Form1.playerStrength;
                                    enemies[0].ShapeSetup();
                                    enemies[0].health = healthPercent * enemies[0].maxHealth;
                                }
                                break;
                            case 6:
                                if (enemies[0].shots != 4 + powerupStrengths[6])
                                {
                                    enemies[0].shots = 4 + powerupStrengths[6];
                                }
                                if (powerupCounters[6] == 0)
                                {
                                    enemies[0].shots = 1;
                                }
                                break;
                            case 9:
                                if (enemies[0].reload != 7 - powerupStrengths[9] * 2)
                                {
                                    enemies[0].reload = 7 - powerupStrengths[9] * 2;
                                }
                                if (powerupCounters[9] == 0)
                                {
                                    enemies[0].reload = 10;
                                }
                                break;
                            case 11:
                                if (enemies[0].resistance != 3 * (1 + powerupStrengths[11]))
                                {
                                    enemies[0].resistance = 3 * (1 + powerupStrengths[11]);
                                }
                                if (powerupCounters[11] == 0)
                                {
                                    enemies[0].resistance = 0;
                                }
                                break;
                            case 13:
                                if (enemies[0].damage != 40 * (1 + powerupStrengths[13]) * Math.Pow(1.5, Form1.playerStrength))
                                {
                                    enemies[0].damage = 40 * (1 + powerupStrengths[13]) * (float)Math.Pow(1.5, Form1.playerStrength);
                                }
                                if (powerupCounters[13] == 0)
                                {
                                    enemies[0].damage = 5;
                                }
                                break;
                            case 15:
                                if (enemies[0].weight > 0)
                                {
                                    enemies[0].weight *= -1;
                                }
                                if (enemies[0].resistance < 1 + powerupStrengths[15])
                                {
                                    enemies[0].resistance = 1 + powerupStrengths[15];
                                }
                                if (powerupCounters[15] == 0)
                                {
                                    enemies[0].weight *= -1;
                                    if (powerupCounters[11] == 0)
                                    {
                                        enemies[0].resistance = 0;
                                    }
                                }
                                break;
                        }
                    }
                }
                foreach (Particle p in particles)
                {
                    p.x += (int)(p.speed * Math.Sin(p.direction * Math.PI / 180));
                    p.y += (int)(p.speed * Math.Cos(p.direction * Math.PI / 180));
                    if (p.speed > 0)
                    {
                        p.speed--;
                    }
                    p.size = (int)(p.size * 0.75);
                    if (p.size == 0)
                    {
                        particles.Remove(p);
                        break;
                    }
                }
            }

            Refresh();
        }
        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            SolidBrush greyBrush = new SolidBrush(Color.Gray);
            SolidBrush darkGreyBrush = new SolidBrush(Color.FromArgb(30, 30, 30));
            Pen barPen = new Pen(Color.White);
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
                    int drawSize = DrawSize(en);
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
                if (en.isBoss)
                {
                    if (bossTimer < 40 && bossAttack == 1 && tick % 2 == 1)
                    {
                        foreach (Enemy en2 in enemies)
                        {
                            if (en.target == en2.id)
                            {
                                e.Graphics.DrawImage(powerupIcons[1], (float)(en2.x - Math.Sin(en2.direction * Math.PI / 180) * (en2.size + en.size + 40)) - 10, (float)(en2.y - Math.Cos(en2.direction * Math.PI / 180) * (en2.size + en.size + 40)) - 10, 20, 20);
                            }
                        }
                    }
                    else if (bossTimer < 30 && bossAttack == 3)
                    {
                        if (bossTimer == 29)
                        {
                            foreach (Enemy target in enemies)
                            {
                                if (target.id == en.target)
                                {
                                    en.tp = new Point(Convert.ToInt16(target.x), Convert.ToInt16(target.y));
                                    break;
                                }
                            }
                        }
                        if (!(en.tp.X == 0 && en.tp.Y == 0) && tick % 2 == 1)
                        {
                            e.Graphics.DrawImage(powerupIcons[1], en.tp.X - 10, en.tp.Y - 10, 20, 20);
                        }
                    }
                }
                if (en.health > 0)
                {
                    if (!(en.tp.X == 0 && en.tp.Y == 0))
                    {
                        if (tick % 2 == 1)
                        {
                            e.Graphics.DrawImage(powerupIcons[1], en.tp.X - 10, en.tp.Y - 10, 20, 20);
                        }
                        Pen connectPen = new Pen(Color.FromArgb(30, 30, 30), 2);
                        e.Graphics.DrawLine(connectPen, en.x, en.y, en.tp.X, en.tp.Y);
                    }
                    int drawSize = DrawSize(en);
                    Color shapeColour = en.colour;
                    if (en.damageFlash > 0)
                    {
                        shapeColour = Color.White;
                        en.damageFlash--;
                    }
                    shapeBrush.Color = shapeColour;
                    Point[] points;
                    switch (en.type)
                    {
                        case 0:
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x + (10 + drawSize) * Math.Sin(en.direction * Math.PI / 180)), Convert.ToInt16(en.y + (10 + drawSize) * Math.Cos(en.direction * Math.PI / 180)), 8, en.direction, shapeColour, e);
                            break;
                        case 15:
                            if (en.reloadTimer == 25)
                            {
                                foreach (Enemy target in enemies)
                                {
                                    if (target.id == en.target)
                                    {
                                        en.tp = new Point(Convert.ToInt16(target.x), Convert.ToInt16(target.y));
                                        break;
                                    }
                                }
                            }
                            break;
                        case 18:
                            if (en.reloadTimer == 15)
                            {
                                en.tp = new Point(random.Next(0, 801), random.Next(0, 801));
                            }
                            break;
                        case 107:
                            points = new Point[]
                            {
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin(en.direction * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos(en.direction * Math.PI / 180)))),
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 144) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 144) * Math.PI / 180)))),
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 216) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 216) * Math.PI / 180))))
                            };
                            e.Graphics.FillPolygon(shapeBrush, points);
                            break;
                        case 110:
                            points = new Point[]
                            {
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin(en.direction * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos(en.direction * Math.PI / 180)))),
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 135) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 135) * Math.PI / 180)))),
                                new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 225) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 225) * Math.PI / 180))))
                            };
                            e.Graphics.FillPolygon(shapeBrush, points);
                            break;
                        case 115:
                            Pen laserPen = new Pen(shapeColour, drawSize);

                            foreach (Enemy shotBy in enemies)
                            {
                                if (shotBy.id == en.shotBy)
                                {
                                    e.Graphics.DrawLine(laserPen, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(shotBy.x), Convert.ToInt16(shotBy.y));
                                }
                            }
                            break;
                        case 203:
                            for (int i = 0; i < 6; i++)
                            {
                                Form1.FillShape(5, 1, Convert.ToInt16(en.x - (i * drawSize * Math.Sin(en.direction * Math.PI / 180))), Convert.ToInt16(en.y - (i * drawSize * Math.Cos(en.direction * Math.PI / 180))), Convert.ToInt16(drawSize * 1.6 * Math.Pow(0.7, i)), tick * (i + 1) * 10, Color.DarkOrange, e);
                            }
                            break;
                        default:
                            break;
                    }
                    foreach (Enemy.Polygon p in en.design)
                    {
                        en.FillShape(p, drawSize, tick * p.spin, e);
                    }
                    switch(en.type)
                    {
                        case 0:
                            for (int i = 0; i < powerupCounters.Length; i++)
                            {
                                if (powerupCounters[i] > 0)
                                {
                                    switch (i)
                                    {
                                        case 11:
                                            Form1.DrawShape(1, 1, en.x, en.y, drawSize * 1.4f, 0, powerupColours[i], 3, e);
                                            break;
                                        case 13:
                                            Form1.FillShape(9, 2, en.x, en.y, drawSize * 1.8f, tick * 8, powerupColours[i], e);
                                            break;
                                        case 15:
                                            Form1.FillShape(1, 1, en.x, en.y, drawSize * 0.8f, 0, Color.Black, e);
                                            break;
                                    }
                                }
                            }
                            break;
                        case 201:
                            e.Graphics.DrawImage(powerupIcons[5], Convert.ToInt16(en.x - drawSize * 0.5), Convert.ToInt16(en.y - drawSize * 0.5), drawSize, drawSize);
                            break;
                        case 202:
                            e.Graphics.DrawImage(powerupIcons[7], Convert.ToInt16(en.x - drawSize * 0.5), Convert.ToInt16(en.y - drawSize * 0.5), drawSize, drawSize);
                            break;
                        default:
                            break;
                    }
                }
            }
            foreach (Enemy en in enemies)
            {
                if (en.health > 0)
                {
                    int drawSize = DrawSize(en);
                    if (en.type < 100 || en.type >= 200)
                    {
                        if (en.targetType != 3 && en.type < 200 || en.type == 202)
                        {
                            // Shape lvl display
                            e.Graphics.DrawString($"Lvl {en.strength + 1}", Font, whiteBrush, Convert.ToInt16(en.x), Convert.ToInt16(en.y + drawSize + 10), stringFormat);
                            // Team-switch available indicator
                            if (en.team == 0 && en.health <= 3000 * Math.Pow(1.5, Form1.playerStrength + powerupStrengths[5]) && en.damage <= 1000 * Math.Pow(1.5, powerupStrengths[5]) && en.strength - Form1.playerStrength <= 6 + powerupStrengths[5] && powerupCounters[5] > 0)
                            {
                                e.Graphics.DrawImage(powerupIcons[5], Convert.ToInt16(en.x - 5), Convert.ToInt16(en.y + drawSize + 27), 10, 10);
                            }
                            if (en.isBoss)
                            {
                                e.Graphics.DrawImage(crown, Convert.ToInt16(en.x - 10), Convert.ToInt16(en.y - drawSize - 47), 20, 20);
                            }
                        }
                        SolidBrush healthBrush = new SolidBrush(powerupColours[11]);
                        if (en.resistance > 0)
                        {
                            e.Graphics.FillRectangle(healthBrush, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 25), Convert.ToInt16(60 * (1 - (float)en.resistHits / en.resistance)), 3);
                        }
                        if (en.health < en.maxHealth) // Display health bar
                        {
                            if (en.id == 0)
                            {
                                shapeBrush.Color = Color.FromArgb(3 * powerupColours[0].R / 4, 3 * powerupColours[0].G / 4, 3 * powerupColours[0].B / 4);
                                if (en.health + 8 * Math.Pow(1.5, Form1.playerStrength) * powerupCounters[0] < en.maxHealth)
                                {
                                    e.Graphics.FillRectangle(shapeBrush, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 22), Convert.ToInt16(60 * (en.health + 8 * Math.Pow(1.5, Form1.playerStrength) * powerupCounters[0]) / en.maxHealth), 6);
                                }
                                else
                                {
                                    e.Graphics.FillRectangle(shapeBrush, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 22), 60, 6);
                                }
                            }
                            healthBrush.Color = Color.Red;
                            if (en.poisonTaken.Count == 0)
                            {
                                if (100 * en.health / en.maxHealth > 65)
                                {
                                    healthBrush.Color = Color.Green;
                                }
                                else if (100 * en.health / en.maxHealth > 30)
                                {
                                    healthBrush.Color = Color.Yellow;
                                }
                                else if (100 * en.health / en.maxHealth > 15)
                                {
                                    healthBrush.Color = Color.Orange;
                                }
                            }
                            else
                            {
                                healthBrush.Color = powerupColours[14];
                            }
                            e.Graphics.FillRectangle(healthBrush, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 22), Convert.ToInt16(60 * en.health / en.maxHealth), 6);
                            e.Graphics.DrawRectangle(barPen, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 22), 60, 6);
                        }
                    }
                }
            }
            foreach (Powerup p in powerups) // Powerups
            {
                if ((tick % 3 == 0 && p.timer < 50) || (tick % 3 >= 1 && p.timer >= 50) || p.timer > 150)
                {
                    int drawSize = Convert.ToInt16(20 * Math.Pow(1.1, p.strength - minStrength));
                    Color drawColour = (p.type >= 0) ? powerupColours[p.type] : Color.White;
                    //Form1.FillShape(5, 1, p.x, p.y, drawSize, tick * 5, powerupColours[p.type], e);
                    //Form1.FillShape(5, 1, p.x, p.y, drawSize, tick * -5, powerupColours[p.type], e);
                    Form1.FillShape(1, 1, (float)(p.x + drawSize * 0.1 * Math.Sin((tick + p.rand) * 5 * Math.PI / 180)), (float)(p.y + drawSize * 0.1 * Math.Cos((tick + p.rand) * 5 * Math.PI / 180)), drawSize * 1.2f, 0, Color.FromArgb((int)(drawColour.R * 0.8), (int)(drawColour.G * 0.8), (int)(drawColour.B * 0.8)), e);
                    Form1.FillShape(1, 1, (float)(p.x + drawSize * 0.1 * Math.Sin((tick + p.rand) * -10 * Math.PI / 180)), (float)(p.y + drawSize * 0.1 * Math.Cos((tick + p.rand) * -10 * Math.PI / 180)), drawSize, 0, drawColour, e);
                    e.Graphics.DrawImage((p.type >= 0) ? powerupIcons[p.type] : mystery, Convert.ToInt16(p.x - 12 * Math.Pow(1.1, p.strength - minStrength)), Convert.ToInt16(p.y - 12 * Math.Pow(1.1, p.strength - minStrength)), Convert.ToInt16(24 * Math.Pow(1.1, p.strength - minStrength)), Convert.ToInt16(24 * Math.Pow(1.1, p.strength - minStrength)));
                    e.Graphics.DrawString($"Lvl {p.strength + 1}", Font, whiteBrush, Convert.ToInt16(p.x), Convert.ToInt16(p.y + drawSize + 10), stringFormat);
                }
            }
            string waveMsg = (gameState == 2) ? ((wave + 1 == Form1.startingWave) ? $"Starting at Wave {wave + 2}..." : $"Wave {wave + 1} Complete") : $"Wave {wave + 1}";
            e.Graphics.DrawString(waveMsg, gameFont, whiteBrush, 400, 30, stringFormat); // Wave display
            if (gameState != -1)
            {
                e.Graphics.DrawString($"Score: {Math.Round(score * 10) / 10}", gameFontSmall, whiteBrush, 35, 755); // Score display
            }
            else
            {
                e.Graphics.DrawString($"Points Earned: {Math.Round(score)}", gameFontSmall, whiteBrush, 35, 735); // Points display
                e.Graphics.DrawString($"Total Points: {Form1.points}", gameFontSmall, whiteBrush, 35, 755);
            }
            if (gameState == 2) // Wave countdown bar
            {
                e.Graphics.DrawRectangle(barPen, 350, 70, 100, 30);
                e.Graphics.FillRectangle(greyBrush, 350, 70, (60 - delayTimer) * 5 / 3, 30);
                e.Graphics.DrawString("Next wave", gameFontSmall, whiteBrush, 400, 75, stringFormat);
            }
            if (gameState == 0)
            {
                e.Graphics.DrawRectangle(barPen, 300, 70, 200, 15);
                e.Graphics.FillRectangle(greyBrush, 300, 70, (float)(enemyNumber * 200) / spawnList.Count, 15);
                e.Graphics.DrawString($"{enemyNumber} / {spawnList.Count}", gameFontSmall, whiteBrush, 400, 68, stringFormat);
            }
            e.Graphics.FillRectangle(darkGreyBrush, 750, 5, 45, 45);

            int x = 0;
            for (int i = 0; i < powerupCounters.Length; i++) // Powerup HUD
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
                        case 17:
                            keyPress = "Q";
                            break;
                    }
                    if (powerupCounters[i] >= Math.Round((float)powerupDuration[i] / 3) || tick % 10 >= 5)
                    {
                        e.Graphics.FillRectangle(shapeBrush, 35 + 30 * x, 705, 25, 25);
                        e.Graphics.DrawImage(powerupIcons[i], 35 + 30 * x, 705, 25, 25);
                    }
                    string dec = ((float)powerupCounters[i] / 20 >= 10) ? "0" : "1";
                    string timer = (powerupCategories[i] == 0) ? ((float)powerupCounters[i] / 20).ToString("F" + dec) : powerupCounters[i].ToString();
                    e.Graphics.DrawString(timer, Font, whiteBrush, 47 + 30 * x, 685, stringFormat);
                    if (keyPress != "")
                    {
                        e.Graphics.DrawString(keyPress, (i == 10) ? gameFontTiny : Font, whiteBrush, 47 + 30 * x, 732, stringFormat);
                    }
                    for (int j = 0; j < powerupStrengths[i]; j++)
                    {
                        shapeBrush.Color = Color.White;
                        e.Graphics.FillRectangle(shapeBrush, 58 + 30 * x - (j * 4), 701, 2, 8);
                    }
                    x++;
                }
            }
            foreach (Particle p in particles)
            {
                Form1.FillShape(p.shapeSides, p.shapeLvl, p.x, p.y, p.size, p.direction, p.colour, e);
            }

            if (pauseGame || gameState == -1)
            {
                e.Graphics.FillRectangle(darkGreyBrush, 200, 200, 400, 400);
                if (currentButtons.Count == 0)
                {
                    if (pauseGame)
                    {
                        menuMessage = "Pause Menu";
                        currentButtons.Add(buttons[0]);
                        currentButtons.Add(buttons[2]);
                    }
                    else if (gameState == -1)
                    {
                        menuMessage = "Game Over";
                        currentButtons.Add(buttons[1]);
                        currentButtons.Add(buttons[2]);
                    }
                }
                e.Graphics.DrawString(menuMessage, gameFont, whiteBrush, 400, 220, stringFormat);
            }
            else if (currentButtons.Count > 0)
            {
                currentButtons.Clear();
            }
            foreach (Button b in currentButtons)
            {
                if (b.Run(mouseX, mouseY, leftClick, gameFont, stringFormat, e))
                {
                    DoButtonFunction(b.action);
                    break;
                }
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
                case Keys.Q:
                    Qkey = false;
                    break;
            }
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
                case Keys.Q:
                    Qkey = true;
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
                    spawnList.Add(new Enemy(xSpawn, ySpawn, 20, random.Next(0, 5), random.Next(0, 360), 0));
                    break;
                case Keys.D1:
                    AddPowerup(new Powerup(Convert.ToInt16(enemies[0].x), Convert.ToInt16(enemies[0].y), -1, 1));
                    break;
                case Keys.D2:
                    AddPowerup(new Powerup(Convert.ToInt16(enemies[0].x), Convert.ToInt16(enemies[0].y), 17, 15));
                    break;
                case Keys.D3:
                    particles.Add(new Particle(400, 400, random.Next(0, 360), 10, 30, random.Next(3, 10), 1, Color.Lime));
                    break;
            }
        }
        private void PauseLabel_Click(object sender, EventArgs e)
        {
            PauseToggle();
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
            pauseGame = false;
            enemies.Clear();
            spawnList.Clear();
            powerups.Clear();
            ResetCounters();
            currentButtons.Clear();
            AddShape(400, 400, 0, Form1.playerStrength, 0, 1);
        }
        private void EnemyAI(Enemy enemy) // Any object (except dropped powerups) that isn't a player will do this
        {
            MoveEnemy(enemy);
            if (enemy.homing > 0 && enemy.target >= 0)
            {
                bool targetDead = true;
                foreach (Enemy target in enemies)
                {
                    if (target.id == enemy.target)
                    {
                        float targetAngle = (Functions.GetDirection(target.x - enemy.x, target.y - enemy.y) - enemy.direction + 540) % 360 - 180;
                        enemy.direction += Math.Sign(targetAngle) * enemy.homing;
                        targetDead = false;
                        break;
                    }
                }
                if (targetDead && enemy.type < 100)
                {
                    enemy.target = -1;
                    FindTarget(enemy);
                    if (enemy.type == 9)
                    {
                        enemy.targetType = 0;
                    }
                }
            }
            if (enemy.type < 100)
            {
                if (tick % 50 == 0 && random.Next(0, 2) == 1)
                {
                    FindTarget(enemy);
                }
                else
                {
                    if (Math.Abs(enemy.x - 400) > 400)
                    {
                        enemy.direction = Functions.Flip(enemy.direction, "y");
                        enemy.x = (enemy.x > 400) ? 800 : 0;
                    }
                    if (Math.Abs(enemy.y - 400) > 400)
                    {
                        enemy.direction = Functions.Flip(enemy.direction, "x");
                        enemy.y = (enemy.y > 400) ? 800 : 0;
                    }
                }
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
                        float distHit = (float)(en1.size * Math.Pow(1.1, en1.strength - minStrength) + en2.size * Math.Pow(1.1, en2.strength - minStrength));
                        float dirHit = Functions.GetDirection(en2.x - en1.x, en2.y - en1.y);

                        en1.x = en2.x - distHit * (float)Math.Sin(dirHit * Math.PI / 180);
                        en1.y = en2.y - distHit * (float)Math.Cos(dirHit * Math.PI / 180);
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
                if (en1.id != en2.id && (!((en1.type >= 100 || en2.type >= 100) && en1.team == en2.team) && en1.health > 0 && en2.health > 0 && !(en1.targetType == 3 && (en1.target == en2.id || en2.target == en1.id)) || en1.type == en2.type))
                {
                    double distHit = en1.size * Math.Pow(1.1, en1.strength - minStrength) + en2.size * Math.Pow(1.1, en2.strength - minStrength);
                    if (Functions.GetDistance(en1.x, en1.y, en2.x, en2.y) < distHit)
                    {
                        float centreX = (en1.x + en2.x) / 2;
                        float centreY = (en1.y + en2.y) / 2;
                        float dirHit = Functions.GetDirection(centreX - en1.x, centreY - en1.y);
                        float push = 1;
                        if (en1.weight != en2.weight) // Calculation for weight ratio
                        {
                            if (Math.Abs(en1.weight) > Math.Abs(en2.weight))
                            {
                                push = en2.weight / en1.weight;
                            }
                            else
                            {
                                push = 2 - en1.weight / en2.weight;
                            }
                        }
                        int bounce = 1;
                        if (en1.team != en2.team)
                        {
                            if (en1.type != 201)
                            {
                                bounce = 3;
                                en1.resistHits++;
                                en2.resistHits++;
                                if (en1.resistHits > en1.resistance)
                                {
                                    en1.resistHits = 0;
                                    en1.health -= en2.damage;
                                    en1.damageFlash = 2;
                                    if (en2.poisonDamage.duration > 0 && en1.type < 100)
                                    {
                                        en1.poisonTaken.Add(en2.poisonDamage);
                                    }
                                }
                                if (en2.resistHits > en2.resistance)
                                {
                                    en2.resistHits = 0;
                                    en2.health -= en1.damage;
                                    en2.damageFlash = 2;
                                    if (en1.poisonDamage.duration > 0 && en2.type < 100)
                                    {
                                        en2.poisonTaken.Add(en1.poisonDamage);
                                    }
                                }
                            }
                            else
                            {
                                if (en1.health >= en2.health && en2.damage <= 1000 * Math.Pow(1.5, en1.strength) && en2.strength - en1.strength <= 6 && en2.id != 0)
                                {
                                    ConvertConnected(en2.id, en1.team);
                                    en2.team = en1.team;
                                    en1.health -= en2.health;
                                    FindEnemiesLeft(0, true);
                                }
                            }
                        }
                        if (Math.Sign(en1.weight) * Math.Sign(en2.weight) >= 0)
                        {
                            en1.x += (float)(centreX - distHit * Math.Sin(dirHit * Math.PI / 180) / 2 - en1.x) * push * bounce;
                            en1.y += (float)(centreY - distHit * Math.Cos(dirHit * Math.PI / 180) / 2 - en1.y) * push * bounce;
                            en2.x += (float)(centreX + distHit * Math.Sin(dirHit * Math.PI / 180) / 2 - en2.x) * (2 - push) * bounce;
                            en2.y += (float)(centreY + distHit * Math.Cos(dirHit * Math.PI / 180) / 2 - en2.y) * (2 - push) * bounce;
                        }
                    }
                }
            }
            if (Math.Abs(en1.x - 400) > 410 && en1.type < 100)
            {
                en1.x = Math.Sign(en1.x - 400) * 410 + 400;
            }
            if (Math.Abs(en1.y - 400) > 410 && en1.type < 100)
            {
                en1.y = Math.Sign(en1.y - 400) * 410 + 400;
            }
        }
        private bool Shoot(Enemy enemy, int shots) // Shape will shoot if it's able to
        {
            bool canShoot = false;
            if (enemy.reload > 0 && enemy.reloadTimer == 0)
            {
                for (int i = 0; i < shots; i++)
                {
                    canShoot = true;
                    AddShape(enemy.x, enemy.y, enemy.type + 100 + ((enemy.id == 0 && powerupCounters[16] > 0) ? 1 : 0), enemy.strength, enemy.direction + (360 * i / shots), enemy.team);
                    enemies[enemies.Count - 1].SetColour(enemy.colour);
                    enemies[enemies.Count - 1].shotBy = enemy.id;
                    if (!(enemy.tp.X == 0 && enemy.tp.Y == 0))
                    {
                        enemies[enemies.Count - 1].direction = (int)Functions.GetDirection(enemy.tp.X - enemy.x, enemy.tp.Y - enemy.y);
                        enemy.tp = new Point(0, 0);
                    }
                    else
                    {
                        FindTarget(enemies[enemies.Count - 1]);
                    }

                    if (enemies[enemies.Count - 1].shotBy == 0)
                    {
                        if (powerupCounters[1] > 0) // homing bullets powerup
                        {
                            enemies[enemies.Count - 1].homing = 5 + powerupStrengths[1] * 3;
                            float shortestDistance = -1;
                            float mouseDistance = Functions.GetDistance(enemy.x, enemy.y, mouseX, mouseY);
                            for (int j = 0; j < enemies.Count; j++)
                            {
                                if (enemies[j].team != enemy.team && enemies[j].type < 100)
                                {
                                    float distance = Functions.GetDistance(enemies[j].x, enemies[j].y, (float)(enemy.x + mouseDistance * Math.Sin(enemies[enemies.Count - 1].direction * Math.PI / 180)), (float)(enemy.y + mouseDistance * Math.Cos(enemies[enemies.Count - 1].direction * Math.PI / 180)));
                                    if (distance < shortestDistance || shortestDistance == -1)
                                    {
                                        shortestDistance = distance;
                                        enemies[enemies.Count - 1].target = enemies[j].id;
                                    }
                                }
                            }
                        }
                        if (powerupCounters[14] > 0) // poisonous bullets powerup
                        {
                            enemies[enemies.Count - 1].poisonDamage.damage = ((enemies[enemies.Count - 1].damage / 150) * (powerupStrengths[14] + 1) * (float)Math.Pow(1.5, Form1.playerStrength));
                            enemies[enemies.Count - 1].poisonDamage.duration = 150;
                        }
                        enemies[enemies.Count - 1].resistance *= powerupStrengths[16] + 1;
                    }
                }
                switch (enemy.type) // special cases
                {
                    case 11:
                        FindTarget(enemy);
                        if (enemy.target >= 0)
                        {
                            enemy.speed = 20;
                        }
                        break;
                    case 18:
                        enemy.x = enemy.tp.X;
                        enemy.y = enemy.tp.Y;
                        enemy.direction = random.Next(0, 360);
                        enemy.tp = new Point(0, 0);
                        break;
                    case 113:
                        enemy.health = 0;
                        break;
                    default:
                        break;
                }
                enemy.reloadTimer = enemy.reload;
            }
            return canShoot;
        }
        private void Shoot(Enemy enemy, int type, int strength) // Shape will shoot a specific shape
        {
            AddShape(enemy.x, enemy.y, type, strength, enemy.direction, enemy.team);
            enemies[enemies.Count - 1].SetColour(enemy.colour);
            enemies[enemies.Count - 1].shotBy = enemy.id;
            if (!(enemy.tp.X == 0 && enemy.tp.Y == 0))
            {
                enemies[enemies.Count - 1].direction = Convert.ToInt16(Math.Round(Functions.GetDirection(enemy.tp.X - enemy.x, enemy.tp.Y - enemy.y)));
                enemy.tp = new Point(0, 0);
            }
            else
            {
                FindTarget(enemies[enemies.Count - 1]);
            }
        }
        void FindTarget(Enemy enemy) // Change direction and find the closest enemy if possible
        {
            if ((enemy.targetType != 1 && enemy.type < 100) || enemy.target == -1)
            {
                enemy.target = -1;
                enemy.direction = random.Next(0, 360); // Point in a random direction
                if (enemy.targetType != 2)
                {
                    float shortestDistance = -1;
                    for (int i = 0; i < enemies.Count; i++) // Change target to closest enemy, excluding bullets and powerup shapes
                    {
                        if (enemies[i].team != enemy.team && enemies[i].type < 100 && enemies[i].health > 0)
                        {
                            float distance = Functions.GetDistance(enemies[i].x, enemies[i].y, enemy.x, enemy.y);
                            if (distance < shortestDistance || shortestDistance == -1)
                            {
                                shortestDistance = distance;
                                enemy.target = enemies[i].id;
                            }
                        }
                    }
                    foreach (Enemy target in enemies) // Point toward the target
                    {
                        if (target.id == enemy.target && !(enemy.homing > 0 && enemy.type >= 100))
                        {
                            enemy.direction = Convert.ToInt16(Math.Round(Functions.GetDirection(target.x - enemy.x, target.y - enemy.y)));
                            break;
                        }
                    }
                }
            }
        }
        void UsePowerup(int pType, int type) // Spawn powerup shape if there are any of that type available, for player only
        {
            if (enemies[0].reloadTimer == 0 && powerupCounters[pType] > 0)
            {
                AddShape(enemies[0].x, enemies[0].y, type, Form1.playerStrength + powerupStrengths[pType], enemies[0].direction, enemies[0].team);
                enemies[enemies.Count - 1].SetColour(enemies[0].colour);
                enemies[0].reloadTimer = enemies[0].reload;
                powerupCounters[pType] -= powerupStrengths[pType] + 1;
            }
        }
        void NewWave(int waveNumber) // Determine which shapes are available on this wave and randomly select them based on their rarity
        {
            gameState = 0;
            enemyNumber = 0;
            if (enemies[0].health <= 0) // respawn with 25% health
            {
                enemies[0].health = enemies[0].maxHealth / 4;
            }

            typeList.Clear();
            typeList.Add(1);
            AddListShape(2, 1, waveNumber);
            AddListShape(3, 2, waveNumber);
            AddListShape(new int[] { 4, 7, 11 }, 5, waveNumber);
            AddListShape(new int[] { 5, 14, 15 }, 6, waveNumber);
            AddListShape(new int[] { 8, 9 }, 7, waveNumber);
            AddListShape(new int[] { 17, 18 }, 8, waveNumber);
            AddListShape(new int[] { 12, 16 }, 9, waveNumber);
            AddListShape(new int[] { 10, 13, 19 }, 11, waveNumber);


            if (wave >= 6)
            {
                minStrength = (int)Math.Floor((float)waveNumber / 3) - 1;
            }

            spawnPower = 10 * (float)Math.Pow(1.2, waveNumber);
            List<int> possibleShapes = new List<int>();
            foreach (int type in typeList)
            {
                for (int i = 0; i < typeChance[type]; i++)
                {
                    possibleShapes.Add(type);
                }
            }
            while (spawnPower > 0)
            {
                int enemyStrength = (int)Math.Floor((float)waveNumber / 3);
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
                spawnPower -= (float)Math.Pow(1.5, enemyStrength);
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
                int chosenType = possibleShapes[random.Next(0, possibleShapes.Count)];
                spawnList.Add(new Enemy(xSpawn, ySpawn, chosenType, enemyStrength, random.Next(0, 360), 0));
            }
            spawnTime = 500 / spawnList.Count;
            if (waveNumber % 5 == 4 && Form1.startingWave < waveNumber) // boss wave (every 5 waves)
            {
                Enemy boss = spawnList[random.Next(0, spawnList.Count)];
                boss.strength = Convert.ToInt16(Math.Round(Math.Log10(10 * Math.Pow(1.2, waveNumber)) / Math.Log10(1.5)));
                boss.isBoss = true;
                boss.ShapeSetup();
                spawnList.Clear();
                spawnList.Add(boss);
                spawnTime = 100;
                bossTimer = 200;
                if (boss.bossAttacks[0] != -1)
                {
                    bossAttack = boss.bossAttacks[random.Next(0, boss.bossAttacks.Length)];
                }
                else
                {
                    bossAttack = -1;
                }
            }
            else if (waveNumber > 5 && random.Next(0, 7) == 0 && Form1.startingWave < waveNumber) // special wave (1/7 chance)
            {
                int specialType = spawnList[random.Next(0, spawnList.Count)].type;
                foreach (Enemy en in spawnList)
                {
                    en.type = specialType;
                    en.ShapeSetup();
                }
            }
        }
        void FindEnemiesLeft(int team, bool shapesOnly) // Find the amount of shapes left on a certain team, if there are none on the team, either game over or wave complete
        {
            if (gameState != -1)
            {
                enemiesLeft = 0;
                foreach (Enemy en in enemies)
                {
                    if ((en.type < 100 || !shapesOnly) && en.team == team && en.health > 0)
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
                    else if (gameState != 2)
                    {
                        gameState = -1;
                        Form1.points += (int)Math.Round(score);
                        ResetCounters();
                        foreach (Enemy en in enemies)
                        {
                            if (en.team == 1)
                            {
                                en.health = 0;
                            }
                        }
                        if (wave > 3 && wave - 3 > Form1.startingWave)
                        {
                            Form1.startingWave = wave - 3;
                        }
                        else if (wave == Form1.startingWave && Form1.startingWave > 0)
                        {
                            Form1.startingWave--;
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
                    en.team = team;
                    ConvertConnected(en.id, team);
                }
            }
        }
        void MoveEnemy(Enemy enemy) // Shape moves in its direction and checks for collisions if there are shapes within range
        {
            bool collisionPossible = false;
            foreach (Enemy en in enemies)
            {
                if (Functions.GetDistance(en.x, en.y, enemy.x, enemy.y) < enemy.size * Math.Pow(1.1, enemy.strength - minStrength) + en.size * Math.Pow(1.1, en.strength - minStrength) + enemy.speed || enemy.targetType == 3)
                {
                    collisionPossible = true;
                    break;
                }
            }
            if (collisionPossible)
            {
                for (int i = 0; i < enemy.speed; i++)
                {
                    enemy.x += (float)Math.Sin(enemy.direction * Math.PI / 180);
                    enemy.y += (float)Math.Cos(enemy.direction * Math.PI / 180);
                    if (i % 5 == 0)
                    {
                        CheckCollisions(enemy);
                    }
                }
            }
            else
            {
                enemy.x += (float)Math.Sin(enemy.direction * Math.PI / 180) * enemy.speed;
                enemy.y += (float)Math.Cos(enemy.direction * Math.PI / 180) * enemy.speed;
            }
        }
        void SpecialAttack(Enemy enemy) // Performs the boss attack and resets the boss timer if possible
        {
            bossTimer = 100;
            switch (bossAttack)
            {
                case 0: // 5 Fast Triangles
                    FindEnemiesLeft(enemy.team, true);
                    if (enemiesLeft < 7)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Shoot(enemy, 2, enemy.strength - 5);
                        }
                        bossTimer = 250;
                    }
                    break;
                case 1: // teleport behind
                    foreach (Enemy en in enemies)
                    {
                        if (enemy.target == en.id)
                        {
                            enemy.x = en.x - (float)Math.Sin(en.direction * Math.PI / 180) * (enemy.size + en.size + 40);
                            enemy.y = en.y - (float)Math.Cos(en.direction * Math.PI / 180) * (enemy.size + en.size + 40);
                            bossTimer = 200;
                            break;
                        }
                    }
                    break;
                case 2: // 5 mines
                    for (int i = 0; i < 5; i++)
                    {
                        Shoot(enemy, 200, enemy.strength - 7);
                    }
                    bossTimer = 100;
                    break;
                case 3: // Laser
                    Shoot(enemy, 115, enemy.strength - 5);
                    bossTimer = 100;
                    break;
                case 4: // 2 Dangerous Triangles
                    FindEnemiesLeft(enemy.team, true);
                    if (enemiesLeft < 7)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Shoot(enemy, 14, enemy.strength - 5);
                        }
                        bossTimer = 200;
                    }
                    break;
                case 5: // 15 Team Switchers
                    FindEnemiesLeft(1, false);
                    if (enemiesLeft > 1)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            enemy.direction += 24;
                            Shoot(enemy, 201, enemy.strength - 3);
                        }
                        bossTimer = 100;
                    }
                    break;
                case 6: // 3 lower lvl shapes
                    FindEnemiesLeft(enemy.team, true);
                    if (enemiesLeft < 7)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Shoot(enemy, enemy.type, enemy.strength - 5);
                        }
                        bossTimer = 250;
                    }
                    break;

            }
        }
        void AddListShape(int[] type, int wave, int currentWave) // Adds shapes to the list of possible shapes if a certain wave is reached
        {
            if (currentWave >= wave)
            {
                for (int i = 0; i < type.Length; i++)
                {
                    typeList.Add(type[i]);
                }
            }
        }
        void AddListShape(int type, int wave, int currentWave) // Adds a shape to the list of possible shapes if a certain wave is reached
        {
            if (currentWave >= wave)
            {
                typeList.Add(type);
            }
        }
        void AddShape(float x, float y, int type, int strength, int direction, int team) // Adds a shape directly into the arena
        {
            enemies.Add(new Enemy(x, y, type, strength, direction, team));
            if (enemies[enemies.Count - 1].speed == 0)
            {
                enemies[enemies.Count - 1].x += (float)(random.NextDouble() * 2) - 1;
                enemies[enemies.Count - 1].y += (float)(random.NextDouble() * 2) - 1;
                foreach (Enemy en in enemies)
                {
                    CheckCollisions(en);
                }
            }
            FindEnemiesLeft(team, true);
        }
        void AddPowerup(Powerup powerup)
        {
            powerup.rand = random.Next(0, 360);
            powerups.Add(powerup);
        }
        void ResetCounters()
        {
            for (int i = 0; i < NUM_OF_POWERUPS; i++)
            {
                powerupCounters[i] = 0;
                powerupStrengths[i] = 0;
            }
        }
        void DoButtonFunction(int function)
        {
            switch (function)
            {
                case 0:
                    PauseToggle();
                    break;
                case 1:
                    NewGame();
                    break;
                case 2:
                    Form1.ChangeScreen(this, new TitleScreen());
                    break;
            }
        }
        void PauseToggle()
        {
            if (gameState != -1)
            {
                pauseGame = !pauseGame;
            }
        }
        int DrawSize(Enemy enemy)
        {
            return (int)(enemy.size * Math.Pow(1.1, enemy.strength - minStrength));
        }
        int DrawSize(Powerup powerup)
        {
            return (int)(20 * Math.Pow(1.1, powerup.strength - minStrength));
        }
        void Swap(object[] array, int i1, int i2) // Swaps two items in an object array
        {
            object item1 = array[i1];
            object item2 = array[i2];

            array[i1] = item2;
            array[i2] = item1;
        }
        void Swap(Color[] array, int i1, int i2) // Swaps two items in a Color array
        {
            Color item1 = array[i1];
            Color item2 = array[i2];

            array[i1] = item2;
            array[i2] = item1;
        }
    }
}