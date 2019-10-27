using System;

namespace Models
{
    public class FaceElement : ICloneable
    {
        public int VertexIndex { get; set; }

        public int? VertexTextureIndex { get; set; }

        public int? VertexNormalIndex { get; set; }

        public FaceElement()
        {
        }

        public FaceElement(int vertexIndex, int? vertexTextureIndex, int? vertexNormalIndex)
        {
            VertexIndex = vertexIndex;
            VertexTextureIndex = vertexTextureIndex;
            VertexNormalIndex = vertexNormalIndex;
        }

        public object Clone()
        {
            return new FaceElement(this.VertexIndex, this.VertexTextureIndex, this.VertexNormalIndex);
        }
    }
}
