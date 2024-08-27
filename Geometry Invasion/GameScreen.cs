﻿using System;
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

namespace Geometry_Invasion
{
    public partial class GameScreen : UserControl
    {
        bool upKey, downKey, leftKey, rightKey, mouseDown, pauseGame;
        int wave = -1;
        double score = 0;
        double spawnPower;
        double spawnTime = 50;
        int enemiesLeft = 0;
        int enemyTimer = 0;
        public static int minStrength = 0;
        int tick = 0;
        int delayTimer = 0;
        int gameState = 2;
        int mouseX, mouseY;
        public static int idCounter = 0;
        Random random = new Random();
        Font gameFont = new Font("VT323", 20);
        Font gameFontSmall = new Font("VT323", 15);

        List<Enemy> enemies = new List<Enemy>();
        List<Enemy> spawnList = new List<Enemy>();
        List<int> typeList = new List<int>();
        int[] typeRarities = { 0, 0, 20, 60, 40, 60, 0, 50, 60, 60, 50, 50 }; // The chance of the type of enemy chosen being rerolled
        SolidBrush playerBrush = new SolidBrush(Color.Blue);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush greyBrush = new SolidBrush(Color.Gray);
        SolidBrush darkGreyBrush = new SolidBrush(Color.FromArgb(30, 30, 30));
        Pen barPen = new Pen(Color.White);
        public GameScreen()
        {
            InitializeComponent();
            enemies.Add(new Enemy(400, 400, 0, 0, 0, 1));
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (!pauseGame)
            {
                if (gameState != -1)
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
                    if (mouseDown)
                    {
                        Shoot(enemies[0]);
                    }
                }

                if (gameState == 0)
                {
                    enemyTimer++;
                }
                else if (gameState == 2)
                {
                    delayTimer++;
                    if (delayTimer >= 100)
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
                    if ((en.type >= 100 && (en.x > 810 || en.x < -10 || en.y > 810 || en.y < -10)) || en.health <= 0)
                    {
                        if (en.id != 0)
                        {
                            if (en.type == 5)
                            {
                                enemies.Add(new Enemy(en.x, en.y, 6, en.strength, en.direction, en.team));
                                enemies.Add(new Enemy(en.x, en.y, 6, en.strength, en.direction + 180, en.team));
                            }
                            if (en.team == 0)
                            {
                                score += en.scoreValue;
                            }
                            enemies.Remove(en);
                            enemiesLeft = 0;
                            foreach (Enemy en2 in enemies)
                            {
                                if (en2.type < 100 && en2.team == 0)
                                {
                                    enemiesLeft++;
                                }
                            }
                            if (enemiesLeft == 0 && gameState == 1)
                            {
                                gameState = 2;
                            }
                            break;
                        }
                        else
                        {
                            gameState = -1;
                        }
                    }
                    if (en.type == 11 && en.speed > 4)
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
                    }
                }
            }

