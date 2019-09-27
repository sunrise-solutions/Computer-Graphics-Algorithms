using Labwork1.Models;
using System;

namespace Labwork1.FileParser
{
    public static class VertexNormalParser
    {
        public static VertexNormal Parse(string line)
        {
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            VertexNormal vertexNormal = new VertexNormal();

            vertexNormal.X = parts[0].ParseInvariantFloat();
            vertexNormal.Y = parts[1].ParseInvariantFloat();
            vertexNormal.Z = parts[2].ParseInvariantFloat();

            return vertexNormal;
        }
    }
}
