using System;

namespace Models
{
    // List of vertex normals in (x,y,z) form; normals might not be unit vectors.
    // vn 0.707 0.000 0.707
    public class VertexNormal : ICloneable
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public VertexNormal()
        {
        }

        public VertexNormal(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public object Clone()
        {
            return new VertexNormal(this.X, this.Y, this.Z);
        }
    }
}
