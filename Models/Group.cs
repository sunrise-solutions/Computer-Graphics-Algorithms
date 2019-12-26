using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Group : ICloneable
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

        public Group(List<Vertex> vertices, List<Face> faces, List<VertexTexture> textures, List<VertexNormal> normals)
        {
            Vertices = vertices;
            VertexTextures = textures;
            VertexNormals = normals;
            Faces = MakeTriangleFaceList(faces);
        }

        private List<Face> MakeTriangleFaceList(List<Face> list)
        {
            List<Face> triangleFaceList = new List<Face>();

            foreach (Face face in list)
            {
                if (face.FaceElements.Count < 3)
                {
                    throw new ArgumentException("Should be 3 values");
                }

                for (int i = 1; i < face.FaceElements.Count - 1; i++) // devide face into triangles
                {
                    Face triangleFace = new Face();
                    triangleFace.FaceElements.Add((FaceElement)face.FaceElements[0].Clone());
                    triangleFace.FaceElements.Add((FaceElement)face.FaceElements[i].Clone());
                    triangleFace.FaceElements.Add((FaceElement)face.FaceElements[i+1].Clone());
                    triangleFaceList.Add(triangleFace);
                }
            }

            return triangleFaceList;
        }

        public object Clone()
        {
            var pointsList = this.Vertices.Select(x => (Vertex)x.Clone()).ToList();
            var facesList = this.Faces.Select(x => (Face)x.Clone()).ToList();
            var normalList = this.VertexNormals.Select(x => (VertexNormal)x.Clone()).ToList();
            var textureList = this.VertexTextures.Select(x => (VertexTexture)x.Clone()).ToList();

            return new Group(pointsList, facesList, textureList, normalList);
        }
    }
}
