using System;

namespace Models
{
    // List of geometric vertices, with (x, y, z [,w]) coordinates, w is optional and defaults to 1.0.
    // v 0.123 0.234 0.345 1.0
    public class Vertex : ICloneable
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

        public Vertex()
        {
            //W = 1;
        }

        public Vertex(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public object Clone()
        {
            return new Vertex(this.X, this.Y, this.Z, this.W);
        }
    }
}
