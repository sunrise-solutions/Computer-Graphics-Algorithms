namespace Labwork1.Models
{
    // List of geometric vertices, with (x, y, z [,w]) coordinates, w is optional and defaults to 1.0.
    // v 0.123 0.234 0.345 1.0
    public class GeometricVertex
    {
        // ?
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public int W { get; set; }
    }
}
