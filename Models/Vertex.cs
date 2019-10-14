namespace Models
{
    // List of geometric vertices, with (x, y, z [,w]) coordinates, w is optional and defaults to 1.0.
    // v 0.123 0.234 0.345 1.0
    public class Vertex
    {
        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float W { get; set; }

        public Vertex()
        {
            W = 1;
        }
    }
}
