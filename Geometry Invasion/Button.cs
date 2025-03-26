using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Geometry_Invasion
{
    internal class Button
    {
        public string text;
        public int action, x, y, length, width;
        public Color colour;
        public Button(string _text, int _action, int _x, int _y, int _length, int _width, Color _colour)
        {
            text = _text;
            action = _action;
            x = _x;
            y = _y;
            length = _length;
            width = _width;
            colour = _colour;
        }
    }
}
