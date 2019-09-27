using Labwork1.Models;
using System;

namespace Labwork1.FileParser
{
    public static class FaceParser
    {
        public static Face Parse(string line)
        {

            var vertices = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Face face = new Face();

            foreach (var v in vertices)
            {
                FaceElement faceElement = ParseFaceElement(v);
                face.FaceElements.Add(faceElement);
            }
            return face;
        }

        private static FaceElement ParseFaceElement(string vertexString)
        {
            var fields = vertexString.Split(new[] { '/' }, StringSplitOptions.None);

            FaceElement faceElement = new FaceElement();

            int vertexIndex = fields[0].ParseInvariantInt();
            faceElement.VertexIndex = (int)ChangeIndex(vertexIndex);

            if (fields.Length > 1)
            {
                int? vertexTextureIndex = fields[1].Length == 0 ? null : (int?)fields[1].ParseInvariantInt();
                faceElement.VertexTextureIndex = ChangeIndex(vertexTextureIndex);
            }

            if (fields.Length > 2)
            {
                var vertexNormalIndex = fields.Length > 2 && fields[2].Length == 0 ? null : (int?)fields[2].ParseInvariantInt();
                faceElement.VertexNormalIndex = vertexNormalIndex;
            }

            return faceElement;
        }

        /// <summary>
        /// List Indexing starts from 0, but obj-file using the indexing that starts from 0.
        /// This method helps to synchronized the indexing.
        /// </summary>
        private static int? ChangeIndex(int? index)
        {
            return (index == null) ? null : index - 1;
        }
    }
}
