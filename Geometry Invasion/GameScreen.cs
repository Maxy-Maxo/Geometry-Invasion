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

namespace Geometry_Invasion
{
    public partial class GameScreen : UserControl
    {
        bool upKey, downKey, leftKey, rightKey;
        int wave = -1;
        double spawnPower;
        double spawnTime = 50;
        int enemiesLeft = 0;
        int enemyTimer = 0;
        int tick = 0;
        int delayTimer = 0;
        int gameState = 2;
        int mouseX, mouseY;
        bool mouseDown;
        public static int idCounter = 0;
        Random random = new Random();
        Font gameFont = new Font("VT323", 20);
        Font gameFontSmall = new Font("VT323", 15);

        List<Enemy> enemies = new List<Enemy>();
        List<Enemy> spawnList = new List<Enemy>();
        List<int> typeList = new List<int>();
        int[] typeRarities = { 0, 0, 20, 60, 40, 60, 0, 50, 60, 0, 0 }; // The chance of the type of enemy chosen being rerolled
        SolidBrush playerBrush = new SolidBrush(Color.Blue);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush greyBrush = new SolidBrush(Color.Gray);
        Pen barPen = new Pen(Color.White);
        public GameScreen()
        {
            InitializeComponent();
            enemies.Add(new Enemy(400, 400, 0, 0, 0, 1));
        }

