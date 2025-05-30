﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Geometry_Invasion
{
    public partial class Form1 : Form
    {
        public static int startingWave = 0;
        public static int playerStrength = 0;
        public static int points = 0;
        public static string[] data;
        public static int slot;
        public Form1()
        {
            InitializeComponent();
            RetrieveData();
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
        public static void FillShape(int sides, int level, float x, float y, float size, int rotation, Color colour, PaintEventArgs e)
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
        public static void DrawShape(int sides, int level, float x, float y, float size, int rotation, Color colour, int thickness, PaintEventArgs e)
        {
            Pen pen = new Pen(colour, thickness);
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
                e.Graphics.DrawPolygon(pen, points);
            }
            else
            {
                e.Graphics.DrawEllipse(pen, x - size, y - size, size * 2, size * 2);
            }
        }
        internal static void SaveData()
        {
            FileStream file;
            StreamWriter writer;
            file = File.OpenWrite("..\\..\\Resources\\save.txt");
            writer = new StreamWriter(file);
            data[slot] = $"{startingWave}|{playerStrength}|{points}";
            for (int i = 0; i < data.Length - 1; i++)
            {
                writer.WriteLine(data[i].Trim());
            }
            writer.Write(slot);
            writer.Close();
            file.Close();
        }
        internal void RetrieveData()
        {
            FileStream file;
            StreamReader reader;
            file = File.OpenRead("..\\..\\Resources\\save.txt");
            reader = new StreamReader(file);
            string str = reader.ReadToEnd();
            if (str != null)
            {
                data = str.Split('\n');
                slot = Convert.ToInt16(data[data.Length - 1]);
                string[] saveInfo = data[slot].Split('|');
                startingWave = int.Parse(saveInfo[0]);
                playerStrength = int.Parse(saveInfo[1]);
                points = int.Parse(saveInfo[2]);
            }
            reader.Close();
            file.Close();
        }
    }
}
