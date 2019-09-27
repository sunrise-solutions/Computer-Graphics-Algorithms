using System.Collections.Generic;

namespace Labwork1.Models
{
    public class GraphElement
    {      
        // v 0.123 0.234 0.345 1.0
        public List<GeometricVertex> Vertices { get; set; }

        // vt 0.500 1 [0]
        public List<VertexTexture> Textures { get; set; }

        // vn 0.707 0.000 0.707
        public List<VertexNormal> Normals { get; set; }

        // f 1 2 3
        public List<Face> Faces { get; set; }
    }
}