        private void gameTimer_Tick(object sender, EventArgs e)
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
                if (spawnList[0].type == 8)
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
                    Enemy newSegment = new Enemy(en.x, en.y, en.type, en.strength, en.direction, en.team)
                    {
                        segments = en.segments - 1,
                        target = en.id,
                        targetType = 3
                    };
                    enemies.Add(newSegment);
                    en.segments = 0;
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
                    if (en.type == 5)
                    {
                        enemies.Add(new Enemy(en.x, en.y, 6, en.strength, en.direction, en.team));
                        enemies.Add(new Enemy(en.x, en.y, 6, en.strength, en.direction + 180, en.team));
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
                switch (en.type)
                {
                    case 0:
                        e.Graphics.FillEllipse(playerBrush, Convert.ToInt16(en.x) - Convert.ToInt16(en.size), Convert.ToInt16(en.y) - Convert.ToInt16(en.size), Convert.ToInt16(en.size * 2), Convert.ToInt16(en.size * 2));
                        Form1.FillShape(3, 1, Convert.ToInt16(en.x + (10 + en.size) * Math.Sin(en.direction * Math.PI / 180)), Convert.ToInt16(en.y + (10 + en.size) * Math.Cos(en.direction * Math.PI / 180)), 8, en.direction, Color.Blue, e);
                        break;
                    case 1:
                        Form1.FillShape(4, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction + 45, Color.Goldenrod, e);
                        break;
                    case 2:
                        Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction, Color.Lime, e);
                        break;
                    case 3:
                        Form1.FillShape(5, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction, Color.Purple, e);
                        break;
                    case 4:
                        Form1.FillShape(5, 2, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction + tick * 10, Color.HotPink, e);
                        break;
                    case 5:
                        Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction, Color.OrangeRed, e);
                        Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction + 180, Color.OrangeRed, e);
                        break;
                    case 6:
                        Form1.FillShape(3, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction, Color.OrangeRed, e);
                        break;
                    case 7:
                        Form1.FillShape(5, 2, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction, Color.Yellow, e);
                        break;
                    case 8:
                        Form1.FillShape(6, 1, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction, Color.DarkCyan, e);
                        shapeBrush.Color = Color.Black;
                        e.Graphics.FillEllipse(shapeBrush, Convert.ToInt16(en.x) - Convert.ToInt16(en.size * 0.5), Convert.ToInt16(en.y) - Convert.ToInt16(en.size * 0.5), Convert.ToInt16(en.size), Convert.ToInt16(en.size));
                        break;
                    case 9:
                        Form1.FillShape(7, 2, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction + tick * 5, Color.BlueViolet, e);
                        break;
                    case 10:
                        Form1.FillShape(8, 3, Convert.ToInt16(en.x), Convert.ToInt16(en.y), Convert.ToInt16(en.size), en.direction, Color.Magenta, e);
                        break;
                    // missles
                    case 100:
                        e.Graphics.FillEllipse(playerBrush, Convert.ToInt16(en.x) - Convert.ToInt16(en.size), Convert.ToInt16(en.y) - Convert.ToInt16(en.size), Convert.ToInt16(en.size * 2), Convert.ToInt16(en.size * 2));
                        break;
                    case 107:
                        shapeBrush.Color = Color.Yellow;
                        List<Point> vertices = new List<Point>
                        {
                            new Point(Convert.ToInt16(Math.Round(en.x + en.size * Math.Sin(en.direction * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + en.size * Math.Cos(en.direction * Math.PI / 180)))),
                            new Point(Convert.ToInt16(Math.Round(en.x + en.size * Math.Sin((en.direction + 144) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + en.size * Math.Cos((en.direction + 144) * Math.PI / 180)))),
                            new Point(Convert.ToInt16(Math.Round(en.x + en.size * Math.Sin((en.direction + 216) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + en.size * Math.Cos((en.direction + 216) * Math.PI / 180))))
                        };

                        Point[] points = vertices.ToArray();
                        e.Graphics.FillPolygon(shapeBrush, points);
                        break;
                    case 110:
                        shapeBrush.Color = Color.Magenta;
                        List<Point> vertices2 = new List<Point>
                        {
                            new Point(Convert.ToInt16(Math.Round(en.x + en.size * Math.Sin(en.direction * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + en.size * Math.Cos(en.direction * Math.PI / 180)))),
                            new Point(Convert.ToInt16(Math.Round(en.x + en.size * Math.Sin((en.direction + 135) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + en.size * Math.Cos((en.direction + 135) * Math.PI / 180)))),
                            new Point(Convert.ToInt16(Math.Round(en.x + en.size * Math.Sin((en.direction + 225) * Math.PI / 180))), Convert.ToInt16(Math.Round(en.y + en.size * Math.Cos((en.direction + 225) * Math.PI / 180))))
                        };

                        Point[] points2 = vertices2.ToArray();
                        e.Graphics.FillPolygon(shapeBrush, points2);
                        break;

                }
                if (en.type < 100)
                {
                    if (en.targetType != 3)
                    {
                        e.Graphics.DrawString($"Tier {en.strength + 1}", Font, whiteBrush, Convert.ToInt16(en.x), Convert.ToInt16(en.y + en.size + 10), stringFormat);
                    }
                    if (en.health != en.maxHealth)
                    {
                        e.Graphics.DrawRectangle(barPen, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - en.size - 20), 60, 4);
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
                        e.Graphics.FillRectangle(healthBrush, Convert.ToInt16(en.x - 30), Convert.ToInt16(en.y - en.size - 20), Convert.ToInt16(60 * en.health / en.maxHealth), 4);
                    }
                }
            }
            e.Graphics.DrawString($"Wave {wave + 1}", gameFont, whiteBrush, 400, 30, stringFormat);
            if (gameState == 2)
            {
                e.Graphics.DrawRectangle(barPen, 350, 70, 100, 30);
                e.Graphics.FillRectangle(greyBrush, 350, 70, 100 - delayTimer, 30);
                e.Graphics.DrawString("Next wave", gameFontSmall, whiteBrush, 400, 75, stringFormat);
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
                    spawnList.Add(new Enemy(xSpawn, ySpawn, 10, 5, random.Next(0, 360), 0));
                    break;
            }
        }
        private void EnemyAI(Enemy enemy)
        {
            for (int i = 0; i < enemy.speed; i++)
            {
                enemy.x += Math.Sin(enemy.direction * Math.PI / 180);
                enemy.y += Math.Cos(enemy.direction * Math.PI / 180);
                if (i % 5 == 0)
                {
                    if (CheckCollisions(enemy))
                    {
                        break;
                    }
                }
            }
            if (enemy.homing > 0 && enemy.target >= 0)
            {
                foreach (Enemy target in enemies)
                {
                    if (target.id == enemy.target)
                    {
                        double targetAngle = (Form1.GetDirection(target.x - enemy.x, target.y - enemy.y) - enemy.direction + 540) % 360 - 180;
                        enemy.direction += Math.Sign(targetAngle) * enemy.homing;
                    }
                }
            }
            if (enemy.targetType < 1 && (tick % 60 == 0 && random.Next(0, 2) == 1 || enemy.x > 790 || enemy.x < 10 || enemy.y > 790 || enemy.y < 10))
            {
                FindTarget(enemy);
            }
        }
        private bool CheckCollisions(Enemy en1)
        {
            bool collision = false;
            if (en1.targetType == 3)
            {
                foreach (Enemy en2 in enemies)
                {
                    if (en1.target == en2.id)
                    {
                        double distHit = (en1.size + en2.size) * 2;
                        double dirHit = Form1.GetDirection(en2.x - en1.x, en2.y - en1.y);

                        en1.x = en2.x - distHit * Math.Sin(dirHit * Math.PI / 180) / 2;
                        en1.y = en2.y - distHit * Math.Cos(dirHit * Math.PI / 180) / 2;
                        en1.direction = Convert.ToInt16(dirHit);
                        if (en1.team != en2.team)
                        {
                            en1.health -= en2.damage;
                            en2.health -= en1.damage;
                        }
                        collision = true;
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
                if (en1.x != en2.x && en1.y != en2.y && !(en1.type - 100 == en2.type || en2.type - 100 == en1.type && en1.team == en2.team) && en1.health > 0 && en2.health > 0 && !(en1.type == en2.type && en1.type >= 100) && !((en1.targetType == 3 && en1.target == en2.id) || (en2.targetType == 3 && en2.target == en1.id)))
                {
                    double distX = Math.Abs(en2.x - en1.x);
                    double distY = Math.Abs(en2.y - en1.y);
                    double distHit = en1.size + en2.size;
                    if (Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2)) < distHit)
                    {
                        double centreX = (en1.x + en2.x) / 2;
                        double centreY = (en1.y + en2.y) / 2;
                        double dirHit = Form1.GetDirection(centreX - en1.x, centreY - en1.y);
                        if (en2.type < 100)
                        {
                            en1.x = centreX - distHit * Math.Sin(dirHit * Math.PI / 180) / 2;
                            en1.y = centreY - distHit * Math.Cos(dirHit * Math.PI / 180) / 2;
                        }
                        if (en1.type < 100)
                        {
                            en2.x = centreX + distHit * Math.Sin(dirHit * Math.PI / 180) / 2;
                            en2.y = centreY + distHit * Math.Cos(dirHit * Math.PI / 180) / 2;
                        }
                        if (en1.team != en2.team)
                        {
                            en1.health -= en2.damage;
                            en2.health -= en1.damage;
                        }
                        collision = true;
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
            return collision;
        }
        private bool Shoot(Enemy enemy)
        {
            bool canShoot = false;
            if (enemy.reload > 0 && enemy.reloadTimer == 0)
            {
                canShoot = true;
                for (int i = 0; i < enemy.shots; i++)
                {
                    enemies.Add(new Enemy(enemy.x, enemy.y, enemy.type + 100, enemy.strength, enemy.direction + (360 * i / enemy.shots), enemy.team));
                    if (enemies[enemies.Count - 1].targetType < 2)
                    {
                        FindTarget(enemies[enemies.Count - 1]);
                    }
                }
                enemy.reloadTimer = enemy.reload;
            }
            return canShoot;
        }
        private void FindTarget(Enemy enemy)
        {
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
                        if (wave >= 6)
                        {
                            typeList.Add(5);
                            typeList.Add(7);
                            if (wave >= 7)
                            {
                                typeList.Add(8);
                            }
                        }
                    }
                }
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
                    while ((random.Next(0, 3) < 1 || Math.Pow(1.5, enemyStrength) > spawnPower) && enemyStrength > 0)
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