            Refresh();
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            StringFormat stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center
            };
            SolidBrush shapeBrush = new SolidBrush(Color.White);
            foreach (Enemy en in enemies)
            {
                if (en.health > 0)
                {
                    double drawSize = en.size * Math.Pow(1.1, en.strength - minStrength);
                    switch (en.type)
                    {
                        case 0:
                            e.Graphics.FillEllipse(playerBrush, Convert.ToInt16(en.x) - Convert.ToInt16(drawSize), Convert.ToInt16(en.y) - Convert.ToInt16(drawSize), Convert.ToInt16(drawSize * 2), Convert.ToInt16(drawSize * 2));
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x + (10 + drawSize) * Math.Sin(en.direction * Math.PI / 180)), Convert.ToInt16(en.y + (10 + drawSize) * Math.Cos(en.direction * Math.PI / 180)), 8, en.direction, Color.Blue, e);
                            break;
                        case 1:
                            Form1.FillShape(4, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction + 45, Color.Goldenrod, e);
                            break;
                        case 2:
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction, Color.Lime, e);
                            break;
                        case 3:
                            Form1.FillShape(5, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction, Color.Purple, e);
                            break;
                        case 4:
                            Form1.FillShape(5, 2, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction + tick * 10, Color.HotPink, e);
                            break;
                        case 5:
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction, Color.OrangeRed, e);
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction + 180, Color.OrangeRed, e);
                            break;
                        case 6:
                            Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction, Color.OrangeRed, e);
                            break;
                        case 7:
                            Form1.FillShape(5, 2, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction, Color.Yellow, e);
                            break;
                        case 8:
                            Form1.FillShape(6, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction, Color.DarkCyan, e);
                            shapeBrush.Color = Color.Black;
                            e.Graphics.FillEllipse(shapeBrush, Convert.ToInt16(en.x) - Convert.ToInt16(drawSize * 0.5), Convert.ToInt16(en.y) - Convert.ToInt16(drawSize * 0.5), Convert.ToInt16(drawSize), Convert.ToInt16(drawSize));
                            break;
                        case 9:
                            Form1.FillShape(7, 2, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction + tick * 5, Color.BlueViolet, e);
                            Form1.FillShape(7, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize * 0.5), en.direction + tick * -5, Color.BlueViolet, e);
                            break;
                        case 10:
                            Form1.FillShape(8, 3, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction, Color.Magenta, e);
                            break;
                        case 11:
                            Form1.FillShape(6, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(drawSize), en.direction, Color.DarkRed, e);
                            break;
                        // missles
                        case 100:
                            e.Graphics.FillEllipse(playerBrush, Convert.ToInt16(en.x) - Convert.ToInt16(drawSize), Convert.ToInt16(en.y) - Convert.ToInt16(drawSize), Convert.ToInt16(drawSize * 2), Convert.ToInt16(drawSize * 2));
                            break;
                        case 107:
                            shapeBrush.Color = Color.Yellow;
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
                            shapeBrush.Color = Color.Magenta;
                            List<Point> vertices2 = new List<Point>
                        {
                            new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin(en.direction * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos(en.direction * Math.PI / 180)))),
                            new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 135) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 135) * Math.PI / 180)))),
                            new Point(Convert.ToInt16(Math.Round(en.x + drawSize * Math.Sin((en.direction + 225) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + drawSize * Math.Cos((en.direction + 225) * Math.PI / 180))))
                        };

                            Point[] points2 = vertices2.ToArray();
                            e.Graphics.FillPolygon(shapeBrush, points2);
                            break;

                    }
                    if (en.type < 100)
                    {
                        if (en.targetType != 3)
                        {
                            e.Graphics.DrawString($"Tier {en.strength + 1}", Font, whiteBrush, Convert.ToInt16(en.x), Convert.ToInt16(en.y + drawSize + 10), stringFormat);
                        }
                        if (en.health != en.maxHealth)
                        {
                            e.Graphics.DrawRectangle(barPen, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 20), 60, 4);
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
                            e.Graphics.FillRectangle(healthBrush, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - drawSize - 20), Convert.ToInt16(60 * en.health / en.maxHealth), 4);
                        }
                    }
                }
            }
            e.Graphics.DrawString($"Wave {wave + 1}", gameFont, whiteBrush, 400, 30, stringFormat);
            e.Graphics.DrawString($"Score: {Math.Round(score * 10) / 10}", gameFontSmall, whiteBrush, 35, 750);
            if (gameState == 2)
            {
                e.Graphics.DrawRectangle(barPen, 350, 70, 100, 30);
                e.Graphics.FillRectangle(greyBrush, 350, 70, 100 - delayTimer, 30);
                e.Graphics.DrawString("Next wave", gameFontSmall, whiteBrush, 400, 75, stringFormat);
            }
            e.Graphics.FillRectangle(darkGreyBrush, 750, 5, 45, 45);

            if (pauseGame || gameState == -1)
            {
                e.Graphics.FillRectangle(darkGreyBrush, 200, 200, 400, 400);

                string message = "";
                if (pauseGame)
                {
                    message = "Pause Menu";
                }
                else if (gameState == -1)
                {
                    message = "Game Over";
                }
                e.Graphics.DrawString(message, gameFont, whiteBrush, 400, 220, stringFormat);
            }
        }

        private void GameScreen_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
        }

        private void GameScreen_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
        }

        private void GameScreen_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
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
                    spawnList.Add(new Enemy(xSpawn, ySpawn, 11, 3, random.Next(0, 360), 1));
                    break;
            }
        }
        private void EnemyAI(Enemy enemy)
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
                if (targetDead && enemy.type < 100)
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
        private void CheckCollisions(Enemy en1)
        {
            if (en1.targetType == 3)
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
            foreach (Enemy en2 in enemies)
            {
                if (en1.x != en2.x && en1.y != en2.y && !(en1.type >= 100 || en2.type >= 100 && en1.team == en2.team) && en1.health > 0 && en2.health > 0 && !(en1.type == en2.type && en1.type >= 100) && !((en1.targetType == 3 && en1.target == en2.id) || (en2.targetType == 3 && en2.target == en1.id)) && !(en1.type == 9 && (en1.target == en2.id || en2.target == en1.id)))
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
                        if (en1.weight != en2.weight)
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
                            en1.health -= en2.damage;
                            en2.health -= en1.damage;
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
        private bool Shoot(Enemy enemy)
        {
            bool canShoot = false;
            if (enemy.reload > 0 && enemy.reloadTimer == 0)
            {
                for (int i = 0; i < enemy.shots; i++)
                {
                    canShoot = true;
                    enemies.Add(new Enemy(enemy.x, enemy.y, enemy.type + 100, enemy.strength, enemy.direction + (360 * i / enemy.shots), enemy.team));
                    if (enemies[enemies.Count - 1].targetType < 2)
                    {
                        FindTarget(enemies[enemies.Count - 1]);
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
                enemy.reloadTimer = enemy.reload;
            }
            return canShoot;
        }
        private void FindTarget(Enemy enemy)
        {
            enemy.target = -1;
            if (enemy.targetType != 1)
            {
                enemy.direction = random.Next(0, 360);
            }
            double distance = -1;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].team != enemy.team && enemies[i].type < 100)
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
            foreach (Enemy target in enemies)
            {
                if (target.id == enemy.target && !(enemy.homing > 0 && enemy.type >= 100))
                {
                    enemy.direction = Convert.ToInt16(Math.Round(Form1.GetDirection(target.x - enemy.x, target.y - enemy.y)));
                    break;
                }
            }
        }
        public void NewWave(int waveNumber)
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
                                    typeList.Add(10);
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
        }
    }
}
