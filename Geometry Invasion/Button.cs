using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Geometry_Invasion
{
    internal class Button
    {
        public string text;
        public int action, x, y, width, height;
        SolidBrush buttonBrush;
        public Button(string _text, int _action, int _x, int _y, int _width, int _height, Color _colour)
        {
            text = _text;
            action = _action;
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            buttonBrush = new SolidBrush(_colour);
        }
        public bool Run(int mouseX, int mouseY, bool leftClick, Font gameFont, StringFormat stringFormat, PaintEventArgs e)
        {
            bool clicked = false;
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            e.Graphics.FillRectangle(buttonBrush, x - width / 2, y - height / 2, width, height);
            e.Graphics.DrawString(text, gameFont, whiteBrush, x, y - gameFont.Size / 2, stringFormat);
            if (leftClick && Math.Abs(mouseX - x) < width / 2 && Math.Abs(mouseY - y) < height / 2)
            {
                clicked = true;
            }
            return clicked;
        }
    }
}
