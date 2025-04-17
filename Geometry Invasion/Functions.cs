using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Invasion
{
    internal static class Functions
    {
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
        public static float GetDistance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}
