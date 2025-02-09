using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Geometry_Invasion
{
    public partial class Form1 : Form
    {
        public static int startingWave = 0;
        public static int playerStrength = 0;
        public Form1()
        {
            InitializeComponent();
            ChangeScreen(this, new TitleScreen());
        }
        public static void ChangeScreen(object sender, UserControl next)
        {
            Form f; // will either be the sender or parent of sender

            if (sender is Form)
            {
                f = (Form)sender;
            }

            else
            {
                UserControl current = (UserControl)sender;
                f = current.FindForm();
                f.Controls.Remove(current);
            }

            next.Location = new Point((f.ClientSize.Width - next.Width) / 2, (f.ClientSize.Height - next.Height) / 2);

            f.Controls.Add(next);
            next.Focus();
        }
        public static void FillShape(int sides, int level, int x, int y, int size, int rotation, Color colour, PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(colour);
            if (sides > 2) // Less than 3 sides makes a circle
            {
                double counter = rotation;

                List<Point> vertices = new List<Point>();
                for (int i = 0; i < sides; i++)
                {
                    vertices.Add(new Point(Convert.ToInt16(Math.Round(x + size * Math.Sin(counter * Math.PI / 180))), Convert.ToInt16(Math.Round(y + size * Math.Cos(counter * Math.PI / 180)))));
                    counter += 3600 * level / sides / 10;
                }
                Point[] points = vertices.ToArray();
                e.Graphics.FillPolygon(brush, points);
            }
            else
            {
                e.Graphics.FillEllipse(brush, x - size, y - size, size * 2, size * 2);
            }
        }
        public static float GetDirection(double x, double y)
        {
            float direction;

            if (y == 0)
            {
                if (x < 0)
                {
                    direction = 270;
                }
                else
                {
                    direction = 90;
                }
            }
            else
            {
                direction = (float)(Math.Atan2(x, y) * 180 / Math.PI);
            }
            return (direction + 360) % 360;
        }
        public static int Flip(double direction, string axis)
        {
            float x = (float)Math.Sin(direction * Math.PI / 180);
            float y = (float)Math.Cos(direction * Math.PI / 180);
            switch (axis)
            {
                case "x":
                    return (int)Math.Round(GetDirection(x, y * -1));
                case "y":
                    return (int)Math.Round(GetDirection(x * -1, y));
                default:
                    return -1;
            }
        }
    }
}
