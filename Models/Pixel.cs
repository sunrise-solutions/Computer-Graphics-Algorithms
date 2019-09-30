﻿using System.Drawing;

namespace Models
{
    public class Pixel
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Color Color { get; set; }

        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
            Color = Color.Blue;
        }
    }
}
