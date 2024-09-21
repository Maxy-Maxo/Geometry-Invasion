using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geometry_Invasion
{
    public partial class Form1 : Form
    {
        public static int startingWave = 0;
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
        public static double GetDirection(int xSpeed, int ySpeed)
        {
            double direction;

            if (ySpeed == 0)
            {
                if (xSpeed < 0)
                {
                    return -90;
                }
                else
                {
                    return 90;
                }
            }
            else
            {
                direction = Math.Atan2(xSpeed, ySpeed) * 180 / Math.PI;

                if (xSpeed <= 0 && ySpeed > 0)
                {
                    direction += 360;
                }

                return direction;
            }

        }
        public static double GetDirection(double xSpeed, double ySpeed)
        {
            double direction;

            if (ySpeed == 0)
            {
                if (xSpeed < 0)
                {
                    return -90;
                }
                else
                {
                    return 90;
                }
            }
            else
            {
                direction = Math.Atan2(xSpeed, ySpeed) * 180 / Math.PI;

                if (xSpeed <= 0 && ySpeed > 0)
                {
                    direction += 360;
                }

                return direction;
            }

        }
    }
}
