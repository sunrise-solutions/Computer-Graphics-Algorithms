using System.Collections.Generic;

namespace Models
{
    public class Group
    {      
        // v 0.123 0.234 0.345 1.0
        public List<Vertex> Vertices { get; set; }

        // vt 0.500 1 [0]
        public List<VertexTexture> VertexTextures { get; set; }

        // vn 0.707 0.000 0.707
        public List<VertexNormal> VertexNormals { get; set; }

        // f 1 2 3
        public List<Face> Faces { get; set; }

        public Group()
        {
            Vertices = new List<Vertex>();
            VertexTextures = new List<VertexTexture>();
            VertexNormals = new List<VertexNormal>();
            Faces = new List<Face>();
        }
    }
}
