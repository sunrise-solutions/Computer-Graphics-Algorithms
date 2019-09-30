using Models;
using System;
using System.Collections.Generic;

namespace DrawingAlgorithm
{
    public static class Bresenham
    {
        public static IEnumerable<Pixel> GetPixels(int x0, int y0, int x1, int y1)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            List<Pixel> line = new List<Pixel>();
            for (; ; )
            {
                line.Add(new Pixel(x0, y0));
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
            return line;
        }
    }
}
