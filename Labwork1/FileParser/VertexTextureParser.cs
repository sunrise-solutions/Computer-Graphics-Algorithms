using Labwork1.Models;
using System;

namespace Labwork1.FileParser
{
    public static class VertexTextureParser
    {
        public static VertexTexture Parse(string line)
        {
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            VertexTexture vertexTexture = new VertexTexture();

            vertexTexture.U = parts[0].ParseInvariantFloat();
            vertexTexture.V = (parts.Length > 1) ? parts[1].ParseInvariantFloat() : 0;
            vertexTexture.W = (parts.Length > 2) ? parts[2].ParseInvariantFloat() : 0;

            return vertexTexture;
        }
    }
}
