using System.Drawing;

namespace Models
{
    public class Pixel
    {
        public int X { get; set; }

        public int Y { get; set; }

        public float Z { get; set; }

        public Color Color { get; set; }

        public Pixel(int x, int y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            Color = Color.LightGreen;
        }
    }
}
